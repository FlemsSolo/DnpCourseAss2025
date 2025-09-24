using CLI.UI;
//using InMemoryRepositories;
using FileRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI App ...");

IUserRepository userRepository = new UserFileRepository() ; // UserInMemoryRepository() ;
IPostRepository postRepository = new PostFileRepository() ; // PostInMemoryRepository() ;
ICommentRepository commentRepository = new CommentFileRepository() ; // CommentInMemoryRepository() ;

// Repositories Passed to CLI_App
CLI_App cli_App = new CLI_App(userRepository, commentRepository, postRepository) ;

await cli_App.StartAsync() ; // Calls CLI_App.cs Which Contains Main Menu