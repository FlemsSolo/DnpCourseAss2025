namespace ApiContracts_DTO;

public class PostDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int SubForumId { get; set; }
    public int UserId { get; set; }
    public int subForumId { get; set; }
    public UserDTO? Author { get; set; }
}