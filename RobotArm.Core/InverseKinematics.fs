namespace RobotArm.Core

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
    let L4 = 9.75
    
    let solve (target: Vector3) =
        let x = target.X - 1.5
        let y = target.Y
        let z =
            if target.Z < -6.0 then -6.0
            else target.Z
        
        let gripLength =
            13.0 - (target.Grip / 110.0) * (13.0 - 6.5)
        
        let Lw = 3.25 + gripLength
        
        let r = sqrt (x**2.0 + y**2.0)
        let r2 = r - Lw
        
        let d = sqrt (r2**2.0 + z**2.0)
        
        if d > (L2 + L3) then
            failwith "Punto fuera de alcance"
            
        if d < abs (L2 - L3) then
            failwith "Muy cerca"
            
        // Base
        let theta0 = atan2 x y
        
        // Shoulder
        let cosTheta2 =
            (r2**2.0 + z**2.0 - L2**2.0 - L3**2.0) /
            (2.0 * L2 * L3)
            
        let theta2 = acos cosTheta2
        
        // Elbow
        let theta1 =
            atan2 z r2 -
            atan2 (L3 * sin theta2) (L2 + L3 * cos theta2)
            
        // Muneca
        let wristPitch =
            target.Pitch - (theta1 + theta2)
            
        // Rotacion muneca
        let wristYaw = target.Yaw
        
        {
            Base = theta0
            Shoulder = theta1
            Elbow = theta2
            WristPitch = wristPitch
            WristYaw = wristYaw
            Grip = target.Grip
        }

