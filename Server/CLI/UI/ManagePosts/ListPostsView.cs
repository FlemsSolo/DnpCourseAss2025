using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task GetManyPosts()
    {
        Console.WriteLine("\nPOSTS ------------");

        foreach (var post in postRepository.GetMany())
            Console.WriteLine($"[{post.Id}, {post.Title}, UserId {post.UserId}]");
        
        Console.WriteLine("------------------");
    }
}