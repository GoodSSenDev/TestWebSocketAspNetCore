#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace NetworkModule.Messages
{
    public class HeartBeatRequestMessage : Message
    {
        [JsonProperty( "posData" )]
        public POSData? POSData { get; set; }

        public HeartBeatRequestMessage( )
        {
            Type = MessageType.Request;
            Action = "HeartBeat";
        }
    }
}
