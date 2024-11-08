using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TransportationApplication.SharedModels;
using TransportationApplication.UserService.DTOs;
using TransportationApplication.UserService.Services;
using FluentAssertions;

namespace UserService.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserServices _userServices;

        public UserServiceTests()
        {
            // Mock dependencies
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<User>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>());

            _mockConfiguration = new Mock<IConfiguration>();

            // Mock JWT config with a more robust setup
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("DSLKJFASDIOFJAWIOEJ!@#!$!#$!#%!$SDJFALKJSDFAOldsjkfakdlvnma@#%$@#%q3452q345rkq23ejl4tnm32lk5t1q@#$!%)24rtj2345klj245@E%T234tjnwerjkl;tq;o235u25431q#%$@3451245tqkrgjlfqsdfgjasl/rgfERTQWREtq234j5rj2j45roi32u90uwejqwejklrqw/erjfq");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("domain.com");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("domain.com");

            // Initialize UserServices with all dependencies
            _userServices = new UserServices(_mockUserManager.Object, _mockSignInManager.Object, _mockRoleManager.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnToken_ForValidCreds()
        {
            // Arrange
            var loginModel = new LoginViewModel
            {
                Email = "test@test.com",
                Password = "GoodEnoughPassword@1"
            };

            var user = new User
            {
                UserName = "test@test.com",
                Email = "test@test.com",
                Id = "user-id-123" 
            };

            // Mock user retrieval
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Mock sign-in manager to return success
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(
                It.Is<string>(email => email == loginModel.Email),
                It.Is<string>(password => password == loginModel.Password),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            // Mock getting roles for the user
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "UserRole" });

            // Act
            var result = await _userServices.LoginUserAsync(loginModel);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsNull_ForInvalidPassword()
        {
            // Arrange
            var loginModel = new LoginViewModel { Email = "test@test.com", Password = "bad" };
            var user = new User
            {
                UserName = "test@test.com",
                Email = "test@test.com",
                Id = "user-id-123" 
            };

            // Mock the user retrieval
            _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email)).ReturnsAsync(user);

            // Mock sign-in manager to simulate an invalid password
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(
                It.Is<string>(email => email == loginModel.Email),
                It.Is<string>(password => password == loginModel.Password),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var token = await _userServices.LoginUserAsync(loginModel);

            // Assert
            Assert.Null(token);
        }


        [Fact]
        public async Task LoginUserAsync_ReturnsNull_ForInvalidEmail()
        {
            // Arrange
            var loginModel = new LoginViewModel { Email = "invalid@test.com", Password = "GoodEnoughPassword@1" };
            
            // Mock the user retrieval to return null for any email
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // Mock the sign-in to fail since the email is invalid
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var token = await _userServices.LoginUserAsync(loginModel);

            // Assert
            Assert.Null(token);
        }

    }
}
