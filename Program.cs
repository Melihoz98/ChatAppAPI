using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using ChatAppAPI.Hub;
using ChatAppAPI;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddSingleton<IDictionary<string, UserRoomConnection>>(IServiceProvider =>
    new Dictionary<string, UserRoomConnection>());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
    
var app = builder.Build();
app.UseRouting();
app.UseCors();
app.UseEndpoints(endpoint =>
{
    endpoint.MapHub<ChatHub>("/chat");
});

app.Run();

