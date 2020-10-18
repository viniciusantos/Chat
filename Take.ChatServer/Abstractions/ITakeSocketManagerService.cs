using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Take.ChatServer.Services;

namespace Take.ChatServer.Abstractions
{
    /// <summary>
    ///   ITakeSocketManagerService
    /// </summary>
    public interface ITakeSocketManagerService
    {
        /// <summary>Gets the socket by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        WebSocket GetSocketById(string id);

        /// <summary>Gets all.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        ConcurrentDictionary<string, WebSocket> GetAll();

        /// <summary>Determines whether [contains socket identifier] [the specified socket identifier].</summary>
        /// <param name="socketId">The socket identifier.</param>
        /// <returns>
        ///   <c>true</c> if [contains socket identifier] [the specified socket identifier]; otherwise, <c>false</c>.</returns>
        bool ContainsSocketId(string socketId);

        /// <summary>Adds the socket.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="socketId">The socket identifier.</param>
        /// <param name="executeAfterAdd">The execute after add.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        string AddSocket(WebSocket socket, string socketId, Action<ITakeSocketManagerService> executeAfterAdd);

        /// <summary>Removes the socket.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="executeAfterRemove">The execute after remove.</param>
        void RemoveSocket(string id, Action<ITakeSocketManagerService> executeAfterRemove);

        /// <summary>Sends the message asynchronous.</summary>
        /// <param name="socketId">The socket identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task SendMessageAsync(string socketId, dynamic message);

        /// <summary>Sends the message to all.</summary>
        /// <param name="message">The message.</param>
        void SendMessageToAll(dynamic message);
    }
}
