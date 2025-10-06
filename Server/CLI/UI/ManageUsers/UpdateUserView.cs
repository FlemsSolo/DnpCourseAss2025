using Entities ;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class UpdateUserView
{
    private readonly IUserRepository userRepository;

    public UpdateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task UpdateUserAsync()
    {
        User? user = null;
        Console.WriteLine("UPDATE USER");
        
        while (true)
        {
        Console.Write("Enter user id to edit: ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out var userId))
        {
            Console.WriteLine("Invalid user id.");
            continue;
        }

        try
        {
        user = await userRepository.GetSingleAsync(userId);
        break;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }
        }

        Console.Write("New Username: ");
        string? newUsername = Console.ReadLine();

        user.Name = newUsername;

        Console.Write("New password: ");
        string? newPassword = Console.ReadLine();

        user.Pw = newPassword;

        await userRepository.UpdateAsync(user);
        Console.WriteLine($"User {user.Id} updated successfully");
    }
}