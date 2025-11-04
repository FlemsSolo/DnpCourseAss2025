using System.ComponentModel.DataAnnotations;

namespace ApiContracts_DTO.Users;

public class UpdateUserDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Du skal udfylde BrugerNavn.")]
    public required string Username { get; set; }
    
    public string? Password { get; set; }    
}