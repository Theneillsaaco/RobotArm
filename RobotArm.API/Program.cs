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
    opts.EnableDetailedErrors = true;
});

builder.Services.AddSingleton<ArduinoClient>();
builder.Services.AddSingleton<RobotService>();

builder.Services.AddHostedService<RobotConnectionService>();

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(p => 
        p.SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    )
);

// Add services to the container.
var app = builder.Build();

app.UseWebSockets();
app.UseCors();
app.MapHub<RobotHub>("/hubs/robot");

app.Run();