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
        Console.WriteLine("POSTS");

        foreach (var post in postRepository.GetMany())
            Console.WriteLine($"[{post.Title}, {post.Id}]");
    }
}