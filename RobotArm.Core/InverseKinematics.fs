namespace RobotArm.Core

open System

module InverseKinematics =
    type Vector3 = {
        X: float
        Y: float
        Z: float
        Pitch: float
        Yaw: float
        Grip: float
    }
    
    type Angle = {
        Base: float
        Shoulder: float
        Elbow: float
        WristPitch: float
        WristYaw: float
        Grip: float
    }
    
    let L1 = 6.5
    let L2 = 11.5
    let L3 = 8.5
    
    let solve (target: Vector3) =
        let x = target.X
        let y = target.Y
        let zSafe = max (-L1 + 2.0) target.Z
        let z = zSafe - L1
        
        let Lw = 4.0
        
        let r_total = sqrt (x**2.0 + y**2.0)
        let r = r_total - 1.5
        let r2 = r - Lw
        
        let d = sqrt (r2**2.0 + z**2.0)
        
        if d > (L2 + L3) then
            failwith "Punto fuera de alcance"
            
        if d < abs (L2 - L3) then
            failwith "Muy cerca"
            
        // Base
        let theta0 = atan2 target.X target.Y
        
        // Shoulder
        let cosTheta2 =
            (r2**2.0 + z**2.0 - L2**2.0 - L3**2.0) /
            (2.0 * L2 * L3)
            
        let theta2 = acos (max -1.0 (min 1.0 cosTheta2)) 
        
        // Elbow

        let theta1 =
            atan2 z r2 -
            atan2 (L3 * sin theta2) (L2 + L3 * cos theta2)
            
        let theta1Safe = max (-Math.PI / 4.0) theta1
        
        // Muneca
        let forearmAngle = theta1Safe + theta2
        let wristPitch = (target.Pitch * Math.PI / 180.0) - forearmAngle
            
        // Rotacion muneca
        let wristYaw = target.Yaw
        
        {
            Base = theta0
            Shoulder = theta1Safe
            Elbow = theta2
            WristPitch = wristPitch
            WristYaw = wristYaw
            Grip = target.Grip
        }