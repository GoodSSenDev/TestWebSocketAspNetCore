using NetworkModule.Json;
using NetworkModule.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientBll
    {
        //Httpclient is reusable no need dispose 
        static HttpClient _client = new HttpClient();
        JsonClientChannel _clientChannel;


        public async Task StartConnection()
        {
            _clientChannel = new JsonClientChannel();
            var clientServerUrl = "localhost:3314/";
            var _ = Task.Run(async () =>
            {
                await _clientChannel.StartGetWebSocket(clientServerUrl);
            });
            await TriggerWebSocketRequest("https://localhost:44313/api/Trigger/startRemoteTransaction/", clientServerUrl);
        }

        public async Task<bool> TriggerWebSocketRequest(string destinationUrl, string url)
        {
            var serverUrl = "ws://" + url;
            var stringContent = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(serverUrl),
                System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(destinationUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var resContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"The response is : {resContent}");
                return true;
            }
            else return false;
        }

        public async Task SendHeartBeatRequest()
        {
            HeartBeatRequestMessage heartBeatRequestMessage = new HeartBeatRequestMessage
            {
                Id = "CLIENT HeartBeat MESSAGE RECEIVED - client is still alive",
                POSData = new POSData { Id = "POS001" }
            };
            await _clientChannel.SendAsync<HeartBeatRequestMessage>(heartBeatRequestMessage);
        }

        public async Task SendHeartBeartRequestAndReceiveAsync()
        {
            HeartBeatRequestMessage heartBeatRequestMessage = new HeartBeatRequestMessage
            {
                Id = "CLIENT HeartBeat MESSAGE RECEIVED - client is still alive",
                POSData = new POSData { Id = "POS001" }
            };
            var response = await _clientChannel.SendAndReceiveAsync<HeartBeatRequestMessage,HeartBeatResponseMessage>(heartBeatRequestMessage);

            Debug.WriteLine(response.Id);
        }
    }
}
