using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkModule
{
    /// <summary>
    /// Represents a channel that sends and recieves messages using a WebSocket.
    /// </summary>
    /// <typeparam name="TSerializedDataType"></typeparam>
    public abstract class Channel<TSerializedDataType> : IAsyncDisposable, IDisposable, IChannel
    {
        protected bool _isDisposed = false;
        protected bool _isClosed = false;

        protected readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected WebSocket _webSocket;
        Func<TSerializedDataType, Task> _messageCallback;
        Task _receiveLoopTask;

        public event EventHandler Closed;

        public Guid Id { get; } = Guid.NewGuid();
        public DateTime LastSent { get; protected set; }
        public DateTime LastReceived { get; protected set; }

        /// <summary>
        /// Attaches a webSocket into this channel and start the receive loop
        /// </summary>
        /// <param name="webSocket"></param>
        public void Attach(WebSocket webSocket)
        {
            _webSocket = webSocket;
            _receiveLoopTask = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Registers a delegate that gets the TSerializedDataType as parameter and is invoked when a message received
        /// </summary>
        /// <param name="callbackHandler">delegate that gets the TSerializedDataType as parameter and is invoked when a message received</param>
        public void OnMessage(Func<TSerializedDataType, Task> callbackHandler)
            => _messageCallback = callbackHandler;

        /// <summary>
        /// Close this channel including attached socket 
        /// </summary>
        public async Task CloseAsync()
        {
            if(!_isClosed)
            {
                _isClosed = true;
                if (_webSocket?.State == WebSocketState.Open)
                {
                    //this sends asynchronous message to a client to close the connection. 
                    //If the server initiates the request to close the connection, the method returns without waiting for a response.
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Socket In CMS is closed",
                       CancellationToken.None).ConfigureAwait(false);
                }
                _cancellationTokenSource.Cancel();
                Closed?.Invoke(this, EventArgs.Empty);
            }
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
                ,WebSocketMessageType.Text,
                true,_cancellationTokenSource.Token).ConfigureAwait(false);
            LastSent = DateTime.UtcNow;
        }

        /// <summary>
        /// Receive loop and pass a message to controller, 
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual async Task ReceiveLoop()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var receiveBuffer = new byte[1024 * 4];
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _cancellationTokenSource.Token);
                    if(result.MessageType == WebSocketMessageType.Text)
                    {
                        await _messageCallback(Decode(receiveBuffer)).ConfigureAwait(false);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {

                    }
                    LastReceived = DateTime.UtcNow;

                }
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
        }

        #region Dispose

        //Finalizer
        ~Channel() => Dispose(false);

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
        }

        #endregion

        /// <summary>
        /// Decodes bytes to a message
        /// </summary>
        /// <param name="message">Message in byte array</param>
        /// <returns>Message</returns>
        protected abstract TSerializedDataType Decode(byte[] message);

        /// <summary>
        /// Encodes message
        /// </summary>
        /// <typeparam name="TMessage">Message</typeparam>
        /// <param name="message">Message</param>
        /// <returns>Encoded message in byte array</returns>
        protected abstract byte[] Encode<TMessage>(TMessage message);
    }
}
