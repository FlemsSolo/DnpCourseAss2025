using System.Text.Json;
using ApiContracts_DTO;

namespace BlazorApp.HttpServices;

public class PostService : IPostService
{
    private readonly HttpClient httpClient;

    public PostService(HttpClient httpClient)
    {
        // Dependency Injection
        this.httpClient = httpClient;
    }
// --------------------------------------------------------------------------

    // -- Create Post Async --
    public async Task<PostDTO> CreatePostAsync(CreatePostDTO request)
    {
        HttpResponseMessage httpResponse =
            await httpClient.PostAsJsonAsync("posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<PostDTO>(response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
// --------------------------------------------------------------------------

    // -- Update Post Async --
    public async Task UpdatePostAsync(int id, UpdatePostDTO request)
    {
        HttpResponseMessage httpResponse =
            await httpClient.PutAsJsonAsync($"posts/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
// --------------------------------------------------------------------------

    // -- Get Posts Async --
    public async Task<IEnumerable<PostDTO>> GetPostsAsync(string? title,
        int? userId, string? authorName)
    {
        var posts =
            await httpClient.GetFromJsonAsync<List<PostDTO>>(
                $"posts?title={title}&userId={userId}&authorName={authorName}");

        return posts ?? Enumerable.Empty<PostDTO>();
    }
// --------------------------------------------------------------------------

    // -- Get Post With Comment Async --
    /*
    public async Task<PostWithCommentsDTO?> GetPostWithCommentsAsync(int postId)
    {
        HttpResponseMessage httpResponse =
            await httpClient.GetAsync(
                $"posts?{postId}?include=comments");
        if (httpResponse.IsSuccessStatusCode)
        {
            return await httpResponse.Content.ReadFromJsonAsync<PostWithCommentsDTO>();;
        }

        return null;
    }
    */
// --------------------------------------------------------------------------

    // -- Get Single Post By Id Async --
    public async Task<PostWithCommentsDTO> GetSinglePostByIdAsync(int id,
        string? include)
    {
        HttpResponseMessage httpResponse =
            await httpClient.GetAsync($"posts/{id}?include={include}");
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null; // gracefully handle deleted posts
        }

        /*
         if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        */

        httpResponse.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<PostWithCommentsDTO>(response,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true })!;
    }
// --------------------------------------------------------------------------

    // -- Delete Post Async --
    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage httpResponse =
            await httpClient.DeleteAsync($"posts/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
// --------------------------------------------------------------------------
}