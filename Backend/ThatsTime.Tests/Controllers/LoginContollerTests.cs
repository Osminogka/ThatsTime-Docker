using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using webapi.Controllers;
using webapi.DL.Repositories;
using webapi.Models;
using webapi.Models.User;

namespace ThatsTime.Tests.Controllers
{
    public class LoginContollerTests
    {
        private IUsersRepository Repository { get; }

        private Mock<IBaseRepository<UserInfo>> UserRepository { get; }

        private ApiAuthController Controller { get; }

        public LoginContollerTests()
        {
            var entities = new List<UserInfo>
            {
                new UserInfo
                {
                    Id = 1,
                    UserName = "TestUser"
                }
            };

            var users = new List<IdentityUser>
            {
                new IdentityUser
                {
                    UserName = "TestUser",
                    Email = "test@gmail.com"
                }
            }.AsQueryable();

            var fakeUserManager = new Mock<FakeUserManager>();

            fakeUserManager.Setup(x => x.Users)
                .Returns(users);

            fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var signInManager = new Mock<FakeSignInManager>();
            signInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var configuration = GetTestConfiguration();

            UserRepository = new Mock<IBaseRepository<UserInfo>>();

            UserRepository.Setup(x => x.AddAsync(It.IsAny<UserInfo>()))
                .Callback((UserInfo entity) => entities.Add(entity));

            Repository = new UsersRepository(fakeUserManager.Object, signInManager.Object);
            Controller = new ApiAuthController(Repository, configuration, UserRepository.Object);
        }

        [Fact]
        public async Task CanRegisterUser()
        {
            //Arrange
            var testUser = new SingupCredentials
            {
                Username = "TestUser2",
                Email = "testuser2@gmail.com",
                Password = "TestUser1234!"
            };

            //Act
            var result = await Controller.ApiSignUp(testUser);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<LoginResponse>(okResult.Value);

            Assert.Equal(true, model.Success);
        }

        private IConfiguration GetTestConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:Key", "EUt719k5GENP1pWWhrmyDldHPaKXyIa9yImWhPuqHBUlgZ10Fk"},
            // Add more settings as needed
        };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return configuration;
        }
    }
}
