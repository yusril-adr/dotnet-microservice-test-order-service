using Microsoft.Data.SqlClient;

namespace DotNetService.Infrastructure.Queues
{
    public class ConnectionPoolCheckerService(ILogger<ConnectionPoolCheckerService> logger, string connectionString, int interval) : BackgroundService
    {
        private readonly ILogger<ConnectionPoolCheckerService> _logger = logger;
        private readonly string _connectionString = connectionString;
        private readonly int _interval = interval;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Connection Pool Checker Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckConnectionPoolSize(stoppingToken);

                    // Use WaitForDelay pattern to handle cancellation properly
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_interval), stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // Log graceful shutdown
                        _logger.LogInformation("Connection Pool Checker Service is shutting down gracefully.");
                        break;
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "An error occurred while checking connection pool size.");
                }
            }

            _logger.LogInformation("Connection Pool Checker Service has stopped.");
        }

        private async Task CheckConnectionPoolSize(CancellationToken cancellationToken)
        {
            var query = @"
        SELECT COUNT(connection_id) AS active_connections,
               net_transport,
               auth_scheme
        FROM sys.dm_exec_connections
        GROUP BY net_transport, auth_scheme;";

            using var connection = new SqlConnection(_connectionString);

            // Increase timeout to 60 seconds
            using var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(360));
            using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);
            var linkedToken = linkedSource.Token;

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync(linkedToken)
                        .ConfigureAwait(false);
                }

                using var command = new SqlCommand(query, connection)
                {
                    CommandTimeout = 30 // Set command timeout
                };

                using var reader = await command.ExecuteReaderAsync(linkedToken)
                    .ConfigureAwait(false);

                while (!linkedToken.IsCancellationRequested && await reader.ReadAsync(linkedToken).ConfigureAwait(false))
                {
                    var now = DateTime.UtcNow;
                    int activeConnections = reader.GetInt32(0);
                    string netTransport = reader.GetString(1);
                    string authScheme = reader.GetString(2);

                    _logger.LogInformation(
                        "{Now:yyyy-MM-dd HH:mm:ss.fff} - Active Connections: {ActiveConnections}, Net Transport: {NetTransport}, Auth Scheme: {AuthScheme}",
                        now, activeConnections, netTransport, authScheme);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Connection pool check was canceled: {Message}", ex.Message);
                // Rethrow if it's a service shutdown, swallow if it's just a timeout
                if (cancellationToken.IsCancellationRequested) throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError("Database error during pool check: {Message}, Error Code: {Code}",
                    ex.Message, ex.Number);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during connection pool check");
                throw;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    try
                    {
                        await connection.CloseAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error closing connection: {Message}", ex.Message);
                    }
                }
            }
        }
    }
}