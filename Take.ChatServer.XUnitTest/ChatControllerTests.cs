using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Take.ChatServer.Abstractions;
using Take.ChatServer.Controllers;
using Take.ChatServer.Dto;
using Xunit;

namespace Take.ChatServer.XUnitTest
{
    public class ChatControllerTests
    {
        private Mock<IChatDataService> _chatDataServiceMock;

        private ChatController _controller;

        public ChatControllerTests()
        {
            _chatDataServiceMock = new Mock<IChatDataService>(MockBehavior.Strict);

            _controller = new ChatController(_chatDataServiceMock.Object);
        }

        [Fact]
        public async Task Connect_WithRequestOk_ReturnsOkObjectResult()
        {
            //arrange
            var username = "Someone";

            _chatDataServiceMock.Setup(a => a.Connect(username))
                .ReturnsAsync(new ServiceDataResponse())
                .Verifiable();

            //act
            var result = (OkResult)await _controller.Connect(username);

            //assert
            result.StatusCode.ShouldBe(200);
        }

        [Fact]
        public async Task Connect_WithRequestFailed_ReturnsOkObjectResult()
        {
            //arrange
            var username = "Someone";

            _chatDataServiceMock.Setup(a => a.Connect(username))
                .ReturnsAsync(new ServiceDataResponse("something went wrong."))
                .Verifiable();

            //act
            var result = (ObjectResult)await _controller.Connect(username);

            //assert
            result.StatusCode.ShouldBe(409);
            var data = (IList<String>)result.Value;
            data.ShouldContain("something went wrong.");

            _chatDataServiceMock.Verify();
        }

        [Fact]
        public void Disconnect_WithRequestOk_ReturnsOkObjectResult()
        {
            var username = "Someone";

            _chatDataServiceMock.Setup(a => a.Disconnect(username))
                .Returns(new ServiceDataResponse())
                .Verifiable();

            //act
            var result = (OkResult)_controller.Disconnect(username);

            //assert
            result.StatusCode.ShouldBe(200);
        }

        [Fact]
        public void Disconnect_WithRequestFailed_ReturnsOkObjectResult()
        {
            var username = "Someone";

            _chatDataServiceMock.Setup(a => a.Disconnect(username))
                .Returns(new ServiceDataResponse("something went wrong."))
                .Verifiable();

            //act
            var result = (ObjectResult)_controller.Disconnect(username);

            //assert
            result.StatusCode.ShouldBe(409);
            var data = (IList<String>)result.Value;
            data.ShouldContain("something went wrong.");

            _chatDataServiceMock.Verify();
        }

        [Fact]
        public async Task SendMessage_WithRequestOk_ReturnsOkObjectResult()
        {
            //arrange
            var dto = new ConversationDto 
            {
                From = "Someone",
                isPublicMessage = true,
                Message = "Some text",
                To = "Someoneelse"
            };

            _chatDataServiceMock.Setup(a => a.SendMessage(dto))
                .ReturnsAsync(new ServiceDataResponse())
                .Verifiable();

            //act
            var result = (OkObjectResult)await _controller.SendMessage(dto);

            //assert
            result.StatusCode.ShouldBe(200);
        }

        [Fact]
        public async Task SendMessage_WithRequestFailed_ReturnsOkObjectResult()
        {
            //arrange
            var dto = new ConversationDto
            {
                From = "Someone",
                isPublicMessage = true,
                Message = "Some text",
                To = "Someoneelse"
            };

            _chatDataServiceMock.Setup(a => a.SendMessage(dto))
                .ReturnsAsync(new ServiceDataResponse("something went wrong."))
                .Verifiable();

            //act
            var result = (ObjectResult)await _controller.SendMessage(dto);

            //assert
            result.StatusCode.ShouldBe(409);
            var data = (IList<String>)result.Value;
            data.ShouldContain("something went wrong.");

            _chatDataServiceMock.Verify();
        }
    }
}
