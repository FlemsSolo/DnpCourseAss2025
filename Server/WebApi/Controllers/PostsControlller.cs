using ApiContracts_DTO;
using Entities;
using FileRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IMemoryCache cache;

    public PostsController(IPostRepository postRepository,
        ICommentRepository commentRepository, IUserRepository userRepository,
        IMemoryCache cache)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.cache = cache;
    }
// --------------------------------------------------------------------------

    private void CacheInvalidate(int id)
    {
        cache.Remove($"post-{id}Include");
        cache.Remove($"post-{id}Includecomments");
        cache.Remove("allPosts");
    }
// --------------------------------------------------------------------------

    // -- Create Post
    [HttpPost]
    public async Task<ActionResult<PostDTO>> CreatePost(
        [FromBody] CreatePostDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                $"Title is required and cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new ArgumentException(
                $"Body is required and cannot be empty");
        }
        
        Post post = new(0, request.Title, request.Body, request.SubForumId, request.UserId);
        Post created = await postRepository.AddAsync(post);
        User author = await userRepository.GetSingleAsync(created.UserId);

        // Cache invalidation
        CacheInvalidate(created.Id);

        PostDTO dto = new()
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId,
            Author = new UserDTO { Id = author.Id, Username = author.Name }
        };
        return Created($"/Posts/{dto.Id}", dto);
    }
// --------------------------------------------------------------------------

    // -- Update Post
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDTO>> UpdatePost(int id,
        [FromBody] UpdatePostDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                $"Title is required and cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new ArgumentException(
                $"Body is required and cannot be empty");
        }
        
        var post = await postRepository.GetSingleAsync(id);

        // Only Title and Body updates
        var updatedPost = new Post(post.Id, request.Title, request.Body,
            post.SubForumId, post.UserId);

        await postRepository.UpdateAsync(updatedPost);

        // Cache invalidation
        CacheInvalidate(id);

        var author = await userRepository.GetSingleAsync(updatedPost.UserId);

        var dto = new PostDTO
        {
            Id = updatedPost.Id,
            Title = updatedPost.Title,
            Body = updatedPost.Body,
            UserId = updatedPost.UserId,
            Author = new UserDTO { Id = author.Id, Username = author.Name }
        };

        return Ok(dto);
    }
// --------------------------------------------------------------------------

    // -- Get Single Post
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSinglePostById(int id,
        [FromQuery] string? include)
    {
        string cacheKey = $"post-{id}Include{include?.ToLower()}";

        // cache check
        if (cache.TryGetValue(cacheKey, out object? cachedResult))
        {
            return Ok(cachedResult);
        }


        var post = await postRepository.GetSingleAsync(id);
        var author = await userRepository.GetSingleAsync(post.UserId);


        object dtoToCache;

        if (include != null &&
            include.Contains("comments", StringComparison.OrdinalIgnoreCase))
        {
            var comments = commentRepository
                .GetMany()
                .Where(c => c.PostId == id)
                .ToList();
            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var users = userRepository.GetMany()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new UserDTO()
                    { Id = u.Id, Username = u.Name })
                .ToList();

            dtoToCache = new PostWithCommentsDTO()
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId,
                Author = new UserDTO
                    { Id = author.Id, Username = author.Name },
                Comments = comments.Select(c => new CommentDTO
                {
                    Id = c.Id, Body = c.Body, PostId = c.PostId,
                    UserId = c.UserId,
                    Author = users.FirstOrDefault(u => u.Id == c.UserId)
                })
                .ToList()
            };
        }
        else
        {
            dtoToCache = new PostDTO()
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId,
                Author = new UserDTO
                    { Id = author.Id, Username = author.Name }
            };
        }

        cache.Set(cacheKey, dtoToCache, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(2),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return Ok(dtoToCache);
    }
// --------------------------------------------------------------------------

    // Test GET endpoint : http://localhost:xxxx
    [HttpGet("/")]
    public IActionResult GetWelcomeMessage()
    {
        return Ok("Hello, I am a PostClass Controller-based API!");
    }
// --------------------------------------------------------------------------

    // -- Get Many Posts
    // -- GET Many Posts : endpoint to retrieve all posts : http://localhost:5274/posts?title=B&userid=username&userid=authorname

    [HttpGet]
    public async Task<ActionResult<PostDTO>> GetPosts(
        [FromQuery] string? title,
        [FromQuery] int? userid,
        [FromQuery] string? authorName)
    {
        string allPostsCacheKey = "allPosts";

        if (!cache.TryGetValue(allPostsCacheKey,
                out IEnumerable<Post>? cachedPosts))
        {
            cachedPosts = postRepository
                .GetMany()
                .ToList();
            
            cache.Set(allPostsCacheKey, cachedPosts,
                new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        var filteredPosts = cachedPosts;

        // Filters
        
        // Filter title -------------------------
        if (!string.IsNullOrWhiteSpace(title))
        {
            filteredPosts = filteredPosts
                .Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        // Filter userid -----------------------
        if (userid.HasValue)
        {
            filteredPosts = filteredPosts
                .Where(p => p.UserId == userid.Value);
        }

        var userIds =
            filteredPosts
                .Select(p => p.UserId)
                .Distinct()
                .ToList(); // no duplicates
        
        // Map to UserDTO
        var users = userRepository
            .GetMany()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserDTO()
                { Id = u.Id, Username = u.Name })
            .ToList();

        // Filter autherName ------------------------
        if (!string.IsNullOrWhiteSpace(authorName))
        {
            var author = users.FirstOrDefault(u => u.Username == authorName);
            if (author != null)
            {
                filteredPosts = filteredPosts.Where(p => p.UserId == author.Id);
            }
            else
            {
                filteredPosts = Enumerable.Empty<Post>();
            }
        }

        // Map to PostDTO using LINQ
        var posts = filteredPosts
            .Select(p => new PostDTO
                {
                Id = p.Id, Title = p.Title, Body = p.Body, UserId = p.UserId,
                Author = users.FirstOrDefault(u => u.Id == p.UserId)
                })
            .ToList();

        return Ok(posts);
    }
// --------------------------------------------------------------------------

    // -- Delete Post
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        await postRepository.DeleteAsync(id);

        CacheInvalidate(id);

        return NoContent();
    }
// --------------------------------------------------------------------------
}