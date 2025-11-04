using System.Text.Json;

using ApiContracts_DTO;
using ApiContracts_DTO.Users;

namespace BlazorApp.HttpServices;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDTO> AddUserAsync(CreateUserDTO request)
    {
        // use the PostAsJsonAsync method on the client. This simplifies the process,
        // because we would otherwise have to manually serialize the CreateUserDto request parameter.
        //The method call sends the information to the Web API, which hits the POST /Users endpoint.
        //This endpoint returns some result, as json.
        HttpResponseMessage httpResponse =
            await _httpClient.PostAsJsonAsync("users", request);
        
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

    public async Task UpdateUserAsync(int id, UpdateUsernameDTO request)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.PutAsJsonAsync($"users/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task UpdatePasswordAsync(int id, UpdateUserPasswordDTO request)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.PutAsJsonAsync($"users/{id}/password", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task UpdateUser(int id, UpdateUserDTO request)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.PutAsJsonAsync($"users/{id}/update", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync(string? startsWith,
        string? sortBy)
    {
        var users =
            await _httpClient.GetFromJsonAsync<List<UserDTO>>(
                $"users?startsWith={startsWith}&sortBy={sortBy}");

        return users ?? Enumerable.Empty<UserDTO>();
    }

    public async Task<UserDTO> GetSingleUserByIdAsync(int id)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.GetAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<UserDTO>(response,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true })!;
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.DeleteAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
}