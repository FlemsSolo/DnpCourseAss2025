using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task AddPostAsync()
    {
        Console.WriteLine("CREATE POST");

        Console.Write("Title: ");
        string? title = Console.ReadLine();

        Console.Write("Body: ");
        string? body = Console.ReadLine();

        int forumId;
        while (true)
        {
            Console.Write("Forum id: ");
            var forumIdInput = Console.ReadLine();

            // Is It Numeric ?
            if (int.TryParse(forumIdInput, out forumId)) 
                break; // valid forum id

            Console.WriteLine("Invalid forum id");
        }

        int userId;
        while (true)
        {
            Console.Write("User id: ");
            var userIdInput = Console.ReadLine();

            // Is It Numeric ?
            if (!int.TryParse(userIdInput, out userId))
            {
                Console.WriteLine("Invalid user id");
                continue; // invalid user id
            }

            try
            {
                await userRepository.GetSingleAsync(userId);
                break;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        if (string.IsNullOrWhiteSpace(title)) title = "Unknown title";

        if (string.IsNullOrWhiteSpace(body)) body = "Unknown body";

        var post = new Post(0, title, body, forumId, userId);
        
        // AddAsync Takes Care Of The post.id == 0 And Gives It A Proper ID
        var created = await postRepository.AddAsync(post);
        Console.WriteLine($"Post with id {created.Id} successfully created");
    }
}