
using Microservice_A.Connection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice_A
{
    public class InputTestService
    {
#pragma warning disable CS4014
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<InputTestService> _logger;

        public InputTestService(IConnectionManager connectionManager, ILogger<InputTestService> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }


        public async Task ListenForInputAsync(CancellationToken token)
        {
            Task.Factory.StartNew(() => ListenForInputAsync(),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void ListenForInputAsync()
        {
            Task.Delay(5000).Wait();

            var input = "";
            while (input != "exit")
            {
                Console.Write($"Input commad: ");
                input = Console.ReadLine();
                Console.WriteLine($"You entered: {input}");
                ParseInput(input);

                Task.Delay(5000).Wait();
            }
        }

        private void ParseInput(string input)
        {
            var inputs = input.Split(' ', 2);
            var command = inputs[0];

            if (command == "send")
            {
                if (inputs.Length > 1)
                {
                    var message = inputs[1];
                    _connectionManager.SendMessage(message);
                }
            }

            if (command == "calc")
            {
                var args = inputs[1].Split(' ');

                if (args.Length < 3) return;

                var msg = new Dictionary<string, string> { 
                    { "function", args[0] }
                };

                for (int i = 1; i < args.Length; ++i)
                    msg.Add("arg" + i, JsonConvert.SerializeObject(args[i]));

                var responseJson = _connectionManager.CallMicroserviceBFunction(msg).Result;

                try
                {
                    var resultJson = JObject.Parse(responseJson)["result"].ToString();
                    resultJson = resultJson == "True" || resultJson == "False" ? resultJson.ToLower() : resultJson;
                    var result = JsonConvert.DeserializeObject<double>(resultJson);

                    Console.WriteLine($"Result of calculation is: {result}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to get RPC response from Microservice-B. Json={responseJson} Exception={ex.Message} Trace={ex.StackTrace}");
                }
            }
        }
#pragma warning restore CS4014
    }
}
