using ApiContracts_DTO;
using ApiContracts_DTO.Users;
using Entities;
using RepositoryContracts;
using EfcRepositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IMemoryCache cache;
    private readonly IPostRepository postRepository; // Maybe Use Later On
    private readonly ICommentRepository commentRepository; // Maybe Use Later On

    public UsersController(IUserRepository userRepository, IMemoryCache cache)
    {
        // Dependency Injection
        this.userRepository = userRepository;
        this.cache = cache;
    }
// --------------------------------------------------------------------------

    private async Task VerifyUserNameIsAvailableAsync(string username)
    {
        try
        {
            var user = await userRepository.GetByUsernameAsync(username);

            // If User already exists throw exception
            throw new InvalidOperationException(
                $"Brugernavn '{username}' er allerede brugt. Angiv nyt.");
        }
        catch (InvalidOperationException e)
        {
            if (!e.Message.Contains("Ej Fundet !"))
            {
                throw; // some other error related to user -> throw unspecified exception
            }
            // Could be "not found" error from repository -> 
            // ignore catch since username is available then
        }
         
    }
// --------------------------------------------------------------------------

    /*private void CacheInvalidate(int id)
    {
        cache.Remove($"user-{id}");
        cache.Remove("allUsers");
    }*/
// --------------------------------------------------------------------------

    // -- Create User -- (Add User)
    // https://localhost:7047/users + POS + BODY { UserName: "xx", Password: "xx" }
    
    [HttpPost]
    public async Task<ActionResult<UserDTO>> CreateUser(
        [FromBody] CreateUserDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException(
                $"Brugernavn skal udfyldes !");
        }
        
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                $"Password skal udfyldes");
        }

        try // Try Catch Example : Could be multiple catch depending on error
        {
            await VerifyUserNameIsAvailableAsync(request.Username);

            User user = new(0, request.Username, request.Password);
            User created = await userRepository.AddAsync(user);
            
            UserDTO dto = new()
            {
                Id = created.Id,
                Username = created.Name
            };
        
            return Created($"/Users/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e); // Test
            return StatusCode(500, e.Message);
        }
    }
// --------------------------------------------------------------------------

    // -- Update UserName --
    // PUT + BODY + { "id": 3, "Username": "Chris Coloumbus", "Password": null }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UpdateUsernameDTO>> UpdateUsername(int id,
        [FromBody] UserDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException(
                $"Brugernavn skal udfyldes !");
        }
        
        User user = await userRepository.GetSingleAsync(id);
        
        if (user.Name != request.Username)
        {
            await VerifyUserNameIsAvailableAsync(request.Username);
        }
        
        // Only update username
        //User user = new(0, request.Username);

        await userRepository.UpdateAsync(user);

        //CacheInvalidate(id);

        var dto = new UserDTO() { Id = user.Id, Username = user.Name };

        return Ok(dto);
    }
// --------------------------------------------------------------------------

    // -- Update Password --
    
    [HttpPut("{id:int}/password")]
    public async Task<ActionResult<UpdateUserPasswordDTO>> UpdatePassword(
        int id,
        [FromBody] UpdateUserPasswordDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                $"Password skal udfyldes !");
        }
        
        User user = await userRepository.GetSingleAsync(id);
        
        // Only update password
        //user = new(user.Id, user.Username, request.Password);
        //user = new User{Name = null, Pw = request.Password};
        //User user = new(0, null, request.Password);

        await userRepository.UpdateAsync(user);

        //CacheInvalidate(id);

        var dto = new UpdateUserPasswordDTO()
        {
            Id = user.Id,
            Password = user.Pw
        };

        // maybe not ideal to return password
        return Ok(dto);
    }
// --------------------------------------------------------------------------

    // -- GetSingle User --
    // https://localhost:7047/users/1
    
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

    // -- GET Many Users --
    // -- GET Many Users : endpoint to retrieve all users :
    // http://localhost:5274/users?startsWith=B&sortBy=username
    
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

        // -- Filter --
        
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
    
    // -- Delete User --
    // https://localhost:7047/users + DELETE + BODY { id: x }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await userRepository.DeleteAsync(id);

        //CacheInvalidate(id);

        return NoContent();
    }
// --------------------------------------------------------------------------
}