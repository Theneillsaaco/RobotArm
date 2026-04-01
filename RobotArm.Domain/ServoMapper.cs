namespace RobotArm.Domain;

public static class ServoMapper
{
    public static ServoAngles ToServo(Angles a)
    {
        int baseServo = (int)(a.Base * 180.0 / Math.PI) + 90;
        int shoulderServo = 90 + (int)(a.Shoulder * 180.0 / Math.PI);
        int elbowServo = 90 + (int)(a.Elbow * 180.0 / Math.PI);

        return new ServoAngles
        {
            S6 = baseServo,
            S5 = shoulderServo,
            S4 = elbowServo,
        };
    }
}