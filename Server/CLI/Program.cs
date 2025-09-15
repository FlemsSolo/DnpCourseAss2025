using CLI.UI;
using InMemoryRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI App ...");

IUserRepository userRepository = new UserInMemoryRepository() ;
ICommentRepository commentRepository = new CommentInMemoryRepository() ;
IPostRepository postRepository = new PostInMemoryRepository() ;

CLI_App cli_App = new CLI_App(userRepository, commentRepository, postRepository) ;
await cli_App.StartAsync() ;