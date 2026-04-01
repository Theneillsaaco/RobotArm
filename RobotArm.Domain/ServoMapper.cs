namespace RobotArm.Domain;

public static class ServoMapper
{
    static double Deg(double r) => r * 180.0 / Math.PI;

    // OFFSETS (AJUSTAR CON CALIBRACIÓN)
    const int baseOffset = 87;
    const int shoulderOffset = 102;
    const int elbowOffset = 78;
    const int wristYawOffset = 90;
    const int wristPitchOffset = 90;

    public static ServoAngles ToServo(Angles a)
    {
        int baseServo = (int)(baseOffset + Deg(a.Base));
        int shoulderServo = (int)(shoulderOffset - Deg(a.Shoulder));
        int elbowServo = (int)(elbowOffset + Deg(a.Elbow));
        int wristYaw = (int)(wristYawOffset + Deg(a.WristYaw));
        int wristPitch = (int)(wristPitchOffset + Deg(a.WristPitch));
        int grip = (int)(a.Grip);

        return new ServoAngles
        {
            S6 = Math.Clamp(baseServo, 0, 180),
            S5 = Math.Clamp(shoulderServo, 0, 180),
            S4 = Math.Clamp(elbowServo, 0, 180),
            S3 = Math.Clamp(wristYaw, 0, 180),
            S2 = Math.Clamp(wristPitch, 0, 180),
            S1 = Math.Clamp(grip, 0, 110)
        };
    }
}