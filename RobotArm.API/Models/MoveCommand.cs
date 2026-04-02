namespace RobotArm.API.Models;

public record MoveCommand
{
    public double X { get; init; }
    
    public double Y { get; init; }
    
    public double Z { get; init; }
    
    public double Pitch { get; init; }
    
    public double Yaw { get; init; }
    
    public double Grip { get; init; }
}