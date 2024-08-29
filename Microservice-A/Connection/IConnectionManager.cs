using System.Collections.Generic;
using System.Threading.Tasks;
using NC3.Comm.Core.Layer.Helpers;

namespace Microservice_A.Connection
{
    public interface IConnectionManager
    {
        // Task<string> CallFunction(string targetChannelName, string targetRoutingKey, Dictionary<string, string> message);
        Task<RPCResponse<object>> ProcessFunction(string method, string[] args, Dictionary<string, string> principal);
        void ProcessMessage(string channelName, string address, string message);
        void SendMessage(string message);
        Task<string> CallMicroserviceBFunction(Dictionary<string, string> message);
        void Start();
        void Stop();
    }
}