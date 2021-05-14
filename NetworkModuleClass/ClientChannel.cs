#nullable enable

using NetworkModule.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModule
{
    public abstract class ClientChannel<TSerializedDataType> : Channel<TSerializedDataType>
    {
        protected HttpListener? _httpListener;
        protected bool _isServerStarting = false;

        /// <summary>
        /// Receives handshake Request From CMS and wait and 
        /// </summary>
        /// <param name="uri">the Server URI </param>
        /// <returns>false if Server is already started(already got websocket)</returns>
        public virtual async Task<bool> StartGetWebSocket(string uri)
        {
            if (!_isServerStarting)
            {
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add("http://"+uri);
                _httpListener.Start();

                while (true)
                {
                    var httpListhenerContext = await _httpListener.GetContextAsync();
                    if (httpListhenerContext.Request.IsWebSocketRequest)
                    {
                        var context = await httpListhenerContext.AcceptWebSocketAsync(null);
                        Debug.WriteLine("Client's server received a websocket handshake");
                        _webSocket = context.WebSocket;
                        Debug.WriteLine($"Connected to {httpListhenerContext.Request.RemoteEndPoint.Address}");
                        _isServerStarting = true;
                        return true;
                    }
                    else
                    {
                        httpListhenerContext.Response.StatusCode = 400;
                        httpListhenerContext.Response.Close();
                        Debug.WriteLine("Connection fail due to: It is not Websocket request");
                    }
                }
            }
            else return false;
        }

        /// <summary>
        /// Sends a message using the WebSocket asnd await it
        /// </summary>
        /// <typeparam name="TMessage">Message Type which inherit mesage</typeparam>
        /// <typeparam name="TMessageReceived">Message Type which received</typeparam>
        /// <param name="message"></param>
        /// <returns>null if receive fail or it is close type</returns>
        /// TODO: write process logic on websocket Message Type.
        public async Task<TMessageReceived?> SendAndReceiveAsync<TMessage, TMessageReceived>(TMessage message) 
            where TMessageReceived : class
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(Encode<TMessage>(message))
                , WebSocketMessageType.Text,
                true, _cancellationTokenSource.Token).ConfigureAwait(false);
            try
            {
                var receiveBuffer = new byte[1024 * 4];
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _cancellationTokenSource.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    return Deserialize<TMessageReceived>(Decode(receiveBuffer));
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    return null;
                }
                LastReceived = DateTime.UtcNow;
            }
            catch (System.IO.IOException)
            {
                await CloseAsync();
            }
            catch (Exception _e)
            {
                Console.WriteLine($"Channel::ReceiveLoop || Exception => {_e}");
                await CloseAsync();
            }
            return null;
        }

        /// <summary>
        /// Deserialize the serialized Data.
        /// </summary>
        /// <typeparam name="TMessageReceived"></typeparam>
        /// <param name="message">Serialized Data</param>
        /// <returns>TMessageReceived type message instance </returns>
        protected abstract TMessageReceived Deserialize<TMessageReceived>(TSerializedDataType message);

        #region Dispose

        //Finalizer
        ~ClientChannel() => Dispose(false);

        /// <summary>
        /// Disposeing 
        /// </summary>
        /// <param name="isDisposing">True: for Disposing both unmanaged and managed resources,
        /// False: for disposing only unmanged resources</param>
        protected override void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;

            if (isDisposing)
            {
                CloseAsync().GetAwaiter().GetResult();
                _webSocket?.Dispose();
                _httpListener?.Close();
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Close Async
        /// </summary>
        /// <returns></returns>
        protected override async ValueTask DisposeAsyncCore()
        {
            if (_isDisposed)
                return;
            await CloseAsync();
            _webSocket.Dispose();
            _httpListener?.Close();
        }
        #endregion
    }
}
