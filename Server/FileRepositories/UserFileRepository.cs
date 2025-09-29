using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    // Where Do We Store The Users
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        // If Theres No File We Make An Empty JSON
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    
    public async Task<User> AddAsync(User user)
    {Console.WriteLine("\n-->Trying To AddAsync()\n");
        // Read Content Of File Repo DeSerialize
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users =
            JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        
        // If We Have Users In Repofile. Find Max Id And Add 1 else Id = 1
        user.Id = users.Any() ? users.Max(p => p.Id) + 1 : 1;
        
        // Add User To List
        users.Add(user);
        
        // Serialize And Read Back To File Repo
        usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);Console.WriteLine("\n\n-->Added Via AddAsync()\n");
        
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users =
            JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        
        // Find Id of existingUser else throw Exeption
        User? existingUser = users.SingleOrDefault(p => p.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);
        
        usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
        
        // No Need To Return Anything ?
    }

    public async Task DeleteAsync(int id)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users =
            JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        
        // Find Id of userToRemove else throw Exeption
        User? userToRemove = users.SingleOrDefault(p => p.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);
        
        usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
        
        // No Need To Return Anything ?
        }

    public async Task<User> GetSingleAsync(int id)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users =
            JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        
        // Find Id of Single user else throw Exeption
        User? user = users.SingleOrDefault(p => p.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        return user;
    }

    public IQueryable<User> GetMany()
    {
        // Not able to await a Task. Instead, you can call Result on a task
        string usersAsJson = File.ReadAllText(filePath);Console.WriteLine($"\n\n-->Got Name {usersAsJson}\n");
        List<User> users =
            JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();
        
        return users.AsQueryable();
    }

    // Not Implemented Yet !
    public async Task<List<User>> ReadListFromFile()
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users =
            JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        return users;
    }

    // Not Implemented Yet !
    public async Task WriteListToFile(List<User> users)
    {
        string usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
    }

}