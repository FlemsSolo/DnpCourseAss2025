using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    // Where Do We Store The Comments
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        // If Theres No File We Make An Empty JSON
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    
    public async Task<Comment> AddAsync(Comment comment)
    {
        // Read Content Of File Repo DeSerialize
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        
        // If We Have Comments In Repofile. Find Max Id And Add 1 else Id = 1
        comment.Id = comments.Any() ? comments.Max(p => p.Id) + 1 : 1;
        
        // Add Comment To List
        comments.Add(comment);
        
        // Serialize And Read Back To File Repo
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
        
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        
        // Find Id of existingComment else throw Exeption
        Comment? existingComment = comments.SingleOrDefault(p => p.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException($"Kommentar med ID '{comment.Id}' ikke fundet");
        }

        comments.Remove(existingComment);
        comments.Add(comment);
        
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
        
        // No Need To Return Anything ?
    }

    public async Task DeleteAsync(int id)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        
        // Find Id of commentToRemove else throw Exeption
        Comment? commentToRemove = comments.SingleOrDefault(p => p.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Kommentar med ID '{id}' ikke fundet");
        }

        comments.Remove(commentToRemove);
        
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
        
        // No Need To Return Anything ?
        }

    public async Task<Comment> GetSingleAsync(int id)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        
        // Find Id of Single comment else throw Exeption
        Comment? comment = comments.SingleOrDefault(p => p.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Kommentar med ID '{id}' ikke fundet");
        }

        return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        // Not able to await a Task. Instead, you can call Result on a task
        string commentsAsJson = File.ReadAllText(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        
        return comments.AsQueryable();
    }

    // Not Implemented Yet ! ------------------------------------------------
    public async Task<List<Comment>> ReadListFromFile()
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        return comments;
    }

    // Not Implemented Yet ! ------------------------------------------------
    public async Task WriteListToFile(List<Comment> comments)
    {
        string commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

}