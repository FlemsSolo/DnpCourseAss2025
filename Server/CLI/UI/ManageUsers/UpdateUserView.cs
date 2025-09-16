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
        Console.WriteLine("UPDATE USER");
        Console.Write("Enter user id to edit: ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out var userId))
        {
            Console.WriteLine("Invalid user id.");
            return;
        }

        var user = await userRepository.GetSingleAsync(userId);

        Console.Write("New Username: ");
        var newUsername = Console.ReadLine();

        user.Name = newUsername;

        Console.Write("New password: ");
        var newPassword = Console.ReadLine();

        user.PW = newPassword;

        await userRepository.UpdateAsync(user);
        Console.WriteLine($"User {user.Id} updated successfully");
    }
}