using System.Security.Claims;
using System.Text.Json;
using ApiContracts_DTO;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Components.Authentication;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;


    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        // Dependency Injection
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        string userAsJson = "";
        try
        {
            userAsJson =
                await jsRuntime.InvokeAsync<string>("sessionStorage.getItem",
                    "currentUser");
        }
        catch (InvalidOperationException e)
        {
            return new AuthenticationState(new());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new());
        }

        UserDTO userDto = JsonSerializer.Deserialize<UserDTO>(userAsJson)!;
        
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
        };
        
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
        
        // The ?? is a null coalescing operator. It will check if claimsPrincipal is null,
        // and if so, then return the part after the ??, i.e. a new instance of ClaimsPrincipal.
        //return new AuthenticationState(claimsPrincipal ?? new ());
        
        return new AuthenticationState(claimsPrincipal);
    }

    public async Task LoginASync(string userName, string password)
    {
        Console.WriteLine("LoginASync(): " + userName + ", " + password);
        // httpClient Is Used To Contact The Web API
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequestDTO { Username = userName, Password = password });

        string content = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Response: " + content);
        Console.WriteLine("response.IsSuccessStatusCode: " + response.IsSuccessStatusCode);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        /*UserDTO userDto = JsonSerializer.Deserialize<UserDTO>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

        string serialisedData = JsonSerializer.Serialize(userDto);
        
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser",
            serialisedData);
        
        await RefreshUser(userDto);*/
        
        // LoginResponseDTO instead of UserDTO for web api JWT testing (httpie)
        LoginResponseDTO loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

        string serialisedData = JsonSerializer.Serialize(loginResponse);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser",
            serialisedData);

        await RefreshUser(loginResponse.User);

        
    }

    public async Task Logout()
    {
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser",
            "");
        
        /*ClaimsPrincipal currentClaimsPrincipal = new();
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentClaimsPrincipal)));*/
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new())));

    }
    
    public async Task RefreshUser(UserDTO userDto)
    {
        string serializedData = JsonSerializer.Serialize(userDto);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serializedData);
        
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
        };
        
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(claimsPrincipal))
        );
    }

}