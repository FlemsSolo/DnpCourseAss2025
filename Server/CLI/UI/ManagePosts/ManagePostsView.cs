using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public ManagePostsView(IPostRepository postRepository,
        IUserRepository userRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    public async Task DisplayMenu()
    {
        while (true)
        {
            Console.WriteLine("\nMANAGE POSTS MENU");
            Console.WriteLine("1. Create Posts");
            Console.WriteLine("2. Update Post");
            Console.WriteLine("3. Delete Post");
            Console.WriteLine("4. View Posts Overview");
            Console.WriteLine("5. View Single Post");
            Console.WriteLine("0. Back");
            Console.Write("Choice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    var createPost = new CreatePostView(postRepository, userRepository);
                    await createPost.AddPostAsync();
                    break;
                case "2":
                    var updatePost = new UpdatePostView(postRepository);
                    await updatePost.UpdatePostAsync();
                    break;
                case "3":
                    var deletePost = new DeletePostView(postRepository); // Comment Rep. included In Future
                    await deletePost.DeletePostAsync();
                    break;
                case "4":
                    var listPosts = new ListPostsView(postRepository);
                    await listPosts.GetManyPosts();
                    break;
                case "5":
                    var singlePost = new SinglePostView(postRepository,
                        userRepository, commentRepository);
                    await singlePost.GetSinglePostAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }
}