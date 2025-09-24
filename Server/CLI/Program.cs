using CLI.UI;
using InMemoryRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI App ...");

IUserRepository userRepository = new UserInMemoryRepository() ;
IPostRepository postRepository = new PostInMemoryRepository() ;
ICommentRepository commentRepository = new CommentInMemoryRepository() ;

// Repositories Passed to CLI_App
CLI_App cli_App = new CLI_App(userRepository, commentRepository, postRepository) ;

await cli_App.StartAsync() ; // Calls CLI_App.cs Which Contains Main Menu