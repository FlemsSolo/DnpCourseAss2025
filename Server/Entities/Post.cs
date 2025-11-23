namespace Entities;

public class Post
{
    // Auto-implemented properties for trivial get and set
    public int Id { get; set; }
    public string Title { get; set; }
    
    public string Body { get; set; }
    public int SubForumId { get; set; }
    public int UserId { get; set; }

    private Post(){} // Private NoArgs Constructor Used In EFC (Entity Framework Core)
    
    // Constructor
    public Post(int id, string title, string body, int subForumId, int userId)
    {
        Id = id;
        Title = title;
        Body = body;
        SubForumId = subForumId;
        UserId = userId;
    }
}