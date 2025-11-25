using ApiContracts_DTO;

namespace ApiContracts_DTO;

public class LoginResponseDTO
{
    public UserDTO User { get; set; }
    
    // For web api JWT testing (httpie)
    public string Token { get; set; }
}