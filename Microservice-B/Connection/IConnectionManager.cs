using System.Collections.Generic;
using System.Threading.Tasks;
using NC3.Comm.Core.Layer.Helpers;
using NC3.Comm.Msvc.Events.Models;

namespace Microservice_B.Connection
{
    public interface IConnectionManager
    {
        Task<string> CallFunction(string targetChannelName, string targetRoutingKey, Dictionary<string, string> message);
        Task<RPCResponse<object>> ProcessFunction(string method, string[] args, Dictionary<string, string> principal);
        void ProcessMessage(string channelName, string address, string message);
        void ProcessReturnedMessage(string channelName, string address, string message);
        void SendEvent<TResource>(MsvcEvent<TResource> e);
        void Start();
        void Stop();
    }
}