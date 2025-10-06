namespace Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Pw { get; set; }
    
    // Public parameterless constructor required for System.Text.Json
    public User() {}
    /*public User(int id, string username, string password)
    {
        Id = id;
        Name = username;
        Pw = password;
    }*/
}