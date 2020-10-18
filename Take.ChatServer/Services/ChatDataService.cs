using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Take.ChatServer.Abstractions;
using Take.ChatServer.Dto;

namespace Take.ChatServer.Services
{

    /// <summary>
    ///   ChatDataService
    /// </summary>
    public class ChatDataService : IChatDataService
    {
        private readonly ITakeSocketManagerService _socketManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>Initializes a new instance of the <see cref="ChatDataService" /> class.</summary>
        /// <param name="socketManager">The socket manager.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public ChatDataService(ITakeSocketManagerService socketManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _socketManager = socketManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>Connects the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<ServiceDataResponse> Connect(string username)
        {
            if (String.IsNullOrWhiteSpace(username))
            {
                return new ServiceDataResponse("Username cannot be empty.");
            }

            username = username.Trim();
            var isSocketRequest = _httpContextAccessor.HttpContext.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                if (_socketManager.GetSocketById(username) != null)
                {
                    return new ServiceDataResponse("This username is already being used.");
                }

                _socketManager.AddSocket(await _httpContextAccessor.HttpContext.WebSockets.AcceptWebSocketAsync(),
                    username,
                    (manager) =>
                    {
                        manager.SendMessageToAll(new
                        {
                            action = "userConnectedUpdate",
                            allUsersConnected = manager.GetAll().Select(a => a.Key).ToList()
                        });
                    });
            }
            else
            {
                return new ServiceDataResponse("This is not a websocket request. Invalid request.");
            }

            return new ServiceDataResponse();
        }

        /// <summary>Disconnects the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public ServiceDataResponse Disconnect(string username)
        {
            if (String.IsNullOrWhiteSpace(username))
                return new ServiceDataResponse("Username cannot be empty.");

            if (!_socketManager.ContainsSocketId(username))
                return new ServiceDataResponse("This username is not available.");

            _socketManager.RemoveSocket(username, (manager) =>
            {
                manager.SendMessageToAll(new
                {
                    action = "userConnectedUpdate",
                    allUsersConnected = manager.GetAll().Select(a => a.Key).ToList()
                });
            });

            return new ServiceDataResponse();
        }

        /// <summary>Sends the message.</summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<ServiceDataResponse> SendMessage(ConversationDto message)
        {
            //TODO: A fluent validation framework should be implemented here.

            if (String.IsNullOrWhiteSpace(message.Message))
                return new ServiceDataResponse("A message cannot be empty.");

            if (String.IsNullOrWhiteSpace(message.From))
                return new ServiceDataResponse("A emmiter cannot be empty.");

            if (message.isPublicMessage)
            {
                _socketManager.SendMessageToAll(
                        new
                        {
                            from = message.From,
                            message = message.Message,
                            action = "publicMessage"
                        }
                );
            }
            else
            {
                if (String.IsNullOrWhiteSpace(message.To))
                    return new ServiceDataResponse("A receiver cannot be empty.");

                await _socketManager.SendMessageAsync(
                    message.To,
                        new
                        {
                            from = message.From,
                            message = message.Message,
                            action = "privateMessage"
                        }
                );
            }

            return new ServiceDataResponse();
        }
    }
}
