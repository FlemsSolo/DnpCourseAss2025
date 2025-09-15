using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task GetManyUsers()
    {
        Console.WriteLine("USERS");
        foreach (var user in userRepository.GetMany())
        {
            Console.WriteLine($"({user.Id}) {user.Name}");
        }
    }
}