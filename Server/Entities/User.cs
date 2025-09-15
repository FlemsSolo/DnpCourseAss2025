namespace Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PW { get; set; }
    
    public User(int id, string username, string password)
    {
        Id = id;
        Name = username;
        PW = password;
    }

}