using Microsoft.AspNetCore.SignalR;
using RobotArm.API.Models;
using RobotArm.API.Service;
using RobotArm.Core;

namespace RobotArm.API.Hubs;

public class RobotHub : Hub
{
    private readonly RobotService _robot;
    
    public RobotHub(RobotService robot) => _robot = robot;

    public async Task MoveToPosition(MoveCommand cmd)
    {
        try
        {
            var target = new InverseKinematics.Vector3(
                cmd.X, cmd.Y, cmd.Z, 
                cmd.Pitch, cmd.Yaw, cmd.Grip
            );
            
            var status = InverseKinematics.checkSingularity(target);
            
            if (status is InverseKinematics.ArmStatus.NearFullExtension ext)
                await Clients.Caller.SendAsync("Warning", $"El brazo esta casi extendido ({ext.Item * 100:F1}%)");
            
            var (angles, fkError) = InverseKinematics.solveVerified(target, 1);
            await _robot.SendAngles(angles, cmd);
            
            await Clients.Caller.SendAsync("ArmState", new ArmStateDto
            {
                BaseDeg = angles.Base * 180.0 / Math.PI,
                ShoulderDeg = angles.Shoulder * 180.0 / Math.PI,
                ElbowDeg = angles.Elbow * 180.0 / Math.PI,
                WristPitchDeg = angles.WristPitch * 180.0 / Math.PI,
                WristYawDeg = angles.WristYaw * 180.0 / Math.PI,
                Grip = angles.Grip,
                FkError = fkError,
                X = cmd.X, Y = cmd.Y, Z = cmd.Z
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public async Task JoystickDelta(JoystickCommand delta)
    {
        try
        {
            var current = _robot.CurrentPosition();

            var next = current with
            {
                X = Math.Clamp(current.X + delta.Dx * delta.Speed, -15, 15),
                Y = Math.Clamp(current.Y + delta.Dy * delta.Speed, -15, 15),
                Z = Math.Clamp(current.Z + delta.Dz * delta.Speed, -4, 20),
                Pitch = Math.Clamp(current.Pitch + delta.DPich * delta.Speed, -90, 90),
                Yaw = Math.Clamp(current.Yaw + delta.DYaw * delta.Speed, -90, 90),
                Grip = Math.Clamp(current.Grip + delta.DGrip, 0, 110)
            };
            
            await MoveToPosition(new MoveCommand
            {
                X = next.X, Y = next.Y, Z = next.Z,
                Pitch = next.Pitch, Yaw = next.Yaw, Grip = next.Grip
            });
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }
    
    public async Task GetState()
    {
        var pos = _robot.CurrentPosition();
        await Clients.Caller.SendAsync("CurrentPosition", pos);
    }
}
