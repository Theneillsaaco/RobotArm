namespace RobotArm.API.Models;

public record JoystickCommand
{
    public double Dx { get; init; }
    
    public double Dy { get; init; }
    
    public double Dz { get; init; }
    
    public double DPich { get; init; }
    
    public double DYaw { get; init; }
    
    public double DGrip { get; init; }
    
    public double Speed { get; init; } = 0.5;
}