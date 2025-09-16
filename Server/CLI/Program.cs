using CLI.UI;
using InMemoryRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI App ...");

IUserRepository userRepository = new UserInMemoryRepository() ;
IPostRepository postRepository = new PostInMemoryRepository() ;
ICommentRepository commentRepository = new CommentInMemoryRepository() ;

CLI_App cli_App = new CLI_App(userRepository, commentRepository, postRepository) ;

await cli_App.StartAsync() ;