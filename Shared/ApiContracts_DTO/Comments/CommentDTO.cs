namespace ApiContracts_DTO;

public class CommentDTO
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }

    public UserDTO? Author { get; set; }
}