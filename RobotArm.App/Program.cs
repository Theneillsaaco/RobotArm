using RobotArm.Comms;
using RobotArm.Core;
using RobotArm.Domain;

var client = new ArduinoClient();
await client.Connect("ws://10.0.0.39");

while (true)
{
    Console.Write("X: ");
    double x = double.Parse(Console.ReadLine() ?? "0");
    
    Console.Write("Y: ");
    double y = double.Parse(Console.ReadLine() ?? "0");
    
    Console.Write("Z: ");
    double z = double.Parse(Console.ReadLine() ?? "0");
    
    Console.Write("Pitch (deg): ");
    double pitch = double.Parse(Console.ReadLine() ?? "0");

    Console.Write("Yaw (deg): ");
    double yaw = double.Parse(Console.ReadLine() ?? "0");

    Console.Write("Grip (0-110): ");
    double grip = double.Parse(Console.ReadLine() ?? "0");
    
    var target = new InverseKinematics.Vector3(
        x,
        y,
        z,
        pitch * Math.PI / 180.0,
        yaw * Math.PI / 180.0,
        grip * Math.PI / 180.0
    );
    
    var angles = InverseKinematics.solve(target);

    var servo = ServoMapper.ToServo(new Angles
    {
        Base = angles.Base,
        Shoulder = angles.Shoulder,
        Elbow = angles.Elbow
    });

    await client.SendServos(servo);
}

