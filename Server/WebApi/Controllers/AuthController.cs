using ApiContracts_DTO;
using Entities;
using RepositoryContracts;
using StudHub.SharedDTO;

using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    // -- Login --
    
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(
        [FromBody] LoginRequestDTO request)
    {
        User user;
        
        try
        {
            user = await userRepository.GetByUsernameAsync(request.Username);
        }
        catch (InvalidOperationException)
        {
            throw new UnauthorizedAccessException("Forkert Brugernavn");
        }

        if (user.Pw != request.Password)
        {
            throw new UnauthorizedAccessException("Forkert Password");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyThatIsAtMinimum32CharactersLong"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: "your-issuer",
            audience: "your-audience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

        var userDTO = new UserDTO
        {
            Id = user.Id,
            Username = user.Name
        };

        return Ok( new { User = userDTO, Token = jwt });
    }
}