using System.ComponentModel.DataAnnotations;

namespace ApiContracts_DTO;

public class CreatePostDTO
{
    [Required(ErrorMessage = "Titel er nødvendig og kan ikke være tom.")]
    public required string Title { get; set; }
    
    [Required(ErrorMessage = "Indhold er nødvendig og kan ikke være tom.")]
    public required string Body { get; set; }
    public required int SubForumId { get; set; }
    //public required int subForumId { get; set; }
    public required int UserId { get; set; }
}