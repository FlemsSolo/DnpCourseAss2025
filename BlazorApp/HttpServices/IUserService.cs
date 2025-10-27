using ApiContracts_DTO;
using ApiContracts_DTO.Users;

namespace BlazorApp.HttpServices;

public interface IUserService
{
    public Task<UserDTO> AddUserAsync(CreateUserDTO request);
    public Task UpdateUserAsync(int id, UpdateUsernameDTO request);
    // ... more methods

}