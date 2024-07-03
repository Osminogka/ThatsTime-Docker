using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq.Expressions;
using webapi.Controllers;
using webapi.DL.Infrastructure;
using webapi.DL.Repositories;
using webapi.Models;
using webapi.Models.User;

namespace ThatsTime.Tests.Controllers
{
    public class FriendControllerTests
    {
        private Mock<IBaseRepository<UserInfo>> UserInfoRepository;
        private Mock<IBaseRepository<FriendsList>> FriendListRepository;
        private Mock<IBaseRepository<FriendInvites>> FriendInvitesRepository;
        private Mock<IGetUsername> GetUsernameRepository;

        private FriendManagementController Controller;

        public FriendControllerTests()
        {
            var users = new List<UserInfo>
            {
                new UserInfo
                {
                    Id = 1,
                    UserName = "TestUser"
                },
                new UserInfo
                {
                    Id = 2,
                    UserName = "TestUser2"
                },
                new UserInfo
                {
                    Id = 3,
                    UserName = "TestUser3"
                }
            };

            var friends = new List<FriendsList>
            {
                new FriendsList
                {
                    Id = 1,
                    FirstUserId = 1,
                    SecondUserId = 2
                }
            };

            var friendsInvites = new List<FriendInvites>
            {
                new FriendInvites
                {
                    Id = 1,
                    SenderUserId = 3,
                    TargetUserId = 1
                }
            };

            UserInfoRepository = new Mock<IBaseRepository<UserInfo>>();

            UserInfoRepository.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<UserInfo, bool>>>()))
                .ReturnsAsync((Expression<Func<UserInfo, bool>> exp) => users.AsQueryable().SingleOrDefault(exp));

            UserInfoRepository.Setup(x => x.Where(It.IsAny<Expression<Func<UserInfo, bool>>>()))
                .Returns((Expression<Func<UserInfo, bool>> exp) => users.AsQueryable().Where(exp));

            FriendListRepository = new Mock<IBaseRepository<FriendsList>>();

            FriendListRepository.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<FriendsList, bool>>>()))
                .ReturnsAsync((Expression<Func<FriendsList, bool>> exp) => friends.AsQueryable().SingleOrDefault(exp));

            FriendListRepository.Setup(x => x.Where(It.IsAny<Expression<Func<FriendsList, bool>>>()))
                .Returns((Expression<Func<FriendsList, bool>> exp) => friends.AsQueryable().Where(exp));

            FriendListRepository.Setup(x => x.Delete(It.IsAny<FriendsList>()))
                .Callback((FriendsList entity) => friends.RemoveAt(friends.FindIndex(x => x.Id == entity.Id)));

            FriendListRepository.Setup(x => x.AddAsync(It.IsAny<FriendsList>()))
                .Callback((FriendsList entity) => friends.Add(entity));

            FriendListRepository.Setup(x => x.SaveChanges())
                .ReturnsAsync(0);

            FriendInvitesRepository = new Mock<IBaseRepository<FriendInvites>>();

            FriendInvitesRepository.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<FriendInvites, bool>>>()))
                .ReturnsAsync((Expression<Func<FriendInvites, bool>> exp) => friendsInvites.AsQueryable().SingleOrDefault(exp));

            FriendInvitesRepository.Setup(x => x.Where(It.IsAny<Expression<Func<FriendInvites, bool>>>()))
                .Returns((Expression<Func<FriendInvites, bool>> exp) => friendsInvites.AsQueryable().Where(exp));

            FriendInvitesRepository.Setup(x => x.Delete(It.IsAny<FriendInvites>()))
                .Callback((FriendInvites entity) => friendsInvites.RemoveAt(friendsInvites.FindIndex(x => x.Id == entity.Id)));

            FriendInvitesRepository.Setup(x => x.DeleteRange(It.IsAny<List<FriendInvites>>()))
                .Callback((List<FriendInvites> invites) => friendsInvites.RemoveRange(0,1));

            FriendInvitesRepository.Setup(x => x.AddAsync(It.IsAny<FriendInvites>()))
                .Callback((FriendInvites entity) => friendsInvites.Add(entity));

            FriendInvitesRepository.Setup(x => x.SaveChanges())
                .ReturnsAsync(0);

            GetUsernameRepository = new Mock<IGetUsername>();

            GetUsernameRepository.Setup(x => x.getUserName(It.IsAny<HttpContext>()))
                .Returns("TestUser");

            Controller = new FriendManagementController(UserInfoRepository.Object, FriendListRepository.Object, FriendInvitesRepository.Object, GetUsernameRepository.Object);
        }

        [Fact]
        public void CanSendFriendInvite()
        {
            //Arrange

            //Act
            var result = Controller.SendFrienInviteToUserAsync("TestUser3");

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<LoginResponse>(okResult.Value);

            Assert.Equal(true, model.Success);
        }
    }
}
