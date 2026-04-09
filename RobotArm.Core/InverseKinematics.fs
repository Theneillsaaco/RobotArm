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
    
    // Constantes del robot
    let L1 = 6.5
    let L2 = 11.5
    let L3 = 8.5
    let Lw = 4.0
    let ROffset = 1.5
    let groundOffset = 6.0
    let shoulderHeight = groundOffset + L1

    
    // Cinematica Directa
    let forward (angle: Angle) : Vector3 =
        let shoulderR = L2 * sin angle.Shoulder
        let shoulderY = shoulderHeight + L2 * cos angle.Shoulder
        
        let forearmAngle = angle.Shoulder + angle.Elbow
        let wristR = shoulderR + L3 * sin forearmAngle
        let wristY = shoulderY + L3 * cos forearmAngle
        
        let totalR = wristR + Lw + ROffset
        
        let x = totalR * cos angle.Base
        let z = totalR * sin angle.Base
        
        let pitchAbs = forearmAngle + angle.WristPitch
        let pitchDeg = pitchAbs * 180.0 / Math.PI
        
        {
            X = x; Y = wristY; Z = z 
            Pitch = pitchDeg
            Yaw = angle.WristYaw
            Grip = angle.Grip
        }
        
    // Error euclidiano entre dos puntos
    let positionError (a: Vector3) (b: Vector3) : float =
        sqrt ((a.X - b.X)**2.0 + (a.Y - b.Y)**2.0 + (a.Z - b.Z)**2.0)
    
    let solve (target: Vector3) : Angle =
        let x = target.X
        let z = target.Z
        
        let minYworld = 1.0
        let yWorld = max minYworld target.Y
        let y = yWorld - shoulderHeight
        
        let r_total = sqrt (x**2.0 + z**2.0)
        let r = r_total - ROffset
        let r2 = r - Lw
        
        let d = sqrt (r2**2.0 + y**2.0)
        
        if d > (L2 + L3) then
            failwith "Punto fuera de alcance"
            
        if d < abs (L2 - L3) then
            failwith "Muy cerca"
            
        // Base
        let clamp minVal maxVal value = 
            max minVal (min maxVal value)
        
        let theta0Raw = atan2 target.Z target.X 
        let theta0 = clamp (-Math.PI / 2.0) (Math.PI / 2.0) theta0Raw
        
        // Shoulder
        let cosTheta2 =
            (r2**2.0 + y**2.0 - L2**2.0 - L3**2.0) /
            (2.0 * L2 * L3)
            
        let theta2 = acos (max -1.0 (min 1.0 cosTheta2)) 
        
        // Elbow
        let theta1 =
            atan2 r2 y -
            atan2 (L3 * sin theta2) (L2 + L3 * cos theta2)
        
        // Muneca
        let forearmAngle = theta1 + theta2
        let wristPitch = (target.Pitch * Math.PI / 180.0) - forearmAngle
  
        {
            Base = theta0
            Shoulder = theta1
            Elbow = theta2
            WristPitch = wristPitch
            WristYaw = target.Yaw
            Grip = target.Grip
        }
        
    // Validadcion
    let solveVerified (target: Vector3) (errorThreshold: float): Angle * float =
        let angles = solve target
        let result = forward angles
        let err = positionError target result
        
        if err > errorThreshold then
            eprintfn $"IK round-trip error: {err:F4} mm (umbral : {errorThreshold:F4})"
            
        angles, err
    
    // Interpolacion en joint-space
    let interpolate (from: Angle) (to_: Angle) (steps: int) : Angle list =
        [   for i in 0..steps do 
            let t = float i / float steps
            let lerp a b = a + t * (b - a)
            yield {
                Base = lerp from.Base to_.Base
                Shoulder = lerp from.Shoulder to_.Shoulder
                Elbow = lerp from.Elbow to_.Elbow
                WristPitch = lerp from.WristPitch to_.WristPitch
                WristYaw = lerp from.WristYaw to_.WristYaw
                Grip = lerp from.Grip to_.Grip
            }
        ]
        
    // Detectar proximidad a singularidad
    type ArmStatus =
        | Ok
        | NearFullExtension of float // d / (L2 + L3)
        | NearSingularity of float // d / |L2 - L3|
        
    let checkSingularity (targer: Vector3) : ArmStatus =
        let x = targer.X
        let y = targer.Y
        let z = max (L1 + 1.0) targer.Z - L1
        let r2 = sqrt (x**2.0 + y**2.0) - ROffset - Lw
        let d = sqrt (r2**2.0 + z**2.0)
        let maxD = L2 + L3
        let minD = abs (L2 - L3)
        
        let extRatio = d / maxD
        let singRatio =
            if minD > 0.0 then
                d / minD
            else infinity
            
        if extRatio < 0.95 then
            NearFullExtension extRatio
        elif singRatio < 0.95 then
            NearSingularity singRatio
        else
            Ok