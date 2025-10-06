//using CLI.UI;
using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using RepositoryContracts;

namespace CLI.UI;

public class CLI_App
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CLI_App(IUserRepository userRepository, IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("MAIN MENU --------------");
            Console.WriteLine("1. Manage Users");
            Console.WriteLine("2. Manage Posts");
            //Console.WriteLine("3. Manage Comments");
            Console.WriteLine("0. Exit");
            Console.Write("Enter Choice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    var userView = new ManageUsersView(userRepository);
                    await userView.DisplayMenu();
                    break;
                case "2":
                    var postView = new ManagePostsView(userRepository, postRepository,
                        commentRepository);
                    await postView.DisplayMenu();
                    break;
                /*case "3":
                    var commentView = new ManageCommentView(commentRepository,
                        userRepository, commentRepository);
                    commentView.DisplayMenu();
                    break;*/
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }
}