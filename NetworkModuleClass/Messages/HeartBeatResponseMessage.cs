#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace NetworkModule.Messages
{
    public class HeartBeatResponseMessage : Message
    {
        [JsonProperty( "result" )]
        public Result? Result { get; set; }

        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }

        public HeartBeatResponseMessage( )
        {
            Type = MessageType.Response;
            Action = "HeartBeat";
        }
    }
}
