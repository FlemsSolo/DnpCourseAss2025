using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class DeleteUserView
{
    private readonly IUserRepository userRepository;

    public DeleteUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task DeleteUserAsync()
    {
        Console.Write("Enter user id for user to delete: ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out var userId))
        {
            Console.WriteLine("Invalid user id.");
            return;
        }

        await userRepository.DeleteAsync(userId);
        Console.WriteLine("user deleted successfully");
    }
}