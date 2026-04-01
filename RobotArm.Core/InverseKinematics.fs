namespace RobotArm.Core

module InverseKinematics =
    type Vector3 = { X: float; Y: float; Z: float }
    
    type Angle = {
        Base: float
        Shoulder: float
        Elbow: float
    }
    
    let L1 = 6.5
    let L2 = 11.5
    let L3 = 8.5
    let L4 = 9.75
    
    let solve (target: Vector3) =
        let r = sqrt (target.X**2.0 + target.Y**2.0)
        let r2 = r - L4
        let z =
            if target.Z < -6.0 then -6.0
            else target.Z
        
        let d = sqrt (r2**2.0 + z**2.0)
        
        if d > (L2 + L3) then
            failwith "Punto fuera de alcance"
            
        if d < abs (L2 - L3) then
            failwith "Muy cerca"
            
        let theta0 = atan2 target.X target.Y
        
        let cosTheta2 = (r2**2.0 + z**2.0 - L2**2.0 - L3**2.0) / (2.0 * L2 * L3)
        let theta2 = acos cosTheta2
        
        let theta1 =
            atan2 z r2 -
            atan2 (L3 * sin theta2) (L2 + L3 * cos theta2)
            
        { Base = theta0; Shoulder = theta1; Elbow = theta2 }

