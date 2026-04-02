using RobotArm.Comms;

namespace RobotArm.API.Service;

public class RobotConnectionService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Connecting to robot...");
        await _client.Connect("ws://10.0.0.35");
        Console.WriteLine("Connected!");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Disconnecting from robot...");
        return Task.CompletedTask;
    }
    
    #region Fields
    
    private readonly ArduinoClient _client;

    public RobotConnectionService(ArduinoClient client)
    {
        _client = client;
    }
    
    #endregion
}