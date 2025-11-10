using ApiContracts_DTO;
using Entities;
using RepositoryContracts;
using StudHub.SharedDTO;

using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // -- Login --
    
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(
        [FromBody] LoginRequestDTO request)
    {
        User user;
        try
        {
            user = await _userRepository.GetByUsernameAsync(request.Username);
        }
        catch (InvalidOperationException)
        {
            throw new UnauthorizedAccessException("Forkert Brugernavn");
        }

        if (user.Pw != request.Password)
        {
            throw new UnauthorizedAccessException("Forkert Password");
        }

        var dto = new UserDTO
        {
            Id = user.Id,
            Username = user.Name
        };

        return Ok(dto);
    }
}