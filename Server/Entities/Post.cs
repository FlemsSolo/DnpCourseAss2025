namespace Entities;

public class Post
{
    // Auto-implemented properties for trivial get and set
    public int Id { get; set; }
    public string Title { get; set; }
    public int SubForumId { get; set; }
    public int UserId { get; set; }

    // Constructor
    public Post(int Id, string Title, int SubForumId, int UserId)
    {
        this.Id = Id;
        this.Title = Title;
        this.SubForumId = SubForumId;
        this.UserId = UserId;
    }
}