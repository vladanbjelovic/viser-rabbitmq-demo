

using Microsoft.Extensions.Logging;
using NC3.Comm.Core.Layer.Config;
using NC3.Comm.Core.Layer.Interfaces;
using NC3.Comm.Core.Layer;
using Microservice_B.Remote;
using System.Threading.Tasks;
using System.Collections.Generic;
using NC3.Comm.Core.Layer.Helpers;
using NC3.Comm.Msvc.Events.Models;
using NC3.Comm.Msvc.Events;
using NC3.Comm.Core.Layer.RabbitMQ;
using System.Linq;
using NC3.Comm.Core.Layer.Extensions;

namespace Microservice_B.Connection
{
    public class ConnectionManager : IMessageReceiver, IConnectionManager
    {
        private readonly ILogger<ConnectionManager> _logger;
        private readonly IConfigFactory _commConfigFactory;
        private readonly RemoteFunctions _remoteFunctions;
        private readonly CommLayer _commLayer;
        private IMessageSender _messageSender;

        public ConnectionManager(ILogger<ConnectionManager> logger, IConfigFactory configFactory, RemoteFunctions remoteFunctions)
        {
            _logger = logger;

            _commConfigFactory = configFactory;
            _commLayer = new CommLayer(_commConfigFactory, this);
            _remoteFunctions = remoteFunctions;
        }

        public async Task<string> CallFunction(string targetChannelName, string targetRoutingKey, Dictionary<string, string> message)
        {
            if (_commConfigFactory.RabbitMQ.ReceiveChannelsList == null ||
                _commConfigFactory.RabbitMQ.ReceiveChannelsList.Count == 0)
            {
                _logger.LogError("No RabbitMQ receive channels detected.");
                return null;
            }

            var options = new RPCCallOptions
            {
                ChannelName = targetChannelName,
                Address = targetRoutingKey, // * = Take routing keys from send channel settings
                ReturnExchange = _commConfigFactory.RabbitMQ.ReceiveChannelsList.First().ExchangeName,
                ReturnAddress = _commConfigFactory.RabbitMQ.ReceiveChannelsList.First().RoutingKey
            };

            return await _messageSender.Call(
                message,
                options);
        }

        public Task<RPCResponse<object>> ProcessFunction(string method, string[] args, Dictionary<string, string> principal)
        {
            return FunctionHelper.ProcessFunction(_remoteFunctions, method, args, principal, testMode: false);
        }

        public void ProcessMessage(string channelName, string address, string message)
        {
            Dictionary<string, string> msg = message.ToMessage();

            if (msg == null)
            {
                _logger.LogError($"Invalid message!  message={message}");
                return;
            }

            if (!msg.ContainsKey("Body"))
            {
                _logger.LogError("Unrecognized message format (doesn't contain Body)");
                return;
            }

            _logger.LogInformation($"Received message: {msg["Body"]}");
        }

        public void ProcessReturnedMessage(string channelName, string address, string message)
        {
            // Not Implemented
        }

        public void SendEvent<TResource>(MsvcEvent<TResource> e)
        {
            _messageSender.SendEvent(e);
        }

        public void Start()
        {
            _messageSender = _commLayer.Init();
            if (_messageSender == null)
            {
                _logger.LogError("Configuration is not enabled. Please check config file.");
                return;
            }
        }

        public void Stop()
        {
            _commLayer.Stop();
            _messageSender = null;
        }
    }
}
