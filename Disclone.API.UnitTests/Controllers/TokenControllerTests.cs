using System.Security.Claims;
using Disclone.API.Controllers;
using Disclone.API.DTOs;
using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Disclone.API.Tests.Controllers;

public class TokenControllerTests
{
    private readonly TokenController _tokenController;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public TokenControllerTests()
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
        _tokenController = new TokenController(_userManagerMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task TokenController_RefreshToken_ReturnOk()
    {
        // Arrange
        const string oldMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
        const string newMockAccessToken =
            "+6zgm05GFFT4unczEZIupiI8SuM4X87qAkAL1XHFUcVuEmcxhIvWR5PIDOCThDOcoMcZK5nT5npnIkITY1xuwg==";
        const string newMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEppYqw==";
    
        // Sets up HTTP context.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Testing123"),
            new(ClaimTypes.Role, "User")
        };
        _tokenController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "Refresh", oldMockRefreshToken }
                },
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };

        var mockUser = new ApplicationUser
        {
            UserName = "Testing123",
            About = "",
            Email = "Test@gmail.com",
            RefreshToken = oldMockRefreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime(),
            DateCreated = DateTime.Now.ToUniversalTime(),
            DateModified = DateTime.Now.ToUniversalTime()
        };
        // User found with matching username as in cookie.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser);
        // Successfully generates access and refresh tokens.
        _tokenServiceMock.Setup(o => o.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(newMockAccessToken);
        _tokenServiceMock.Setup(o => o.GenerateRefreshToken()).Returns(newMockRefreshToken);
        // Successfully saves updated user.
        _userManagerMock.Setup(o => o.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _tokenController.RefreshToken();
        var objectResult = result as ObjectResult;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(objectResult);
        Assert.IsType<CredentialsResponseDTO>(objectResult.Value);
        Assert.Equal(newMockAccessToken, ((CredentialsResponseDTO)objectResult.Value).AccessToken);
        Assert.Equal(newMockRefreshToken, ((CredentialsResponseDTO)objectResult.Value).RefreshToken);
    }
    
    [Fact]
    public async Task TokenController_RefreshToken_ReturnUnauthorized_MissingUserClaimError()
    {
        // Arrange
        const string oldMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
    
        // Sets up HTTP context with missing user claim.
        _tokenController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "Refresh", oldMockRefreshToken }
                },
            }
        };
        
        // Act
        var result = await _tokenController.RefreshToken();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
    
        [Fact]
    public async Task TokenController_RefreshToken_ReturnUnauthorized_MissingRefreshTokenError()
    {
        // Arrange
        // Sets up HTTP context with missing refresh token.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Testing123"),
            new(ClaimTypes.Role, "User")
        };
        _tokenController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };

        
        // Act
        var result = await _tokenController.RefreshToken();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
    
    [Fact]
    public async Task TokenController_RefreshToken_ReturnNotFound_User404Error()
    {
        // Arrange
        const string oldMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
    
        // Sets up HTTP context.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Testing123"),
            new(ClaimTypes.Role, "User")
        };
        _tokenController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "Refresh", oldMockRefreshToken }
                },
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };
        
        // No users were found with matching username. 
        _userManagerMock.Setup(o => o.FindByNameAsync("Testing1234")).ReturnsAsync((ApplicationUser?)null);
        
        // Act
        var result = await _tokenController.RefreshToken();
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
    public async Task TokenController_RefreshToken_ReturnForbid_InvalidRefreshTokenError()
    {
        // Arrange
        const string oldMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
    
        // Sets up HTTP context.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Testing123"),
            new(ClaimTypes.Role, "User")
        };
        _tokenController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "Refresh", oldMockRefreshToken }
                },
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };
        
        var mockUser = new ApplicationUser
        {
            UserName = "Testing123",
            About = "",
            Email = "Test@gmail.com",
            RefreshToken = "WrongRefreshToken",
            RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime(),
            DateCreated = DateTime.Now.ToUniversalTime(),
            DateModified = DateTime.Now.ToUniversalTime()
        };
        // User found with matching username as in cookie.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser);

        // Act
        var result = await _tokenController.RefreshToken();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task TokenController_RefreshToken_ReturnForbid_ExpiredRefreshTokenError()
    {
        // Arrange
        const string oldMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
    
        // Sets up HTTP context.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Testing123"),
            new(ClaimTypes.Role, "User")
        };
        _tokenController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "Refresh", oldMockRefreshToken }
                },
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };

        var mockUser = new ApplicationUser
        {
            UserName = "Testing123",
            About = "",
            Email = "Test@gmail.com",
            RefreshToken = oldMockRefreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddDays(-1).ToUniversalTime(),
            DateCreated = DateTime.Now.ToUniversalTime(),
            DateModified = DateTime.Now.ToUniversalTime()
        };
        // User found with matching username as in cookie.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser);

        // Act
        var result = await _tokenController.RefreshToken();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task TokenController_RefreshToken_ReturnBadRequest_UpdateUserError()
    {
        // Arrange
        const string oldMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEptT3w==";
        const string newMockAccessToken =
            "+6zgm05GFFT4unczEZIupiI8SuM4X87qAkAL1XHFUcVuEmcxhIvWR5PIDOCThDOcoMcZK5nT5npnIkITY1xuwg==";
        const string newMockRefreshToken =
            "43D9FeOcBLe9FaNqTMY/lliNZ1W67uLdwS0EgjBSf/pso6S9MND0w3/lOIAeRbeaRRZU0DofTiK1hS6XEppYqw==";
    
        // Sets up HTTP context.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Testing123"),
            new(ClaimTypes.Role, "User")
        };
        _tokenController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>
                {
                    { "Refresh", oldMockRefreshToken }
                },
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };

        var mockUser = new ApplicationUser
        {
            UserName = "Testing123",
            About = "",
            Email = "Test@gmail.com",
            RefreshToken = oldMockRefreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime(),
            DateCreated = DateTime.Now.ToUniversalTime(),
            DateModified = DateTime.Now.ToUniversalTime()
        };
        // User found with matching username as in cookie.
        _userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(mockUser);
        // Successfully generates access and refresh tokens.
        _tokenServiceMock.Setup(o => o.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(newMockAccessToken);
        _tokenServiceMock.Setup(o => o.GenerateRefreshToken()).Returns(newMockRefreshToken);
        // Fails to save updated user.
        _userManagerMock.Setup(o => o.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(
            new IdentityError
            {
                Description = "Unable to update user."
            }));

        // Act
        var result = await _tokenController.RefreshToken();
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