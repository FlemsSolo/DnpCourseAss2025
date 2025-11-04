using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    List<User> users = new List<User>();
    
    // Constructor
    public UserInMemoryRepository()
    {
        // Create Initial Dummy Data Here
        //User UserOne = new User(1, "Mickey Moose", "Very Secret");
        //users.Add(UserOne);
        //User UserTwo = new User(2, "Minnie Moose", "Very Secret");
        //users.Add(UserTwo);
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any()
            ? users.Max(p => p.Id) + 1
            : 1;
        users.Add(user);
        return Task.FromResult(user);
    }
    
    public Task UpdateAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(p => p.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"Bruger med ID '{user.Id}' ikke fundet");
        }

        users.Remove(existingUser);
        users.Add(user);

        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(int id)
    {
        User? userToRemove = users.SingleOrDefault(p => p.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"Bruger med ID '{id}' ikke fundet");
        }

        users.Remove(userToRemove);
        return Task.CompletedTask;
    }
    
    public Task<User> GetSingleAsync(int id)
    {
        // Do implementation
        User? user = users.SingleOrDefault(p => p.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"Bruger med ID '{id}' ikke fundet");
        }

        return Task.FromResult(user);
    }
    
    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        User? user = users.SingleOrDefault(p => p.Name.Equals(username));
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with username '{username}' not found");
        }

        return await Task.FromResult(user);
    }

}