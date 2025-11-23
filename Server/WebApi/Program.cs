using System.Security.Claims;
using System.Text;

//using FileRepositories;
using EfcRepositories;

using RepositoryContracts;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

//using WebApi;
using AppContext = EfcRepositories.AppContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Dependency Injection
// We must register all ”services”, which a Web API controller can request
//builder.Services.AddScoped<IPostRepository, PostFileRepository>();
//builder.Services.AddScoped<IUserRepository, UserFileRepository>();
//builder.Services.AddScoped<ICommentRepository, CommentFileRepository>();
//builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();

builder.Services.AddScoped<IPostRepository, EfcPostRepository>();
builder.Services.AddScoped<IUserRepository, EfcUserRepository>();
builder.Services.AddScoped<ICommentRepository, EfcCommentRepository>();
//builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();
builder.Services.AddScoped<AppContext>();

/*builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

// Add JWT authentication For web api JWT testing (httpie)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "SuperSecretKeyThatIsAtMinimum32CharactersLong"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OG", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(claim =>
                claim.Type == ClaimTypes.NameIdentifier &&
                int.TryParse(claim.Value, out var id) && id <= 10)));
});
*/

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
//app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

//app.UseAuthentication();
//app.UseAuthorization();

app.Run();