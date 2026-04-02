namespace RobotArm.API.Models;

public record ArmStateDto
{
    public double BaseDeg { get; init; }
    
    public double ShoulderDeg { get; init; }
    
    public double ElbowDeg { get; init; }
    
    public double WristPitchDeg { get; init; }
    
    public double WristYawDeg { get; init; }
    
    public double Grip { get; init; }
    
    public double FkError { get; init; }
    
    public double X { get; init; }
    
    public double Y { get; init; }
    
    public double Z { get; init; }
}