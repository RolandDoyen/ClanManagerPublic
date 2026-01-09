using AutoMapper;
using ClanManager.BLL.BLL;
using ClanManager.BLL.DTO;
using ClanManager.BLL.Interfaces;
using ClanManager.Core;
using ClanManager.Core.Exceptions;
using ClanManager.DAL.DAO;
using ClanManager.DAL.Interfaces;
using Moq;
using Xunit;

namespace ClanManager.Tests.Unit.BLL
{
    public class UserBLLUnitTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly Mock<IValidationService> _mockValidationService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserBLL _bll;

        public UserBLLUnitTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockValidationService = new Mock<IValidationService>();
            _mockMapper = new Mock<IMapper>();
            _bll = new UserBLL(_mockRepository.Object, _mockValidationService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenSessionIdInvalid_ThrowsNoSessionUserException()
        {
            var invalidSessionUserId = "invalid-guid";
            _mockValidationService.Setup(v => v.Check_Session_User(invalidSessionUserId)).Throws(new NoSessionUserException());

            await Assert.ThrowsAsync<NoSessionUserException>(() => _bll.GetByIdAsync(Guid.NewGuid(), invalidSessionUserId));

            _mockValidationService.Verify(v => v.Check_Session_User(invalidSessionUserId), Times.Once);
            _mockMapper.Verify(x => x.Map<UserDTO>(new User { Id = Guid.NewGuid() }), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenSessionUserNotFound_ThrowsSessionUserNotFoundException()
        {
            var sessionUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();

            _mockValidationService.Setup(v => v.Check_Session_User(It.IsAny<string>())).Returns(sessionUserId);
            _mockValidationService.Setup(v => v.Find_Session_User(sessionUserId)).ThrowsAsync(new SessionUserNotFoundException());

            await Assert.ThrowsAsync<SessionUserNotFoundException>(() => _bll.GetByIdAsync(targetUserId, sessionUserId.ToString()));

            _mockValidationService.Verify(v => v.Find_Session_User(It.IsAny<Guid>()), Times.Once);
            _mockMapper.Verify(x => x.Map<UserDTO>(new User { Id = sessionUserId }), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTargetUserNotFound_ThrowsUserNotFoundException()
        {
            var sessionUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();

            _mockValidationService.Setup(v => v.Check_Session_User(It.IsAny<string>())).Returns(sessionUserId);
            _mockValidationService.Setup(v => v.Find_Session_User(sessionUserId)).ReturnsAsync(new User { Id = sessionUserId });
            _mockValidationService.Setup(v => v.Find_User(targetUserId)).ThrowsAsync(new UserNotFoundException());

            await Assert.ThrowsAsync<UserNotFoundException>(() => _bll.GetByIdAsync(targetUserId, sessionUserId.ToString()));

            _mockValidationService.Verify(v => v.Find_User(It.IsAny<Guid>()), Times.Once);
            _mockMapper.Verify(x => x.Map<UserDTO>(new User { Id = sessionUserId }), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenAdminRequestsOwnProfile_SetsContextDTOVariables()
        {
            var sessionUserId = Guid.NewGuid();
            var sessionUserEntity = new User { Id = sessionUserId, Role = Role.Admin };

            _mockValidationService.Setup(v => v.Check_Session_User(It.IsAny<string>())).Returns(sessionUserId);
            _mockValidationService.Setup(v => v.Find_Session_User(sessionUserId)).ReturnsAsync(sessionUserEntity);
            _mockValidationService.Setup(v => v.Find_User(sessionUserId)).ReturnsAsync(sessionUserEntity);
            _mockMapper.Setup(m => m.Map<UserDTO>(sessionUserEntity)).Returns(new UserDTO { Id = sessionUserId });

            var result = await _bll.GetByIdAsync(sessionUserId, sessionUserId.ToString());

            Assert.True(result.IsOwnProfile);
            Assert.Equal(result.SessionUserRole, sessionUserEntity.Role);
            Assert.False(result.IsUserSuperAdmin);
        }

        [Fact]
        public async Task GetByIdAsync_WhenSuperAdminRequestsOwnProfile_SetsContextDTOVariables()
        {
            var sessionUserId = Guid.NewGuid();
            var sessionUserEntity = new User { Id = sessionUserId, Role = Role.SuperAdmin };

            _mockValidationService.Setup(v => v.Check_Session_User(It.IsAny<string>())).Returns(sessionUserId);
            _mockValidationService.Setup(v => v.Find_Session_User(sessionUserId)).ReturnsAsync(sessionUserEntity);
            _mockValidationService.Setup(v => v.Find_User(sessionUserId)).ReturnsAsync(sessionUserEntity);
            _mockMapper.Setup(m => m.Map<UserDTO>(sessionUserEntity)).Returns(new UserDTO { Id = sessionUserId });

            var result = await _bll.GetByIdAsync(sessionUserId, sessionUserId.ToString());

            Assert.True(result.IsOwnProfile);
            Assert.Equal(result.SessionUserRole, sessionUserEntity.Role);
            Assert.True(result.IsUserSuperAdmin);
        }

        [Fact]
        public async Task GetByIdAsync_WhenSessionUserIsAdminAndTargetUserIsUser_SetsCanManageUsersToTrue()
        {
            var sessionUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var sessionUserAdmin = new User { Id = sessionUserId, Role = Role.Admin };
            var targetUserUser = new User { Id = targetUserId, Role = Role.User };

            ArrangeMoqs(sessionUserId, sessionUserAdmin, targetUserId, targetUserUser);

            var result = await _bll.GetByIdAsync(targetUserId, sessionUserId.ToString());

            Assert.True(result.CanManageUsers);
        }

        [Fact]
        public async Task GetByIdAsync_WhenSessionUserIsAdminAndTargetUserIsAdmin_SetsCanManageUsersToFalse()
        {
            var sessionUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var sessionUserAdmin = new User { Id = sessionUserId, Role = Role.Admin };
            var targetUserAdmin = new User { Id = targetUserId, Role = Role.Admin };

            ArrangeMoqs(sessionUserId, sessionUserAdmin, targetUserId, targetUserAdmin);

            var result = await _bll.GetByIdAsync(targetUserId, sessionUserId.ToString());

            Assert.False(result.CanManageUsers);

            _mockRepository.Verify(r => r.GetClanLeaderAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTargetUserIsClanLeader_SetsCanBanUserToFalse()
        {
            var sessionUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var sessionUserAdmin = new User { Id = sessionUserId, Role = Role.Admin };
            var targetUserUser = new User { Id = targetUserId, Role = Role.User };

            ArrangeMoqs(sessionUserId, sessionUserAdmin, targetUserId, targetUserUser);
            _mockRepository.Setup(r => r.GetClanLeaderAsync(targetUserId)).ReturnsAsync(new ClanMember());

            var result = await _bll.GetByIdAsync(targetUserId, sessionUserId.ToString());

            Assert.False(result.CanBanUser);

            _mockRepository.Verify(r => r.GetClanLeaderAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTargetUserIsNotClanLeader_SetsCanBanUserToTrue()
        {
            var sessionUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var sessionUserAdmin = new User { Id = sessionUserId, Role = Role.Admin };
            var targetUserUser = new User { Id = targetUserId, Role = Role.User };

            ArrangeMoqs(sessionUserId, sessionUserAdmin, targetUserId, targetUserUser);
            _mockRepository.Setup(r => r.GetClanLeaderAsync(targetUserId)).ReturnsAsync((ClanMember?)null);

            var result = await _bll.GetByIdAsync(targetUserId, sessionUserId.ToString());

            Assert.True(result.CanBanUser);

            _mockRepository.Verify(r => r.GetClanLeaderAsync(It.IsAny<Guid>()), Times.Once);
        }


        private void ArrangeMoqs(Guid sessionUserId, User sessionUser, Guid targetUserId, User targetUser)
        {
            _mockValidationService.Setup(v => v.Check_Session_User(It.IsAny<string>())).Returns(sessionUserId);
            _mockValidationService.Setup(v => v.Find_Session_User(sessionUserId)).ReturnsAsync(sessionUser);
            _mockValidationService.Setup(v => v.Find_User(targetUserId)).ReturnsAsync(targetUser);
            _mockMapper.Setup(m => m.Map<UserDTO>(targetUser)).Returns(new UserDTO { Id = targetUserId });
        }
    }
}