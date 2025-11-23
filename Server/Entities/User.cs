namespace Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Pw { get; set; }
    
    private User(){} // Private NoArgs Constructor Used In EFC (Entity Framework Core)
    
    public User(int id, string username, string password)
    {
        Id = id;
        Name = username;
        Pw = password;
    }
}