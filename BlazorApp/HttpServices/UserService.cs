using System.Text.Json;

using ApiContracts_DTO;
using ApiContracts_DTO.Users;

namespace BlazorApp.HttpServices;

public class UserService : IUserService
{
    private readonly HttpClient httpClient;

    public UserService(HttpClient httpClient)
    {
        // Dependency Injection
        this.httpClient = httpClient;
    }
// --------------------------------------------------------------------------

    // -- Add User Async --
    public async Task<UserDTO> AddUserAsync(CreateUserDTO request)
    {
        // use the PostAsJsonAsync method on the client. This simplifies the process,
        // because we would otherwise have to manually serialize the CreateUserDto request parameter.
        // The method call sends the information to the Web API, which hits the POST /Users endpoint.
        // This endpoint returns some result, as json.
        
        HttpResponseMessage httpResponse =
            await httpClient.PostAsJsonAsync("users", request);
        
        // The result, i.e., httpResponse.Content, is read to a string.
        // This string is either the UserDto or some error message.
        
        string response = await httpResponse.Content.ReadAsStringAsync();
        
        // If the status code of the http response is not a success, we throw an exception.
        // This can be caught in a Blazor page, and a message can be shown to the user.
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        // Otherwise, the response is deserialized into the expected object, UserDto, and returned.
        // You might need this, e.g. to navigate to the user’s home page or whatever.
        
        return JsonSerializer.Deserialize<UserDTO>(response,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true })!;
    }
// --------------------------------------------------------------------------

    // -- Update User Async --
    public async Task UpdateUserAsync(int id, UpdateUsernameDTO request)
    {
        HttpResponseMessage httpResponse =
            await httpClient.PutAsJsonAsync($"users/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
// --------------------------------------------------------------------------

    // -- Update User Password Async --
    public async Task UpdatePasswordAsync(int id, UpdateUserPasswordDTO request)
    {
        HttpResponseMessage httpResponse =
            await httpClient.PutAsJsonAsync($"users/{id}/password", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
// --------------------------------------------------------------------------

    // -- Update User NO Async --
    public async Task UpdateUser(int id, UpdateUserDTO request)
    {
        HttpResponseMessage httpResponse =
            await httpClient.PutAsJsonAsync($"users/{id}/update", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
// --------------------------------------------------------------------------

    // -- Get Users Async --
    public async Task<IEnumerable<UserDTO>> GetUsersAsync(string? startsWith,
        string? sortBy)
    {
        var users =
            await httpClient.GetFromJsonAsync<List<UserDTO>>(
                $"users?startsWith={startsWith}&sortBy={sortBy}");

        return users ?? Enumerable.Empty<UserDTO>();
    }
// --------------------------------------------------------------------------

    // -- Get Single User By Id Async --
    public async Task<UserDTO> GetSingleUserByIdAsync(int id)
    {
        HttpResponseMessage httpResponse =
            await httpClient.GetAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<UserDTO>(response,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true })!;
    }
// --------------------------------------------------------------------------

    // -- Delete User Async --
    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse =
            await httpClient.DeleteAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
// --------------------------------------------------------------------------
}