using NetworkModule.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkModule
{
    /// <summary>
    /// Manages Channels
    /// </summary>
    public class WebSocketChannelManager 
    {
        public static readonly ConcurrentDictionary<string, IChannel> _channels = new ConcurrentDictionary<string, IChannel>();
        //TODO: this code will be changed depends on how we triggers RemoteTransaction and how we specifies charging point
        public event EventHandler? ChannelAccepted;
        public event EventHandler? ChannelClosed;

        public JsonMessageDispatcher MessageDispatcher { get; set; }

        public WebSocketChannelManager(JsonMessageDispatcher messageDispatcher)
        {
            MessageDispatcher = messageDispatcher;
        }

        //TODO:Hey future Dan, plese implement the factory patten for channel 
        //public MessageDispatcher<TSerializedDataType> MessageDispatcher { get; set; }

        //public WebSocketChannelManager(MessageDispatcher<TSerializedDataType> messageDispatcher)
        //{
        //    MessageDispatcher = messageDispatcher;
        //}

        public int ChannelCount => _channels.Count;

        /// <summary>
        /// A method that request a handshake(WebSocket Connection) 
        /// </summary>
        /// <param name="uri">Which Indicate the address of charging point</param>
        /// <returns></returns>
        public async Task RequestConnectionAsync(string uri)
        {
            var clientSocket = new ClientWebSocket();
            try
            {
                await clientSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occured on connecting {uri}:\n RequestConnection: {ex.Message}");
                return;
            }
            AddConnection(clientSocket,uri);
        }

        /// <summary>
        /// Tries adding new channel into channels' concurrent dictionary 
        /// </summary>
        /// <param name="webSocket">This goes inside a new channel</param>
        public void AddConnection(WebSocket webSocket,string uri)
        {
            //TODO: IF XML way is added, then implement a factory pattern to make both XML channel or Json channel.
            var channel = new JsonChannel();
            _channels.TryAdd(uri, channel);
            channel.Closed += (s, e) => {
                _channels.TryRemove(uri, out var _);
                ChannelClosed?.Invoke(this, EventArgs.Empty);
            };

            channel.Attach(webSocket);
            MessageDispatcher.Bind(channel);
            ChannelAccepted?.Invoke(this, EventArgs.Empty);
        }

    }
}
