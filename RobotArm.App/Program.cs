using RobotArm.Comms;
using RobotArm.Core;
using RobotArm.Domain;

var client = new ArduinoClient();
await client.Connect("ws://10.0.0.35");

await client.SendServos(new ServoAngles { S6=90, S5=90, S4=90, S3=90, S2=90, S1=0 });
await Task.Delay(2000);

// Prueba shoulder +30°
await client.SendServos(new ServoAngles { S6=90, S5=120, S4=90, S3=90, S2=90, S1=0 });
await Task.Delay(2000);

// Prueba shoulder -30°
await client.SendServos(new ServoAngles { S6=90, S5=60, S4=90, S3=90, S2=90, S1=0 });
await Task.Delay(2000);

// Vuelve neutral
await client.SendServos(new ServoAngles { S6=90, S5=90, S4=90, S3=90, S2=90, S1=0 });
await Task.Delay(2000);

// Prueba elbow +30°
await client.SendServos(new ServoAngles { S6=90, S5=90, S4=120, S3=90, S2=90, S1=0 });
await Task.Delay(2000);

// Prueba elbow -30°
await client.SendServos(new ServoAngles { S6=90, S5=90, S4=60, S3=90, S2=90, S1=0 });
await Task.Delay(2000);

await client.SendServos(new ServoAngles { S6=90, S5=90, S4=90, S3=90, S2=40, S1=0 });
await Task.Delay(2000);


await client.SendServos(new ServoAngles { S6=90, S5=90, S4=90, S3=90, S2=120, S1=0 });
await Task.Delay(2000);


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
        pitch * Math.PI / 180, 
        yaw * Math.PI / 180, 
        grip
    );

    var status = InverseKinematics.checkSingularity(target);
    
    if (status is InverseKinematics.ArmStatus.NearFullExtension ext)
        Console.WriteLine($"Advertencia: brazo casi extendido ({ext.Item * 100:F1}% del alcance maximo");
    else if (status is InverseKinematics.ArmStatus.NearSingularity sing)
        Console.WriteLine($"Advertencia: cerca de singularidad (ratio = {sing.Item:F2})");
    
    try
    {
        var (angles, fkError) = InverseKinematics.solveVerified(target, 1);
        
        var servo = ServoMapper.ToServo(new Angles
        {
            Base = angles.Base,
            Shoulder = angles.Shoulder,
            Elbow = angles.Elbow,
            WristYaw = angles.WristYaw,
            WristPitch = angles.WristPitch,
            Grip = angles.Grip
        });
        
        Console.WriteLine($"Base:       {angles.Base * 180.0 / Math.PI:F2}°");
        Console.WriteLine($"Shoulder:   {angles.Shoulder * 180.0 / Math.PI:F2}°");
        Console.WriteLine($"Elbow:      {angles.Elbow * 180.0 / Math.PI:F2}°");
        Console.WriteLine($"WristPitch: {angles.WristPitch * 180.0 / Math.PI:F2}°");
        Console.WriteLine($"FK error:   {fkError:F4} mm");
        
        await client.SendServos(servo);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ex.Message} - ingresa otra posicion.");
    }
}

