using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task AddUserAsync()
    {
        Console.WriteLine("CREATE USER MENU");
        string username;

        while (true)
        {
            Console.Write("Username: ");
            username = Console.ReadLine();

            // Check for username already taken
            var usersAlreadyInList = userRepository.GetMany();Console.WriteLine("\n\n-->Got Name\n");

            var userWithSameUsername =
                usersAlreadyInList.FirstOrDefault(userInList =>
                    userInList.Name == username);

            if (userWithSameUsername != null)
                Console.WriteLine("Username already taken");
            else if (string.IsNullOrWhiteSpace(username))
                Console.WriteLine("Username cannot be empty");
            else
                break; // valid username
        }

        string password;

        while (true)
        {
            Console.Write("Password: ");
            password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(password))
                Console.WriteLine("Password cannot be empty");
            else
                break; // valid password
        }

        var user = new User(0, username, password);
        var created = await userRepository.AddAsync(user);
        Console.WriteLine(
            $"User ({created.Name}) with id {created.Id} successfully created");
    }
}