using ApiContracts_DTO;
using ApiContracts_DTO.Users;
using Entities;
using RepositoryContracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IMemoryCache cache;

    public UsersController(IUserRepository userRepository, IMemoryCache cache)
    {
        this.userRepository = userRepository;
        this.cache = cache;
    }
// --------------------------------------------------------------------------

    private async Task VerifyUserNameIsAvailableAsync(string username)
    {
        var users = userRepository.GetMany();
        if (users.Any(u =>
                u.Name.Equals(username)))
        {
            throw new InvalidOperationException(
                $"Username '{username}' is already taken.");
        }
    }
// --------------------------------------------------------------------------

    private void CacheInvalidate(int id)
    {
        cache.Remove($"user-{id}");
        cache.Remove("allUsers");
    }
// --------------------------------------------------------------------------

    // -- Create User : https://localhost:7047/users + POST + BODY { UserName: "xx", Password: "xx" }
    [HttpPost]
    public async Task<ActionResult<UserDTO>> CreateUser(
        [FromBody] CreateUserDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException(
                $"Username is required and cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                $"Password is required and cannot be empty");
        }

        await VerifyUserNameIsAvailableAsync(request.Username);

        //User user = new(0, request.Username, request.Password);
        User user = new User{Name = request.Username, Pw = request.Password};
        User created = await userRepository.AddAsync(user);
        UserDTO dto = new()
        {
            Id = created.Id,
            Username = created.Name
        };
        return Created($"/Users/{dto.Id}", dto);
    }
// --------------------------------------------------------------------------

    // -- Update UserName : PUT + BODY + { "id": 3, "Username": "Chris Coloumbus", "Password": null }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateUsernameDTO>> UpdateUsername(int id,
        [FromBody] UserDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException(
                $"Username is required and cannot be empty");
        }
        
        User user = await userRepository.GetSingleAsync(id);
        if (user.Name != request.Username)
        {
            await VerifyUserNameIsAvailableAsync(request.Username);
        }
        
        // Only update username
        //user = new(user.Id, request.Username, user.Pw);
        user = new User{Name = request.Username, Pw = null};


        await userRepository.UpdateAsync(user);

        CacheInvalidate(id);

        var dto = new UserDTO() { Id = user.Id, Username = user.Name };

        return Ok(dto);
    }
// --------------------------------------------------------------------------

    // -- Update Password
    [HttpPut("{id:int}/password")]
    public async Task<ActionResult<UpdateUserPasswordDTO>> UpdatePassword(
        int id,
        [FromBody] UpdateUserPasswordDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                $"Password is required and cannot be empty");
        }
        
        User user = await userRepository.GetSingleAsync(id);
        
        // Only update password
        //user = new(user.Id, user.Username, request.Password);
        user = new User{Name = null, Pw = request.Password};

        await userRepository.UpdateAsync(user);

        CacheInvalidate(id);

        var dto = new UpdateUserPasswordDTO()
        {
            Id = user.Id,
            Password = user.Pw
        };

        // maybe not ideal to return password
        return Ok(dto);
    }
// --------------------------------------------------------------------------

    // -- GetSingle User : https://localhost:7047/users/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSingleUserById(int id)
    {
        /*string cacheKey = $"post-{id}";

        if (cache.TryGetValue(cacheKey, out UserDTO? cachedUser))
        {
            return Ok(cachedUser);
        }*/

        var user = await userRepository.GetSingleAsync(id);

        var userDto = new UserDTO() { Id = user.Id, Username = user.Name };

        /*cache.Set(cacheKey, userDto, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(2),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });*/
        
        return Ok(userDto);
    }
// --------------------------------------------------------------------------

    // -- GET Many Users : endpoint to retrieve all users : http://localhost:5274/users
    // -- GET Many Users : endpoint to retrieve all users : http://localhost:5274/users?startsWith=B&sortBy=username
    [HttpGet]
    public async Task<ActionResult<UserDTO>> GetUsers(
        [FromQuery] string? startsWith,
        [FromQuery] string? sortBy)
    {

        //var userList = userRepository.GetMany();
        //
        // Why did GetMAny Read From The Old JSON Files in Debug Folder ?
        //foreach (var user in userList)
        //    Console.WriteLine($"({user.Id}) {user.Name}");
        
        var users = userRepository.GetMany() ;//.ToList();
       
        /*
        string allUsersCacheKey = "allUsers";
        
        if (!cache.TryGetValue(allUsersCacheKey,
                out IEnumerable<User>? cachedUsers))
        {
            cachedUsers = userRepository.GetMany().ToList();
            cache.Set(allUsersCacheKey, cachedUsers,
                new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }*/

        var filteredUsers = users;


        // Filter
        
        // Filter StartsWith ----------------------------
        if (!string.IsNullOrWhiteSpace(startsWith))
        {
            filteredUsers = filteredUsers.Where(u =>
                u.Name.StartsWith(startsWith,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Filter sortBy -------------------------------
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            switch (sortBy)
            {
                case "username":
                    filteredUsers = filteredUsers.OrderBy(u => u.Name);
                    break;
                case "id":
                    filteredUsers = filteredUsers.OrderBy(u => u.Id);
                    break;
            }
        }

        // The DTO does Not Need To Contain All The Fields Of The Class User
        // It Would Not Be Nice If You Could Display The Passwords Of All Users
        var filtUsers = filteredUsers
            .Select(u => new UserDTO
                {
                Id = u.Id, Username = u.Name
                })
            .ToList();

        return Ok(filtUsers);
    } 
// --------------------------------------------------------------------------
    
    // -- Delete User : https://localhost:7047/users + DELETE + BODY { id: x }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await userRepository.DeleteAsync(id);

        //CacheInvalidate(id);

        return NoContent();
    }
// --------------------------------------------------------------------------
}