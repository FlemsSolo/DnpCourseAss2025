using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class UpdatePostView
{
    private readonly IPostRepository postRepository;

    public UpdatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task UpdatePostAsync()
    {
        Console.WriteLine("UPDATE POST");
        Console.Write("Enter post id to edit: ");
        var input = Console.ReadLine();
        
        if (!int.TryParse(input, out var postId))
        {
            Console.WriteLine("Invalid post id.");
            return;
        }

        Post?  post = await postRepository.GetSingleAsync(postId);

        Console.Write("New title: ");
        string? newTitle = Console.ReadLine();

        post.Title = newTitle;

        Console.Write("New body: ");
        string? newBody = Console.ReadLine();

        post.Body = newBody;

        await postRepository.UpdateAsync(post);
        Console.WriteLine($"Post {post.Id} updated successfully");
    }
}