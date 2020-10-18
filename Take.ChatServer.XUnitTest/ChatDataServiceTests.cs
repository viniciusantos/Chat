using Xunit;
using Moq;
using Take.ChatServer.Abstractions;
using Take.ChatServer.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Shouldly;
using System.Net.WebSockets;
using System;
using Take.ChatServer.Dto;

namespace Take.ChatServer.Tests
{
    public class ChatDataServiceTests
    {
        private Mock<ITakeSocketManagerService> _socketManagerMock;
        private Mock<IHttpContextAccessor> _httpContextMock;
        private IChatDataService _chatService;

        public ChatDataServiceTests()
        {
            _socketManagerMock = new Mock<ITakeSocketManagerService>(MockBehavior.Strict);
            _httpContextMock = new Mock<IHttpContextAccessor>(MockBehavior.Strict);

            _chatService = new ChatDataService(_socketManagerMock.Object,
                _httpContextMock.Object);
        }

        [Fact]
        public async Task Connect_WithUsernameCannotBeEmpty_ReturnsServiceDataResponse()
        {
            //arrange
            var username = string.Empty;

            //act
            var result = await _chatService.Connect(username);

            //assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("Username cannot be empty.");
        }

        [Fact]
        public async Task Connect_WithNotAWebSocketRequest_ReturnsServiceDataResponse()
        {
            //arrange
            var username = "Some username";
            
            _httpContextMock.SetupGet(a => a.HttpContext.WebSockets.IsWebSocketRequest)
                .Returns(false)
                .Verifiable();

            //act
            var result = await _chatService.Connect(username);

            //assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("This is not a websocket request. Invalid request.");
            
            _httpContextMock.Verify();
        }

        [Fact]
        public async Task Connect_WithUsernameAlreadyInUse_ReturnsServiceDataResponse()
        {
            //arrange
            var username = "Some username";
            WebSocket webSocket = Mock.Of<WebSocket>();

            _socketManagerMock.Setup(a => a.GetSocketById(username))
                .Returns(webSocket)
                .Verifiable();

            _httpContextMock.SetupGet(a => a.HttpContext.WebSockets.IsWebSocketRequest)
                .Returns(true)
                .Verifiable();

            //act
            var result = await _chatService.Connect(username);

            //assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("This username is already being used.");

            _socketManagerMock.Verify();
            _httpContextMock.Verify();
        }

        [Fact]
        public async Task Connect_WithRequestOk_ReturnsServiceDataResponse()
        {
            //arrange
            var username = "Some username";
            WebSocket webSocket = null;
            var webSocketAccepted = Mock.Of<WebSocket>();

            _socketManagerMock.Setup(a => a.GetSocketById(username))
                .Returns(webSocket)
                .Verifiable();

            _httpContextMock.SetupGet(a => a.HttpContext.WebSockets.IsWebSocketRequest)
                .Returns(true)
                .Verifiable();

            _httpContextMock.Setup(a => a.HttpContext.WebSockets.AcceptWebSocketAsync())
                .ReturnsAsync(webSocketAccepted)
                .Verifiable();

            _socketManagerMock.Setup(a => a.AddSocket(webSocketAccepted, username, It.IsAny<Action<ITakeSocketManagerService>>()))
                .Returns("OK")
                .Verifiable();

            //act
            var result = await _chatService.Connect(username);

            //assert
            result.Success.ShouldBeTrue();

            _socketManagerMock.Verify();
            _httpContextMock.Verify();
        }

        [Fact]
        public void Disconnect_WithUsernameCannotBeEmpty_ReturnsServiceDataResponse()
        {
            //arrange
            var username = String.Empty;

            //act
            var result = _chatService.Disconnect(username);

            //assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("Username cannot be empty.");
        }


        [Fact]
        public void Disconnect_WithUsernameDoesntExists_ReturnsServiceDataResponse()
        {
            //arrange
            var username = "Someone";

            _socketManagerMock.Setup(a => a.ContainsSocketId(username))
                .Returns(false)
                .Verifiable();

            //act
            var result = _chatService.Disconnect(username);

            //assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("This username is not available.");
        }

        [Fact]
        public void Disconnect_WithUserDisconnected_ReturnsServiceDataResponse()
        {
            //arrange
            var username = "Someone";

            _socketManagerMock.Setup(a => a.ContainsSocketId(username))
                .Returns(true)
                .Verifiable();

            _socketManagerMock.Setup(a => a.RemoveSocket(username, It.IsAny<Action<ITakeSocketManagerService>>()))
                .Verifiable();

            //act
            var result = _chatService.Disconnect(username);

            //assert
            result.Success.ShouldBeTrue();

            _socketManagerMock.Verify();
        }


        public static TheoryData<ConversationDto, string> SendMessage_WithPrivateMessageInvalidParameters_ReturnsServiceDataResponse_Theory =
            new TheoryData<ConversationDto, string>
            {
                {
                    new ConversationDto
                    {
                        From = "Someone",
                        isPublicMessage = false,

                    }, "A message cannot be empty."
                },
                {
                    new ConversationDto
                    {
                        Message = "Some text",
                        isPublicMessage = true,

                    }, "A emmiter cannot be empty."
                },
                {
                    new ConversationDto
                    {
                        Message = "Some text",
                        isPublicMessage = false,
                        From = "Someoneelse"

                    }, "A receiver cannot be empty."
                }
            };

        [Theory]
        [MemberData(nameof(SendMessage_WithPrivateMessageInvalidParameters_ReturnsServiceDataResponse_Theory))]
        public async Task SendMessage_WithPrivateMessageInvalidParameters_ReturnsServiceDataResponse(ConversationDto dto,
            string validationMessage)
        {
            //act
            var result = await _chatService.SendMessage(dto);

            //assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain(validationMessage);
        }

        [Fact]
        public async Task SendMessage_WithPublicMessageSent_ReturnsServiceDataResponse()
        {
            //arrange
            var conversationDto = new ConversationDto
            {
                From = "Someone",
                isPublicMessage = true,
                Message = "Some text."
            };

            _socketManagerMock.Setup(a => a.SendMessageToAll(It.Is<object>(d =>
                d.GetPropertyValue<string>("from") == conversationDto.From
                && d.GetPropertyValue<string>("message") == conversationDto.Message
                && d.GetPropertyValue<string>("action") == "publicMessage")))
                .Verifiable();

            //act
            var result = await _chatService.SendMessage(conversationDto);

            //assert
            result.Success.ShouldBeTrue();

            _socketManagerMock.Verify();
        }

        [Fact]
        public async Task SendMessage_WithPrivateMessageSent_ReturnsServiceDataResponse()
        {
            //arrange
            var conversationDto = new ConversationDto
            {
                From = "Someone",
                isPublicMessage = false,
                Message = "Some text.",
                To = "Someone else"
            };

            _socketManagerMock.Setup(a => a.SendMessageAsync(conversationDto.To, It.Is<object>(d =>
                d.GetPropertyValue<string>("from") == conversationDto.From
                && d.GetPropertyValue<string>("message") == conversationDto.Message
                && d.GetPropertyValue<string>("action") == "privateMessage")))
                .Returns(Task.CompletedTask)
                .Verifiable();

            //act
            var result = await _chatService.SendMessage(conversationDto);

            //assert
            result.Success.ShouldBeTrue();

            _socketManagerMock.Verify();
        }
    }

    public static class ReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}