using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

// You can view a single post and afterward add/edit/delete a comment on the post
public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public SinglePostView(IPostRepository postRepository,
        IUserRepository userRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    public async Task GetSinglePostAsync()
    {
        Console.WriteLine("SINGLE POST MENU");
        Console.Write("Enter post id:");

        var input = Console.ReadLine();

        // Is It Numeric ?
        if (!int.TryParse(input, out var id))
        {
            Console.WriteLine("Invalid post id.");
            return;
        }

        var post = await postRepository.GetSingleAsync(id);
        if (post == null)
        {
            Console.WriteLine("Post not found");
            return;
        }

        Console.WriteLine($"Title: {post.Title}\nBody: {post.Body}");

        await ShowCommentsAsync(id);

        while (true)
        {
            Console.WriteLine("COMMENTS MENU");
            Console.WriteLine("1. Add comment");
            Console.WriteLine("2. Edit comment");
            Console.WriteLine("3. Delete comment");
            Console.WriteLine("0. Back");
            Console.Write("Choice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await AddCommentAsync(id);
                    await ShowCommentsAsync(id);
                    break;
                case "2":
                    await EditCommentAsync(id);
                    await ShowCommentsAsync(id);
                    break;
                case "3":
                    await DeleteCommentAsync(id);
                    await ShowCommentsAsync(id);
                    break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }

    private async Task ShowCommentsAsync(int postId)
    {
        var comments = commentRepository.GetMany()
            .Where(comment => comment.PostId == postId);

        Console.WriteLine("Comments:");
        foreach (var comment in comments)
        {
            var user = await userRepository.GetSingleAsync(comment.UserId);
            Console.WriteLine($"{comment.Body} (by {user.Name})");
        }
    }

    private async Task AddCommentAsync(int postId)
    {
        Console.Write("Enter user id: ");
        var input = Console.ReadLine();
        
        if (!int.TryParse(input, out var userId))
        {
            Console.WriteLine("Invalid user id.");
            return;
        }

        // Maybe Put In Try Catch Block To Catch InvalidOperationException On GetSingleAsync
        var userIdInList = await userRepository.GetSingleAsync(userId);
        
        if (userIdInList == null)
        {
            Console.WriteLine("User id does not exist");
            return;
        }

        Console.Write("Comment body: ");
        var body = Console.ReadLine();

        var comment = new Comment(0, body, postId, userId);
        var created = await commentRepository.AddAsync(comment);
        Console.WriteLine($"Comment with id {created.Id} successfully created");
    }

    private async Task EditCommentAsync(int postId)
    {
        Console.Write("Enter comment id to edit: ");
        var input = Console.ReadLine();
        
        if (!int.TryParse(input, out var commentId))
        {
            Console.WriteLine("Invalid comment id.");
            return;
        }

        var comment = await commentRepository.GetSingleAsync(commentId);
        if (comment == null ||
            comment.PostId !=
            postId) // If the comment isn't posted on this post
        {
            Console.WriteLine("Comment not found");
            return;
        }

        Console.Write("New body: ");
        var newBody = Console.ReadLine();

        comment.Body = newBody;

        await commentRepository.UpdateAsync(comment);
        Console.WriteLine("Comment updated successfully");
    }

    private async Task DeleteCommentAsync(int postId)
    {
        Console.Write("Enter comment id to delete: ");
        var input = Console.ReadLine();
        
        if (!int.TryParse(input, out var commentId))
        {
            Console.WriteLine("Invalid comment id.");
            return;
        }

        var comment = await commentRepository.GetSingleAsync(commentId);
        if (comment == null || comment.PostId != postId) // If the comment isn't posted on this post
        {
            Console.WriteLine("Comment not found");
            return;
        }

        await commentRepository.DeleteAsync(commentId);
        Console.WriteLine("Comment deleted successfully");
    }
}