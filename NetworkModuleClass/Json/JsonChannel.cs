using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkModule.Json
{
    /// <summary>
    /// Represents a channel using JSON as encoded message type
    /// </summary>
    public class JsonChannel : Channel<JObject>
    {
        protected override JObject Decode(byte[] message)
            => JsonSerialization.Deserialize(Encoding.UTF8.GetString(message));

        protected override byte[] Encode<TMessage>(TMessage message)
            => Encoding.UTF8.GetBytes(JsonSerialization.Serialize(message).ToString());
    }
}
