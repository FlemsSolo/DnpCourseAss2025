using System.Text.Json;
using ApiContracts_DTO;

namespace BlazorApp.HttpServices;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    // HttpClient is automatically injected through the constructor
    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDTO> AddUserAsync(CreateUserDTO request)
    {
        // use the PostAsJsonAsync method on the client. This simplifies the process,
        // because we would otherwise have to manually serialize the CreateUserDto request parameter.
        //The method call sends the information to the Web API, which hits the POST /Users endpoint.
        //This endpoint returns some result, as json.
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("users", request);
        
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
        return JsonSerializer.Deserialize<UserDTO>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task UpdateUserAsync(int id, UpdateUsernameDTO request)
    {
        // todo...
    }

    // more methods...
}