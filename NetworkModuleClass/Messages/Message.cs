#nullable   enable

using System.Xml.Serialization;

using Newtonsoft.Json;

namespace NetworkModule.Messages
{
    /*
      *  <Message type='Request' action='HeartBeat' id='0001'>
      *    <POS id='POS_001'/>
      *  </Message>
      * 
      * <Message type='Response' action='HeartBeat' id='0001'>
      *   <POS id='POS_001'/> 
      *   <Result status='Success'/>
      * </Message>
      * 
      */

    public enum MessageType
    {
        Request, 
        Response
    }

    public enum Status
    {
        Success,
        Failure
    }


    public abstract class Message 
    {
        [JsonProperty( "id" )]
        public string? Id { get; set; }

        [JsonProperty( "type" )]
        public MessageType Type { get; set; }

        [JsonProperty( "action" )]
        public string? Action { get; set; }
    }

    public class POSData
    {
        [JsonProperty( "id" )]
        public string? Id { get; set; }
    }

    public class Result
    {
        [JsonProperty( "status" )]
        public Status Status { get; set; }
    }
}
