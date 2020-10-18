using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Take.ChatServer.Abstractions;
using Take.ChatServer.Dto;

namespace Take.ChatServer.Services
{
    /// <summary>
    ///   TakeSocketManagerService
    /// </summary>
    public class TakeSocketManagerService : ITakeSocketManagerService
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        /// <summary>Gets the socket by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public WebSocket GetSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        /// <summary>Gets all.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        /// <summary>Determines whether [contains socket identifier] [the specified socket identifier].</summary>
        /// <param name="socketId">The socket identifier.</param>
        /// <returns>
        ///   <c>true</c> if [contains socket identifier] [the specified socket identifier]; otherwise, <c>false</c>.</returns>
        public bool ContainsSocketId(string socketId)
        {
            return _sockets.ContainsKey(socketId);
        }

        /// <summary>Adds the socket.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="socketId">The socket identifier.</param>
        /// <param name="executeAfterAdd">The execute after add.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public string AddSocket(WebSocket socket, string socketId, Action<ITakeSocketManagerService> executeAfterAdd)
        {
            _sockets.TryAdd(socketId, socket);

            executeAfterAdd(this);
            
            ReceiveMessage(socket, socketId);
            
            return socketId;
        }

        /// <summary>Removes the socket.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="executeAfterRemove">The execute after remove.</param>
        public void RemoveSocket(string id, Action<ITakeSocketManagerService> executeAfterRemove)
        {
            WebSocket socket;
            _sockets.TryRemove(id, out socket);

            executeAfterRemove(this);
        }

        /// <summary>Sends the message asynchronous.</summary>
        /// <param name="socketId">The socket identifier.</param>
        /// <param name="message">The message.</param>
        public async Task SendMessageAsync(string socketId, dynamic message)
        {
            var socket = this.GetSocketById(socketId);

            if (socket.State != WebSocketState.Open)
                return;

            var serializedMessage = JsonConvert.SerializeObject(message);

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(serializedMessage),
                                                                    offset: 0,
                                                                    count: serializedMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
        }

        /// <summary>Sends the message to all.</summary>
        /// <param name="message">The message.</param>
        public void SendMessageToAll(dynamic message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);

            var sockets = this.GetAll().ToList();

            sockets.ForEach(async socket => {

                if (socket.Value.State == WebSocketState.Open)
                {
                    await socket.Value.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(serializedMessage),
                                                                    offset: 0,
                                                                    count: serializedMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
                }
            });
        }

        /// <summary>Receives the message from client.</summary>
        /// <param name="webSocket">The web socket.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private async Task<string> ReceiveMessageFromClient(WebSocket webSocket)
        {
            if (webSocket != null)
            {
                var buffer = new byte[4096];
                var arraySegment = new ArraySegment<byte>(buffer);
                await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
                return Encoding.UTF8.GetString(arraySegment.Array, 0, arraySegment.Count);
            }
            return null;
        }

        /// <summary>Receives the message.</summary>
        /// <param name="webSocket">The web socket.</param>
        /// <param name="username">The username.</param>
        private void ReceiveMessage(WebSocket webSocket, string username)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                //Start to receiving message from a websocket connection
                ReceiveMessageFromClient(webSocket).GetAwaiter().GetResult();
            }
        }
    }
}
