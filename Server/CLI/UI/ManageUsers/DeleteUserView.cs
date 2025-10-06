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
        while (true)
        {
            Console.Write("Enter user id for user to delete: ");
            string? input = Console.ReadLine();
            if (!int.TryParse(input, out var userId))
            {
                Console.WriteLine("Invalid user id.");
                continue; // Try With Another UserId
            }

            try
            {
                await userRepository.DeleteAsync(userId);
                Console.WriteLine("user deleted successfully");
                break;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"---> Cannot Delete UserId {userId} : \n {e.Message}");
                break ;
            }
        }
    }
}