using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class DeletePostView
{
    private readonly IPostRepository postRepository;

    public DeletePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task DeletePostAsync()
    {
        Console.Write("Enter post id for post to delete: ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int postId))
        {
            Console.WriteLine("Invalid post id.");
            return;
        }

        await postRepository.DeleteAsync(postId);
        Console.WriteLine("post deleted successfully");
    }
}