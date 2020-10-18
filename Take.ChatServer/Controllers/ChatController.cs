using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Take.ChatServer.Abstractions;
using Take.ChatServer.Dto;

namespace Take.ChatServer.Controllers
{
    /// <summary>
    ///   ChatController
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBaseChatServer
    {
        private readonly IChatDataService _chatDataService;

        /// <summary>Initializes a new instance of the <see cref="ChatController" /> class.</summary>
        /// <param name="chatDataService">The chat data service.</param>
        public ChatController(IChatDataService chatDataService)
        {
            _chatDataService = chatDataService;
        }

        /// <summary>Connects the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Connect(string username)
        {
            var result = await _chatDataService.Connect(username);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadBusinessLogicRequest(result.ErrorMessage);
            }
        }

        /// <summary>Disconnects the specified username.</summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet]
        public IActionResult Disconnect(string username)
        {
            var result = _chatDataService.Disconnect(username);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadBusinessLogicRequest(result.ErrorMessage);
            }
        }

        /// <summary>Sends the message.</summary>
        /// <param name="dto">The dto.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SendMessage(ConversationDto dto)
        {
            var result = await _chatDataService.SendMessage(dto);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadBusinessLogicRequest(result.ErrorMessage);
            }
        }
    }
}
