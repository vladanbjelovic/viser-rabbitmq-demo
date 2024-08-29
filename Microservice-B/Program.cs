using Logging;
using Microservice_B.Connection;
using Microservice_B.Remote;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC3.Comm.Core.Layer.Config;
using NC3.Microservices.Util.StartOptions;
using NC3.Util.DI.Extensions;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Microservice_B
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Set current directory as working (because of windows services where working dir is C:\Windows\System32 or C:\Windows\SysWow64)
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Log.For(null, "Microservice-B").Info($"Microservice initializing...");

            #region Environment

            string environment;
#if DEBUG
            environment = "Development";
#elif STAGING
            environment = "Staging";
#elif RELEASE
            environment = "Production";
#endif
            Log.For(null, "Microservice-B").Info($"Environment: {environment}");

            #endregion

            WindowsServiceInstaller.APP_EXECUTABLE_PATH =
                Assembly.GetExecutingAssembly().Location
                    .Remove(Assembly.GetExecutingAssembly().Location.Length - 4) + ".exe";
            WindowsServiceInstaller.OnStatusChanged +=
                status =>
                {
                    Log.For(null, "Microservice-B").Info($"Service info: {status}");
                };

            Log.For(null, "Microservice-B").Info($"Version: {Assembly.GetEntryAssembly().GetName().Version}");

            var hostBuilder = CreateHostBuilder(args);
            var argumentParser = new ArgumentParser(args);
            var host = new HostedService(argumentParser);
            await host.RunAsync(hostBuilder)
                .ConfigureAwait(false);

            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                }).ConfigureServices((hostContext, services) => {
                    services.AddLazyResolution();
                    // Configure the shutdown timeout to 30s
                    services.Configure<HostOptions>(
                        opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));
                    services.AddHostedService<Service>();
                    services.AddSingleton<IConfigFactory>(x => new ConfigFactory("appsettings.json"));
                    services.AddSingleton<IConnectionManager, ConnectionManager>();
                    services.AddSingleton<RemoteFunctions, RemoteFunctions>();
                }).ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddLog4Net("log4net.config");
                    logging.SetMinimumLevel(LogLevel.Debug);
                });
    }
}
