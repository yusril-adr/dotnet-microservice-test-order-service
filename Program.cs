using DotNetOrderService.Infrastructure.Filters;
using RuangDeveloper.AspNetCore.Command;

namespace DotNetOrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
            .Build()
            .RunWithCommands(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddCommandLine(args)
                .Build();

            double sentryTraceSampleRate = double.Parse(config["Sentry:TracesSampleRate"] ?? "0.1");
            //get dsn value
            string dsn = config["Sentry:Dsn"] ?? "";
            string env = config["App:Environment"] ?? "Development";
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry(o =>
                    {
                        o.Dsn = dsn;
                        o.TracesSampleRate = sentryTraceSampleRate;
                        o.Environment = env;
                        o.AutoSessionTracking = true;
                        o.Debug = true;
                        o.StackTraceMode = StackTraceMode.Enhanced;
                        o.ServerName = config["App:Name"];

                        o.AddExceptionFilter(new SentryExceptionFilter());
                        o.SetBeforeSend((sentryEvent) =>
                        {
                            // Manual send sentry error if Exception exist and status code 500, config at HandlerException.cs
                            if (sentryEvent.Exception == null) return null;
                            return sentryEvent;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });

            return hostBuilder;
        }
    }
}
