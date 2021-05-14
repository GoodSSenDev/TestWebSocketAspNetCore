using NetworkModule.Json;
using NetworkModule.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace TestWebApplication
{
    public static class MessageHandler
    {
        //Handler on the 'Server' side of the system
        [JsonRoute("$.action", "HeartBeat")]
        public static Task<HeartBeatResponseMessage> HandleMessage(HeartBeatRequestMessage request)
        {
            Received(request);
            Debug.WriteLine("HeartBeatRequestMessage ");
            var response = new HeartBeatResponseMessage
            {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success }
            };
            Sending(response);
            return Task.FromResult(response);
        }


        static void Received<T>(T msg) where T : Message
            => Debug.WriteLine($"Received {typeof(T).Name}: Action[ {msg.Action} ], Id[ {msg.Id} ]");

        static void Sending<T>(T msg) where T : Message
            => Debug.WriteLine($"Sending {typeof(T).Name}: Action[ {msg.Action} ], Id[ {msg.Id} ]");
    }
}

