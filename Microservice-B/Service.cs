using Microservice_B.Connection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC3.Util.DI.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Microservice_B
{
    public class Service : BackgroundService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<Service> _logger;

        public Service(ILogger<Service> logger, ILoggerFactory loggerFactory, IConnectionManager connectionManager)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _connectionManager = connectionManager;

            StaticLogger.LoggerFactory = loggerFactory; // Set Static Logger Logger Factory
        }

        public override Task StartAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Microservice-B starting...");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connectionManager.Start();

            _logger.LogInformation("Microservice-B started.");

            var input = "";
            while (input != "exit")
            {
                input = Console.ReadLine();
                Console.WriteLine($"You wrote: {input}");
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Microservice-B stopping...");
            _connectionManager.Stop();
            _logger.LogInformation("Microservice-B stopped!");

            return base.StopAsync(cancellationToken);
        }
    }
}
