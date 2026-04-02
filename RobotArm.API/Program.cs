using RobotArm.API.Hubs;
using RobotArm.API.Service;
using RobotArm.Comms;

var builder = WebApplication.CreateBuilder(args);

// SignalR
builder.Services.AddSignalR(opts =>
{
    opts.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    opts.KeepAliveInterval = TimeSpan.FromSeconds(10);
    opts.MaximumReceiveMessageSize = 32 * 1024;
});

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(p => 
        p.WithOrigins(
                "http://localhost:4321",
                "http://localhost:3000",
                "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    )
);

builder.Services.AddSingleton<ArduinoClient>();
builder.Services.AddSingleton<RobotService>();

// Add services to the container.
var app = builder.Build();

app.UseCors();
app.MapHub<RobotHub>("/hubs/robot");

app.Run();