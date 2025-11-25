using System.Security.Claims;
using BlazorApp.Components;
using BlazorApp.Components.Authentication;
using BlazorApp.HttpServices;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp => new HttpClient 
{
    // Web API https value from WebApi/Properties/launchSettings.json
    // Notice : https protocol
    //BaseAddress = new Uri("https://localhost:7047") // This Adress Dont Work
    BaseAddress = new Uri("http://localhost:5274")
});

// Dependency Injection
// We must register all ”services”, which the Blazor App (HttpClient) controller can request
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthProvider>();

//builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddAuthorizationCore();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OG", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(claim =>
                claim.Type == ClaimTypes.NameIdentifier &&
                int.TryParse(claim.Value, out var id) && id <= 10)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios,
    // see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();