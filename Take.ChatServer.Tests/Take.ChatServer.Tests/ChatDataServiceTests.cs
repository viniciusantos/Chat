using Xunit;
using Moq;
using Take.ChatServer.Abstractions;
using Take.ChatServer.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Shouldly;

namespace Take.ChatServer.Tests
{
    public class ChatDataServiceTests
    {
        private Mock<TakeSocketManagerService> _socketManagerMock;
        private Mock<IHttpContextAccessor> _httpContextMock;
        private IChatDataService _chatService;

        public ChatDataServiceTests()
        {
            _socketManagerMock = new Mock<TakeSocketManagerService>(MockBehavior.Strict);
            _httpContextMock = new Mock<IHttpContextAccessor>(MockBehavior.Strict);

            _chatService = new ChatDataService(_socketManagerMock.Object,
                _httpContextMock.Object);
        }

        [Fact]
        public async Task Connect_WithUsernameCannotBeEmpty_ReturnsError()
        {
            //arrange
            var username = string.Empty;

            //act
            var result = await _chatService.Connect(username);

            //assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("Username cannot be empty.");
        } 
    }
}