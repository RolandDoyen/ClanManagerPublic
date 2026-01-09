using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.BLL.Interfaces;
using ClanManager.Core.Exceptions;
using ClanManager.WEB.Controllers;
using ClanManager.WEB.Models.User;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClanManager.Tests.Unit.Controller
{
    public class UserControllerUnitTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserBLL> _mockBLL;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<ISessionService> _mockSessionService;
        private readonly UserController _controller;

        public UserControllerUnitTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockBLL = new Mock<IUserBLL>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockSessionService = new Mock<ISessionService>();
            _controller = new UserController(_mockBLL.Object, _mockMapper.Object, _mockLogger.Object, _mockSessionService.Object);
        }

        #region Detail()
        [Fact]
        public async Task Detail_WhenFound_ReturnsUserDetailViewModel()
        {
            var userId = Guid.NewGuid();
            var userDTO = new UserDTO { Id = userId };
            var userContextDTO = new UserContextDTO { User = userDTO };
            var userDetailViewModel = new UserDetailViewModel { Id = userId };
            var sessionUserId = Guid.NewGuid().ToString();

            _mockSessionService.Setup(x => x.GetUserId()).Returns(sessionUserId);
            _mockBLL.Setup(x => x.GetByIdAsync(userId, sessionUserId)).ReturnsAsync(userContextDTO);
            _mockMapper.Setup(x => x.Map<UserDetailViewModel>(userContextDTO)).Returns(userDetailViewModel);

            var result = await _controller.Detail(userId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var responseModel = Assert.IsType<UserDetailViewModel>(viewResult.Model);
            Assert.Equal(userId, responseModel.Id);

            _mockBLL.Verify(x => x.GetByIdAsync(userId, sessionUserId), Times.Once);
            _mockMapper.Verify(x => x.Map<UserDetailViewModel>(userContextDTO), Times.Once);
        }

        [Fact]
        public async Task Detail_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            var targetId = Guid.NewGuid();
            var sessionUserId = Guid.NewGuid().ToString();

            _mockSessionService.Setup(x => x.GetUserId()).Returns(sessionUserId);

            _mockBLL.Setup(x => x.GetByIdAsync(targetId, sessionUserId)).ThrowsAsync(new UserNotFoundException());

            await Assert.ThrowsAsync<UserNotFoundException>(() => _controller.Detail(targetId));

            _mockMapper.Verify(x => x.Map<UserDetailViewModel>(It.IsAny<UserContextDTO>()), Times.Never);
        }
        #endregion
    }
}
