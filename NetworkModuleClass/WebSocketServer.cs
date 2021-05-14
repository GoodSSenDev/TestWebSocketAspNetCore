using NetworkModule.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkModule
{
    //TODO: Integrate this function to the Channel, and use the Channel class instead
    /// <summary>
    /// Represents a server inside a OCPP simulator, which mocks the server inside charging point.
    /// </summary>
    public class WebSocketServer:IAsyncDisposable,IDisposable
    {

        //one webSocket because
        WebSocket _webSocket;
        bool _isServerStarting = false;
        bool _isDisposed = false;
        bool _isClosed = false;
        HttpListener _httpListener;

        /// <summary>
        /// Receives handshake Request From CMS and wait and 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<bool> StartGetWebSocket(string uri)
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
                        Debug.WriteLine("hellow");
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
        /// Sends a message using the WebSocket
        /// </summary>
        /// <typeparam name="TMessage">Message Type which inherit mesage</typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync<TMessage>(TMessage message)//TODO: think about limit the message generic
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(Encode<TMessage>(message))
                , WebSocketMessageType.Text,
                true, CancellationToken.None).ConfigureAwait(false);
        }


        protected JObject Decode(byte[] message)
            => JsonSerialization.Deserialize(Encoding.UTF8.GetString(message));

        protected byte[] Encode<TMessage>(TMessage message)
            => Encoding.UTF8.GetBytes(JsonSerialization.Serialize(message).ToString());

        #region Dispose
        //Finalizer
        ~WebSocketServer() => Dispose(false);

        /// <summary>
        /// Close the Websocket and httpListhener
        /// </summary>
        public async Task CloseAsync()
        {
            if (!_isClosed)
            {
                _isClosed = true;
                if (_webSocket?.State == WebSocketState.Open)
                {
                    //this sends asynchronous message to a client to close the connection. 
                    //If the server initiates the request to close the connection, the method returns without waiting for a response.
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Socket In CMS is closed",
                       CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        //Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);//avoid calling finalizer
        }

        /// <summary>
        /// Disposeing 
        /// </summary>
        /// <param name="isDisposing">True: for Disposing both unmanaged and managed resources,
        /// False: for disposing only unmanged resources</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;

            if (isDisposing)
            {
                CloseAsync().GetAwaiter().GetResult();
                _webSocket?.Dispose();
                _httpListener.Close();
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Dispose Async
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);//avoid calling finalizer
        }

        /// <summary>
        /// Close Async
        /// </summary>
        /// <returns></returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_isDisposed)
                return;
            await CloseAsync();
            _webSocket.Dispose();
            _httpListener.Close();
        }

        #endregion

    }
}
