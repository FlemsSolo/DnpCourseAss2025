using System.ComponentModel.DataAnnotations;
 
namespace ApiContracts_DTO.Users;

public class CreateUserDTO
{
    [Required(ErrorMessage = "BrugerNavn skal udfyldes.")]
    public required string Username { get; set; }
    
    [Required(ErrorMessage = "Password skal udfyldes.")]
    public required string Password { get; set; }
}