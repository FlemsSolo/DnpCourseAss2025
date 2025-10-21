using ApiContracts_DTO;

namespace BlazorApp.HttpServices;

public interface IUserService
{
    public Task<UserDTO> AddUserAsync(CreateUserDTO request);
    public Task UpdateUserAsync(int id, UpdateUsernameDTO request);
    // ... more methods

}