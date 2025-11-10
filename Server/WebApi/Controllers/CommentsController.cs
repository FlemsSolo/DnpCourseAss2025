using ApiContracts_DTO;
using Entities;
using RepositoryContracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly IMemoryCache cache;

    public CommentsController(ICommentRepository commentRepository,
        IUserRepository userRepository, IPostRepository postRepository,
        IMemoryCache cache)
    {
        // Dependency Injection
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        
        this.cache = cache;
    }
// --------------------------------------------------------------------------
    private void CacheInvalidate(int id)
    {
        cache.Remove($"allComments-{id}");
        cache.Remove("allComments");
    }
// --------------------------------------------------------------------------

    // -- Create Comment --
    
    [HttpPost]
    public async Task<ActionResult<CommentDTO>> CreateComment(
        [FromBody] CreateCommentDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new ArgumentException(
                $"Indhold skal udfyldes !");
        }
        
        User author = await userRepository.GetSingleAsync(request.UserId);
        await postRepository.GetSingleAsync(request.PostId);
        Comment comment = new(0, request.Body, request.PostId, request.UserId);
        Comment created = await commentRepository.AddAsync(comment);


        // Cache invalidation
        CacheInvalidate(created.Id);

        CommentDTO dto = new()
        {
            Id = created.Id,
            Body = created.Body,
            PostId = created.PostId,
            UserId = created.UserId,
            Author = new UserDTO() { Id = author.Id, Username = author.Name }
        };
        
        return Created($"/Comments/{dto.Id}", dto);
    }
// --------------------------------------------------------------------------

    // -- Update Comment --
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDTO>> UpdateComment(int id,
        [FromBody] UpdateCommentDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new ArgumentException(
                $"Indhold skal udfyldes !");
        }
        var comment = await commentRepository.GetSingleAsync(id);
        
        // Only Body updates
        var updatedComment = new Comment(comment.Id, request.Body,
            comment.PostId, comment.UserId);
        await commentRepository.UpdateAsync(updatedComment);

        // Cache invalidation
        CacheInvalidate(id);

        var author =
            await userRepository.GetSingleAsync(updatedComment.UserId);

        var dto = new CommentDTO()
        {
            Id = updatedComment.Id,
            Body = updatedComment.Body,
            PostId = updatedComment.PostId,
            UserId = updatedComment.UserId,
            Author = new UserDTO
                { Id = author.Id, Username = author.Name }
        };

        return Ok(dto);
    }
// --------------------------------------------------------------------------

    // -- Get Single Comment --
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSingleCommentById(int id)
    {
        string cacheKey = $"comment-{id}";

        // cache check
        if (cache.TryGetValue(cacheKey, out CommentDTO? cachedResult))
        {
            return Ok(cachedResult);
        }

        var comment = await commentRepository.GetSingleAsync(id);
        var author = await userRepository.GetSingleAsync(comment.UserId);

        var commentDto = new CommentDTO
        {
            Id = comment.Id, Body = comment.Body, PostId = comment.PostId,
            UserId = comment.UserId, Author = new UserDTO
                { Id = author.Id, Username = author.Name }
        };

        cache.Set(cacheKey, commentDto, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(2),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return Ok(commentDto);
    }
// --------------------------------------------------------------------------

    // -- GetMany Comments --
    
    [HttpGet]
    public async Task<ActionResult<CommentDTO>> GetComments(
        [FromQuery] int? userid,
        [FromQuery] string? authorName,
        [FromQuery] int? postid,
        [FromQuery] string? sortBy)
    {
        string allCommentsCacheKey = "allComments";

        if (!cache.TryGetValue(allCommentsCacheKey,
                out IEnumerable<Comment>? cachedComments))
        {
            cachedComments = commentRepository
                .GetMany()
                .ToList();
            
            cache.Set(allCommentsCacheKey, cachedComments,
                new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        var filteredComments = cachedComments;

        // -- Filters --
        
        // Filter userid -----------------------------
        if (userid.HasValue)
        {
            filteredComments =
                filteredComments.Where(c => c.UserId == userid.Value);
        }

        // Filter postid --------------------------------
        if (postid.HasValue)
        {
            filteredComments =
                filteredComments.Where(c => c.PostId == postid.Value);
        }

        var userIds =
            filteredComments.Select(c => c.UserId).Distinct(); // no duplicates
        // Map to UserDTO
        var users = userRepository
            .GetMany()
            .Where(u => userIds
                .Contains(u.Id))
                .Select(u => new UserDTO()
                    { Id = u.Id, Username = u.Name })
            .ToList();

        // Filter authorName -------------------------------------
        if (!string.IsNullOrWhiteSpace(authorName))
        {
            var author = users.FirstOrDefault(u => u.Username == authorName);
            if (author != null)
            {
                filteredComments =
                    filteredComments.Where(c => c.UserId == author.Id);
            }
            else
            {
                filteredComments = Enumerable.Empty<Comment>();
            }
        }

        // Filter sortBy ----------------------------------------
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            switch (sortBy.ToLowerInvariant())
            {
                case "userid":
                    filteredComments = filteredComments.OrderBy(c => c.UserId);
                    break;
                case "postid":
                    filteredComments = filteredComments.OrderBy(u => u.PostId);
                    break;
            }
        }

        // Map to CommentDTO using LINQ
        var comments = filteredComments
            .Select(c => new CommentDTO()
            {
                Id = c.Id, Body = c.Body, PostId = c.PostId, UserId = c.UserId,
                Author = users.FirstOrDefault(u => u.Id == c.UserId)
            })
            .ToList();

        return Ok(comments);
    }
// --------------------------------------------------------------------------

    // -- Delete Comment --
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        await commentRepository.DeleteAsync(id);

        CacheInvalidate(id);

        return NoContent();
    }
// --------------------------------------------------------------------------
}