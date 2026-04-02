using RobotArm.API.Models;
using RobotArm.Comms;
using RobotArm.Core;
using RobotArm.Domain;

namespace RobotArm.API.Service;

public class RobotService
{
    private readonly ArduinoClient _client;
    private MoveCommand _currentPosition = new() { X = 8, Y = 10, Z = 8 };
    private readonly SemaphoreSlim _lock = new(1, 1);
    
    public RobotService(ArduinoClient client) => _client = client;
    
    public MoveCommand CurrentPosition() => _currentPosition;

    public async Task SendAngles(InverseKinematics.Angle angles)
    {
        await _lock.WaitAsync();
        try
        {
            var servo = ServoMapper.ToServo(new Angles
            {
                Base = angles.Base,
                Shoulder = angles.Shoulder,
                Elbow = angles.Elbow,
                WristPitch = angles.WristPitch,
                WristYaw = angles.WristYaw,
                Grip = angles.Grip
            });
            
            await _client.SendServos(servo);
        }
        finally
        {
            _lock.Release();
        }
    }
    
    public void UpdatePosition(MoveCommand moveCommand) => _currentPosition = moveCommand;
}