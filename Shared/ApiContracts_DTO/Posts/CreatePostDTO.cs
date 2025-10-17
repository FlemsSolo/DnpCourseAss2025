namespace ApiContracts_DTO;

public class CreatePostDTO
{
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required int SubForumId { get; set; }
    public required int subForumId { get; set; }
    public required int UserId { get; set; }
}