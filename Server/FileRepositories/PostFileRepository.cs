using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    // Where Do We Store The Posts
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        // If Theres No File We Make An Empty JSON
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    
    public async Task<Post> AddAsync(Post post)
    {
        // Read Content Of File Repo DeSerialize
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        
        // If We Have Posts In Repofile. Find Max Id And Add 1 else Id = 1
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        
        // Add Post To List
        posts.Add(post);
        
        // Serialize And Read Back To File Repo
        postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
        
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        
        // Find Id of existingPost else throw Exeption
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);
        
        postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
        
        // No Need To Return Anything ?
    }

    public async Task DeleteAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        
        // Find Id of postToRemove else throw Exeption
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        
        postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
        
        // No Need To Return Anything ?
        }

    public async Task<Post> GetSingleAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        
        // Find Id of Single post else throw Exeption
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        return post;
    }

    public IQueryable<Post> GetMany()
    {
        // Not able to await a Task. Instead, you can call Result on a task
        string postsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        
        return posts.AsQueryable();
    }

    public async Task<List<Post>> ReadListFromFile()
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        return posts;
    }


    public async Task WriteListToFile(List<Post> posts)
    {
        string postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }

}