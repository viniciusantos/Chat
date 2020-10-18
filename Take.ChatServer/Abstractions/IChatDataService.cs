using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Take.ChatServer.Dto;

namespace Take.ChatServer.Abstractions
{
    public interface IChatDataService
    {

        /// <summary>Connects the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<ServiceDataResponse> Connect(string username);


        /// <summary>Disconnects the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        ServiceDataResponse Disconnect(string username);


        /// <summary>Sends the message.</summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<ServiceDataResponse> SendMessage(ConversationDto message);
    }
}
