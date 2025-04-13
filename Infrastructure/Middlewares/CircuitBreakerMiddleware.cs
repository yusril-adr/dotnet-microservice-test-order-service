
using DotnetOrderService.Immutables;
using DotnetOrderService.Constants.CircuitBreaker;
using Microsoft.EntityFrameworkCore;
using DotnetOrderService.Infrastructure.Exceptions;

namespace DotnetOrderService.Infrastructure.Middlewares
{

    public class CircuitBreakerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        private readonly List<string> ignoreError = [
            typeof(ValidationException).Name,
            typeof(BusinessException).Name,
            typeof(BadHttpRequestException).Name,
            typeof(DbUpdateException).Name,
            typeof(UnauthenticatedException).Name,
            typeof(ServiceUnavailableException).Name,
            typeof(UnauthorizedAccessException).Name,
            typeof(NotAllowedException).Name,
        ];

        public CircuitBreakerMiddleware(
            IConfiguration config,
            RequestDelegate next
        )
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(
            HttpContext context
        )
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                // Circuit Breaker Handling for Fault Tolerant Type
                var isCheckDone = await CheckFaultTolerant(context, error);
                if (isCheckDone)
                {
                    return;
                }

                // Re-throw to Exception Handler
                throw;
            }
        }

        private async Task<bool> CheckFaultTolerant(HttpContext context, Exception error)
        {
            var isActive = bool.Parse(_config["CircuitBreaker:internal:IsActive"] ?? "false");
            if (!isActive)
            {
                await _next(context);
                return true;
            }

            var typeOfError = error.GetType().Name;
            if (ignoreError.Contains(typeOfError))
            {
                await _next(context);
                return true;
            }

            var keyCircuitBreaker = CircuitBreakerConstant.CIRUCUIT_BREAKER_PREFIX + context.Request.Path.ToString().Replace("/", "-");
            var cooldown = int.Parse(_config["CircuitBraeker:Internal:Cooldown"] ?? "2");
            FaultTolerant.Set(keyCircuitBreaker, cooldown);

            return false;
        }
    }
}
