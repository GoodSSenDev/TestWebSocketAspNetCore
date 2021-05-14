using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace NetworkModule
{
    /// <summary>
    /// Represents a minimum requirement for implementing a Channel
    /// </summary>
    public interface IChannel
    {
        Guid Id { get; }

        DateTime LastSent { get; }
        DateTime LastReceived { get; }

        event EventHandler Closed;

        void Attach(WebSocket socket);
        Task CloseAsync();
        void Dispose();
        Task SendAsync<TMessage>(TMessage message);
    }
}
