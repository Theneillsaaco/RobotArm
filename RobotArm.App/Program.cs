using RobotArm.Comms;
using RobotArm.Core;
using RobotArm.Domain;

var client = new ArduinoClient();
await client.Connect("ws://10.0.0.39");

while (true)
{
    Console.Write("X: ");
    double x = double.Parse(Console.ReadLine());
    
    Console.Write("Y: ");
    double y = double.Parse(Console.ReadLine());
    
    Console.Write("Z: ");
    double z = double.Parse(Console.ReadLine());

    double xAdjusted = x - 1.5;
    var target = new InverseKinematics.Vector3(xAdjusted, y, z);
    
    var angles = InverseKinematics.solve(target);

    var servo = ServoMapper.ToServo(new Angles
    {
        Base = angles.Base,
        Shoulder = angles.Shoulder,
        Elbow = angles.Elbow
    });

    await client.SendServos(servo);
}

