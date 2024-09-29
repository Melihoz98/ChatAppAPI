using ChatAppAPI.Hub;
using ChatAppAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Added Swagger Gen

builder.Services.AddSignalR();
builder.Services.AddSingleton<IDictionary<string, UserRoomConnection>>(opt =>
    new Dictionary<string, UserRoomConnection>());

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // Important for SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Apply the CORS policy
app.UseCors();

app.UseEndpoints(endpoints =>
{
    // Map SignalR hubs
    endpoints.MapHub<ChatHub>("/chat");

    // Map controller routes
    endpoints.MapControllers();
});

app.Run();
