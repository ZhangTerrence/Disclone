using System.Security.Claims;
using Disclone.API.Controllers;
using Disclone.API.DTOs;
using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Disclone.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly AuthController _authController;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public AuthControllerTests()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object, Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
        _tokenServiceMock = new Mock<ITokenService>();
        _authController = new AuthController(_userManagerMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task AuthController_RegisterUser_ReturnOk()
    {
        // Arrange
        var mockRegisterDTO = new RegisterRequestDTO
        {
            UserName = "Testing123",
            Email = "Test@gmail.com",
            Password = "@Password123"
        };
        const string mockAccessToken =
            "+6zgm05GFFT4unczEZIupiI8SuM4X87qAkAL1XHFUcVuEmcxhIvWR5PIDOCThDOcoMcZK5nT5npnIkITY1onmg==";
        const string mockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
        // No existing users found with username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        // Successfully creates and assigns user to USER role.
        _userManagerMock.Setup(o => o.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(o => o.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        // Successfully generates access and refresh tokens.
        _tokenServiceMock.Setup(o => o.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(mockAccessToken);
        _tokenServiceMock.Setup(o => o.GenerateRefreshToken()).Returns(mockRefreshToken);
        // Successfully saves updated user.
        _userManagerMock.Setup(o => o.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authController.RegisterUser(mockRegisterDTO);
        var objectResult = result as OkObjectResult;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<CredentialsResponseDTO>(objectResult.Value);
        Assert.Equal(mockAccessToken, ((CredentialsResponseDTO)objectResult.Value).AccessToken);
        Assert.Equal(mockRefreshToken, ((CredentialsResponseDTO)objectResult.Value).RefreshToken);
    }

    [Fact]
    public async Task AuthController_RegisterUser_ReturnBadRequest_UserExists()
    {
        // Arrange
        var mockRegisterDTO = new RegisterRequestDTO
        {
            UserName = "Testing123",
            Email = "Test@gmail.com",
            Password = "@Password123"
        };
        var mockUser = new Mock<ApplicationUser>();
        // A user was found with username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser.Object);

        // Act
        var result = await _authController.RegisterUser(mockRegisterDTO);
        var objectResult = result as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<ErrorResponseDTO>(objectResult.Value);
        Assert.IsType<Dictionary<string, IEnumerable<string>>>(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Single(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Equal("Username has already been taken.",
            ((ErrorResponseDTO)objectResult.Value).Errors["FindByNameAsync"].First());
    }


    [Fact]
    public async Task AuthController_RegisterUser_ReturnBadRequest_CreateUserError()
    {
        // Arrange
        var mockRegisterDTO = new RegisterRequestDTO
        {
            UserName = "Testing123",
            Email = "Test@gmail.com",
            Password = "@Password123"
        };
        // No existing users found with username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        // Fails to create user.
        _userManagerMock.Setup(o => o.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Description = "Unable to create user."
            }));

        // Act
        var result = await _authController.RegisterUser(mockRegisterDTO);
        var objectResult = result as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<ErrorResponseDTO>(objectResult.Value);
        Assert.IsType<Dictionary<string, IEnumerable<string>>>(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Single(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Equal("Unable to create user.",
            ((ErrorResponseDTO)objectResult.Value).Errors["CreateAsync"].First());
    }

    [Fact]
    public async Task AuthController_RegisterUser_ReturnBadRequest_AssignUserError()
    {
        // Arrange
        var mockRegisterDTO = new RegisterRequestDTO
        {
            UserName = "Testing123",
            Email = "Test@gmail.com",
            Password = "@Password123"
        };
        // No existing users found with username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        // Successfully creates user but fails to assign user to USER role.
        _userManagerMock.Setup(o => o.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(o => o.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Description = "Unable to assign user to USER role."
            }));

        // Act
        var result = await _authController.RegisterUser(mockRegisterDTO);
        var objectResult = result as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<ErrorResponseDTO>(objectResult.Value);
        Assert.IsType<Dictionary<string, IEnumerable<string>>>(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Single(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Equal("Unable to assign user to USER role.",
            ((ErrorResponseDTO)objectResult.Value).Errors["AddToRoleAsync"].First());
    }

    [Fact]
    public async Task AuthController_RegisterUser_ReturnBadRequest_UpdateUserError()
    {
        // Arrange
        var mockRegisterDTO = new RegisterRequestDTO
        {
            UserName = "Testing123",
            Email = "Test@gmail.com",
            Password = "@Password123"
        };
        const string mockAccessToken =
            "+6zgm05GFFT4unczEZIupiI8SuM4X87qAkAL1XHFUcVuEmcxhIvWR5PIDOCThDOcoMcZK5nT5npnIkITY1onmg==";
        const string mockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
        // No existing users found with username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        // Successfully creates and assigns user to USER role.
        _userManagerMock.Setup(o => o.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(o => o.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        // Successfully generates access and refresh tokens.
        _tokenServiceMock.Setup(o => o.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(mockAccessToken);
        _tokenServiceMock.Setup(o => o.GenerateRefreshToken()).Returns(mockRefreshToken);
        // Fails to save updated user.
        _userManagerMock.Setup(o => o.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(
            new IdentityError
            {
                Description = "Unable to update user."
            }));

        // Act
        var result = await _authController.RegisterUser(mockRegisterDTO);
        var objectResult = result as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<ErrorResponseDTO>(objectResult.Value);
        Assert.IsType<Dictionary<string, IEnumerable<string>>>(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Single(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Equal("Unable to update user.",
            ((ErrorResponseDTO)objectResult.Value).Errors["UpdateAsync"].First());
    }

    [Fact]
    public async Task AuthController_LoginUser_ReturnOk()
    {
        // Arrange
        var mockLoginDTO = new LoginRequestDTO
        {
            UserName = "Testing123",
            Password = "@Password123"
        };
        var mockUser = new Mock<ApplicationUser>();
        const string mockAccessToken =
            "+6zgm05GFFT4unczEZIupiI8SuM4X87qAkAL1XHFUcVuEmcxhIvWR5PIDOCThDOcoMcZK5nT5npnIkITY1onmg==";
        const string mockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
        // A user is found with matching username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
        // Credentials match.
        _userManagerMock.Setup(o => o.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        // Successfully generates access and refresh tokens.
        _tokenServiceMock.Setup(o => o.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(mockAccessToken);
        _tokenServiceMock.Setup(o => o.GenerateRefreshToken()).Returns(mockRefreshToken);
        // Successfully saves updated user.
        _userManagerMock.Setup(o => o.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authController.LoginUser(mockLoginDTO);
        var objectResult = result as OkObjectResult;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<CredentialsResponseDTO>(objectResult.Value);
        Assert.Equal(mockAccessToken, ((CredentialsResponseDTO)objectResult.Value).AccessToken);
        Assert.Equal(mockRefreshToken, ((CredentialsResponseDTO)objectResult.Value).RefreshToken);
    }

    [Fact]
    public async Task AuthController_LoginUser_ReturnNotFound_User404Error()
    {
        // Arrange
        var mockLoginDTO = new LoginRequestDTO
        {
            UserName = "Testing123",
            Password = "@Password123"
        };
        // No users were found with matching username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _authController.LoginUser(mockLoginDTO);
        var objectResult = result as NotFoundObjectResult;

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<ErrorResponseDTO>(objectResult.Value);
        Assert.Single(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Equal("User not found.",
            ((ErrorResponseDTO)objectResult.Value).Errors["FindByNameAsync"].First());
    }

    [Fact]
    public async Task AuthController_LoginUser_ReturnUnauthorized_InvalidCredentials()
    {
        // Arrange
        var mockLoginDTO = new LoginRequestDTO
        {
            UserName = "Testing123",
            Password = "@Password123"
        };
        var mockUser = new Mock<ApplicationUser>();
        // A user is found with matching username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
        // Credentials do not match.
        _userManagerMock.Setup(o => o.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _authController.LoginUser(mockLoginDTO);
        var objectResult = result as UnauthorizedObjectResult;

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<ErrorResponseDTO>(objectResult.Value);
        Assert.IsType<Dictionary<string, IEnumerable<string>>>(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Single(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Equal("Invalid username or password.",
            ((ErrorResponseDTO)objectResult.Value).Errors["CheckPasswordAsync"].First());
    }

    [Fact]
    public async Task AuthController_LoginUser_ReturnBadRequest_UpdateUserError()
    {
        // Arrange
        var mockLoginDTO = new LoginRequestDTO
        {
            UserName = "Testing123",
            Password = "@Password123"
        };
        var mockUser = new Mock<ApplicationUser>();
        const string mockAccessToken =
            "+6zgm05GFFT4unczEZIupiI8SuM4X87qAkAL1XHFUcVuEmcxhIvWR5PIDOCThDOcoMcZK5nT5npnIkITY1onmg==";
        const string mockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
        // A user is found with matching username.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
        // Credentials match.
        _userManagerMock.Setup(o => o.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        // Successfully generates access and refresh tokens.
        _tokenServiceMock.Setup(o => o.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(mockAccessToken);
        _tokenServiceMock.Setup(o => o.GenerateRefreshToken()).Returns(mockRefreshToken);
        // Fails to save updated user.
        _userManagerMock.Setup(o => o.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(
            new IdentityError
            {
                Description = "Unable to update user."
            }));

        // Act
        var result = await _authController.LoginUser(mockLoginDTO);
        var objectResult = result as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<ErrorResponseDTO>(objectResult.Value);
        Assert.IsType<Dictionary<string, IEnumerable<string>>>(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Single(((ErrorResponseDTO)objectResult.Value).Errors);
        Assert.Equal("Unable to update user.",
            ((ErrorResponseDTO)objectResult.Value).Errors["UpdateAsync"].First());
    }
}