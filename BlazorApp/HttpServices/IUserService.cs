using ApiContracts_DTO;
using ApiContracts_DTO.Users;

namespace BlazorApp.HttpServices;

public interface IUserService
{
    public Task<UserDTO> AddUserAsync(CreateUserDTO request);
    public Task UpdateUserAsync(int id, UpdateUsernameDTO request);
    
    public Task UpdatePasswordAsync(int id, UpdateUserPasswordDTO request);

    public Task UpdateUser(int id, UpdateUserDTO request);
    public Task<IEnumerable<UserDTO>> GetUsersAsync(string? startsWith, string? sortBy);
    public Task<UserDTO> GetSingleUserByIdAsync(int id);
    public Task DeleteUserAsync(int id);
}