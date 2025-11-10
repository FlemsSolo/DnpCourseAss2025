using System.Text.Json;
using ApiContracts_DTO;

namespace BlazorApp.HttpServices;

public class CommentService : ICommentService
{
    private readonly HttpClient httpClient;

    public CommentService(HttpClient httpClient)
    {
        // Dependency Injection
        this.httpClient = httpClient;
    }

// --------------------------------------------------------------------------
    // -- Create Comment Async --
    public async Task<CommentDTO> CreateCommentAsync(CreateCommentDTO request)
    {
        HttpResponseMessage httpResponse =
            await httpClient.PostAsJsonAsync("comments", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<CommentDTO>(response,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true })!;
    }

// --------------------------------------------------------------------------

    // -- Update Comment Async --
    public async Task UpdateCommentAsync(int id, UpdateCommentDTO request)
    {
        HttpResponseMessage httpResponse =
            await httpClient.PutAsJsonAsync($"comments/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

// --------------------------------------------------------------------------
    // -- Get Comments Async --
    public async Task<IEnumerable<CommentDTO>> GetCommentsAsync(int? userId,
        string? authorName, int? postId,
        string? sortBy)
    {
        var comments =
            await httpClient.GetFromJsonAsync<List<CommentDTO>>(
                $"comments?userid={userId}&authorName={authorName}&postid={postId}&sortBy={sortBy}");

        return comments ?? Enumerable.Empty<CommentDTO>();
    }

// --------------------------------------------------------------------------

    // -- Get Single Comment By Id Async --
    public async Task<CommentDTO> GetSingleCommentByIdAsync(int id)
    {
        HttpResponseMessage httpResponse =
            await httpClient.GetAsync($"comments/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<CommentDTO>(response,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true })!;
    }

// --------------------------------------------------------------------------

    // -- Delete Comment Async --
    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage httpResponse =
            await httpClient.DeleteAsync($"comments/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }
// --------------------------------------------------------------------------
}