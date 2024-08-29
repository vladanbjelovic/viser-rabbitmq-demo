using Microservice_A.Connection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC3.Util.DI.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Microservice_A
{
    public class Service : BackgroundService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<Service> _logger;
        private readonly InputTestService _testService;

        public Service(ILogger<Service> logger, ILoggerFactory loggerFactory, IConnectionManager connectionManager, InputTestService testService)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _connectionManager = connectionManager;
            _testService = testService; 

            StaticLogger.LoggerFactory = loggerFactory; // Set Static Logger Logger Factory
        }

        public override Task StartAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Microservice-A starting...");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connectionManager.Start();

            _logger.LogInformation("Microservice-A started.");

            _testService.ListenForInputAsync(stoppingToken);

            return Task.CompletedTask;
        }

        public override Task StopAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Microservice-A stopping...");
            _connectionManager.Stop();
            _logger.LogInformation("Microservice-A stopped!");

            return base.StopAsync(cancellationToken);
        }
    }
}
