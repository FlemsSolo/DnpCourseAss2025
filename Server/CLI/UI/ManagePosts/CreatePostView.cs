using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;

    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task AddPostAsync()
    {
        Console.WriteLine("CREATE POST");

        Console.Write("Title: ");
        var title = Console.ReadLine();

        Console.Write("Body: ");
        var body = Console.ReadLine();

        int forumId;
        while (true)
        {
            Console.Write("Forum id: ");
            var forumIdInput = Console.ReadLine();

            if (int.TryParse(forumIdInput,
                    out forumId)) break; // valid forum id

            Console.WriteLine("Invalid forum id");
        }

        int userId;
        while (true)
        {
            Console.Write("User id: ");
            var userIdInput = Console.ReadLine();

            if (int.TryParse(userIdInput, out userId)) break; // valid user id

            Console.WriteLine("Invalid user id");
        }

        if (string.IsNullOrWhiteSpace(title)) title = "Unknown title";

        if (string.IsNullOrWhiteSpace(body)) body = "Unknown body";

        var post = new Post(0, title, body, forumId,
            userId);
        var created = await postRepository.AddAsync(post);
        Console.WriteLine($"Post with id {created.Id} successfully created");
    }
}