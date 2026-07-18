using AuraCommerce.IdentityApi.Controllers;
using AuraCommerce.IdentityApi.Core.DTOs;
using AuraCommerce.IdentityApi.Core.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AuraCommerce.IdentityApi.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ValidUser_ReturnsOkResult()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockAuthUser = new Mock<IAuthService>();

            var authController = new AuthController(mockAuthUser.Object, mockConfig.Object);

            var registerDto = new RegisterDto
            {
                Email = "testuser@auracommerce.com",
                Password = "StrongPassword123!"
            };

            var result = await authController.Register(registerDto);

            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public async Task Register_InValidUser_ReturnsBadResult()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockAuthUser = new Mock<IAuthService>();

            var authController = new AuthController(mockAuthUser.Object, mockConfig.Object);

            var registerDto = new RegisterDto
            {
                Email = "testuser@auracommerce.com",
                Password = "StrongPassword123!"
            };

            var result = await authController.Register(registerDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task Register_InValidUserRole_ReturnsBadResult()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockAuthUser = new Mock<IAuthService>();
            
            var authController = new AuthController(mockAuthUser.Object, mockConfig.Object);

            var registerDto = new RegisterDto
            {
                Email = "testuser@auracommerce.com",
                Password = "StrongPassword123!"
            };

            var result = await authController.Register(registerDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task Login_ValidUser_ReturnsOkResult()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockAuthUser = new Mock<IAuthService>();
            mockConfig.Setup(c => c["JwtSettings:SecretKey"]).Returns("ThisIsASuperSecretKeyThatIsAtLeast32BytesLong!");
            mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("auracommerce.com");
            mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("auracommerce.com");

            var authController = new AuthController(mockAuthUser.Object, mockConfig.Object);

            var loginDto = new LoginDto
            {
                Email = "testuser@auracommerce.com",
                Password = "StrongPassword123!"
            };

            var result = await authController.Login(loginDto);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task Login_NullLoginDto_ReturnsBadRequest()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockAuthUser = new Mock<IAuthService>();
            
            var authController = new AuthController(mockAuthUser.Object, mockConfig.Object);

            LoginDto loginDto = null;

            var result = await authController.Login(loginDto);

            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async Task Login_NullUser_ReturnsUnauthorized()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockAuthUser = new Mock<IAuthService>();
            mockConfig.Setup(c => c["JwtSettings:SecretKey"]).Returns("ThisIsASuperSecretKeyThatIsAtLeast32BytesLong!");
            mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("auracommerce.com");
            mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("auracommerce.com");

            var authController = new AuthController(mockAuthUser.Object, mockConfig.Object);

            var loginDto = new LoginDto
            {
                Email = "testuser@auracommerce.com",
                Password = "StrongPassword123!"
            };

            var result = await authController.Login(loginDto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        [Fact]
        public async Task Login_InValidPassword_ReturnsUnauthorized()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockAuthUser = new Mock<IAuthService>();
            
            mockConfig.Setup(c => c["JwtSettings:SecretKey"]).Returns("ThisIsASuperSecretKeyThatIsAtLeast32BytesLong!");
            mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("auracommerce.com");
            mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("auracommerce.com");

            var authController = new AuthController(mockAuthUser.Object, mockConfig.Object);

            var loginDto = new LoginDto
            {
                Email = "testuser@auracommerce.com",
                Password = "StrongPassword123!"
            };

            var result = await authController.Login(loginDto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
