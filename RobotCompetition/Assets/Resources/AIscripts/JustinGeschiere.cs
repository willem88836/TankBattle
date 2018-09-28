using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustinGeschiere : RobotControl {

    //Class that holds robot data in memory
    private class RobotMemory
    {
        public TankData[] data;
        public float time;

        public RobotMemory(TankData[] newData, float newTime)
        {
            data = newData;
            time = newTime;
        }
    }

    //Class that keeps track of predicted bullet positions and their directions
    private class PredictBullet
    {
        public Vector3 pos;
        public Vector3 dir;

        public PredictBullet(Vector3 newPos, Vector3 newDir)
        {
            pos = newPos;
            dir = newDir;
        }
    }

    //Preset edge values
    float minAngleDelta = 5f;
    float minImpactAngle = 45f;
    float minDistance = 8f;
    float minSqrDistance = 144f;
    float minTimeDelta = 0.1f;

    //moving variables
    bool moving = false;
    int currentPower = 1;

    //color variables
    Color[] colors = new Color[6];
    int colorIndex0 = 0;
    int colorIndex1 = 1;
    float colorPercent = 0f;
    float colorScale = 5f;

    //Bullet prediction variables
    float myLastHealth = 100f;
    float timeEnemyShot = -1f;
    float bulletSpeed = 10f;

    //Dodging variables
    bool hitWall = false;
    bool tooClose = false;

    //List variables for the memory and predicted bullets
    List<RobotMemory> memory = new List<RobotMemory>();
    List<PredictBullet> bullets = new List<PredictBullet>();

    //Presseting variables
    void Start()
    {
        AssignColors();
        colorIndex0 = 0;
        colorIndex1 = 1;
        myLastHealth = GetOwnData().Health;
    }

    //Updates AI and visuals, contains a lot of checks to adapt the behaviours
    void Update ()
    {
        //If a tank is spotted, add the data to the memory
        TankData[] currentData = ReadSensor();
        if (currentData.Length > 0)
        {
            memory.Add(new RobotMemory(currentData, Time.timeSinceLevelLoad));
			float distance = Vector3.Distance(GetOwnData().Position, currentData[0].Position);

            if (distance < minDistance)
            {
                CancelInvoke("ResetTooClose");
                tooClose = true;
                Invoke("ResetTooClose", 1f);
            }
        }

        //Main updates that result in behaviour
        MemoryStuff();
        ShootStuff(currentData);
        MoveStuff(currentData);

        //Update last health
        myLastHealth = GetOwnData().Health;

		/*
        //Change the color to show a rainbow pattern
        colorPercent += Time.deltaTime * colorScale;
        if (colorPercent >= 1f)
        {
            colorIndex0++;
            colorIndex1++;
            if (colorIndex0 > colors.Length -1)
            {
                colorIndex0 = 0;
            }
            if (colorIndex1 > colors.Length -1)
            {
                colorIndex1 = 0;
            }
            colorPercent -= 1f;
        }
        Color newColor = Color.Lerp(colors[colorIndex0], colors[colorIndex1], colorPercent);
        ChangeColor(newColor, newColor, newColor);
		*/
	}

    //Behaviour part that regulates shooting and aiming
    private void ShootStuff(TankData[] currentData)
    {
        if (currentData.Length <= 0)
        {
            //if none are found keep rotating till one is found
            SetSensorAngle(90 + GetOwnData().SensorAngle);
            SetGunAngle(90 + GetOwnData().SensorAngle);
        }
        else if (currentData.Length == 1)
        {
            //if one is found, follow that one and shoot
            Vector3 RobotPos = currentData[0].Position;
            Quaternion quat = Quaternion.LookRotation((RobotPos - GetOwnData().Position).normalized);

            //Aim with bullet travel in mind
            Vector3 interceptPos = Intercept(currentData[0]);
            float targetAngle = quat.eulerAngles.y;
            if (interceptPos != Vector3.zero)
            {
                targetAngle = Quaternion.LookRotation(interceptPos).eulerAngles.y;
            }

            //Apply the rotations to the sensor and gun
            SetSensorAngle(quat.eulerAngles.y);
            SetGunAngle(targetAngle);

            //Check if the gun is remotely close to target
            if (Mathf.DeltaAngle(GetOwnData().GunAngle, targetAngle) < minAngleDelta)
            {
                Shoot();
            }
        }
        else
        {
            //if more robots are found, target the closest one
            int closestIndex = 0;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < currentData.Length; i++)
            {
				float distance = Vector3.Distance(GetOwnData().Position, currentData[0].Position);

				if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            //Aim at the closest enemy
            Vector3 RobotPos = currentData[closestIndex].Position;
            Quaternion quat = Quaternion.LookRotation((RobotPos - GetOwnData().Position).normalized);

            //Aim with bullet travel in mind
            Vector3 interceptPos = Intercept(currentData[0]);
            float targetAngle = quat.eulerAngles.y;
            if (interceptPos != Vector3.zero)
            {
                targetAngle = Quaternion.LookRotation(interceptPos).eulerAngles.y;
            }

            //Apply the rotations to the sensor and gun
            SetSensorAngle(quat.eulerAngles.y);
            SetGunAngle(targetAngle);

            //Check if the gun is remotely close to target
            if (Mathf.DeltaAngle(GetOwnData().GunAngle, targetAngle) < minAngleDelta)
            {
                Shoot();
            }
        }
    }

    //Function that calculates where the bullet could hit the target if the target keeps his heading and velocity
    private Vector3 Intercept(TankData target)
    {
        //Totally nicked this from the interwebz.
        Vector3 bulletVelocity = Quaternion.Euler(0f, GetOwnData().GunAngle, 0f) * Vector3.forward * bulletSpeed;
        Vector3 targetVelocity = Quaternion.Euler(0f, target.TankAngle, 0f) * Vector3.forward * target.MoveSpeed;

        Vector3 targetDir = target.Position - GetOwnData().Position;
        float iSpeed2 = bulletSpeed * bulletSpeed;
        float tSpeed2 = target.MoveSpeed * target.MoveSpeed;
        float fDot1 = Vector3.Dot(targetDir, targetVelocity);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)
        {
            return Vector3.zero;
        }

        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;

        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
            {
                return Vector3.zero;
            } else
            {
                return (S2) * targetDir + targetVelocity;
            }
        } else if (S2 < 0.0001f)
        {
            return (S1) * targetDir + targetVelocity;
        } else if (S1 < S2)
        {
            return (S2) * targetDir + targetVelocity;
        }
        return (S1) * targetDir + targetVelocity;
    }

    //Behaviour part that regulates moving and rotating
    private void MoveStuff(TankData[] currentData)
    {
        //Check if there are predicted bullets
        if (bullets.Count > 0)
        {
            int closestBullet = 0;
            float closestDist = 144f;
            float closestAngle = 0;
            //Check all bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                //Check if the bullets is within the danger range
                if ((GetOwnData().Position - bullets[i].pos).sqrMagnitude <= minSqrDistance)
                {
                    Vector3 deltaDir = GetOwnData().Position - bullets[i].pos;

                    Quaternion rot = Quaternion.LookRotation(deltaDir);

                    float deltaAngle = Mathf.DeltaAngle(Quaternion.LookRotation(bullets[i].dir).eulerAngles.y, rot.eulerAngles.y);

                    //Check if the bullet is a threat
                    if (Mathf.Abs(deltaAngle) <= minImpactAngle)
                    {
                        //Determine the closest bullets
                        float dist = (bullets[i].pos - GetOwnData().Position).sqrMagnitude;
                        if (dist < closestDist)
                        {
                            closestBullet = i;
                            closestDist = dist;
                            closestAngle = deltaAngle;
                        }
                    }
                }
            }

            //Move as fast as possible out of the bullet direction
            Quaternion bulletQuat = Quaternion.LookRotation(bullets[closestBullet].dir);
            if (Mathf.Abs(Mathf.DeltaAngle(bulletQuat.eulerAngles.y + 90, GetOwnData().TankAngle)) <= Mathf.Abs(Mathf.DeltaAngle(bulletQuat.eulerAngles.y - 90, GetOwnData().TankAngle)))
            {
                float setAngle = 90f;
                if (closestAngle > 0)
                {
                    SetMovePower(1);
                    if (hitWall)
                    {
                        setAngle -= 30;
                    }
                    if (tooClose)
                    {
                        setAngle += 30;
                    }
                }
                else
                {
                    SetMovePower(-1);
                    if (hitWall)
                    {
                        setAngle += 30;
                    }
                    if (tooClose)
                    {
                        setAngle -= 30;
                    }
                }
                //Apply the result of the pathing
                SetTankAngle(bulletQuat.eulerAngles.y + setAngle);
            }
            else
            {
                float setAngle = -90f;
                if (closestAngle > 0)
                {
                    SetMovePower(-1);
                    if (hitWall)
                    {
                        setAngle += 30;
                    }
                    if (tooClose)
                    {
                        setAngle -= 30;
                    }
                }
                else
                {
                    SetMovePower(1);
                    if (hitWall)
                    {
                        setAngle -= 30;
                    }
                    if (tooClose)
                    {
                        setAngle += 30;
                    }
                }
                //Apply the result of the pathing
                SetTankAngle(bulletQuat.eulerAngles.y + setAngle);
            }
            //Do not fall back on the strafing behaviour
            return;
        }

        //If no bullets are generated, go back to the default strafing behaviour
        if (currentData.Length <= 0)
        {
            SetTankAngle(GetOwnData().TankAngle - 90);
            SetMovePower(1);
        } else
        {
            //Find closest enemy
            int closestIndex = 0;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < currentData.Length; i++)
            {
				float distance = Vector3.Distance(GetOwnData().Position, currentData[0].Position);

				if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            Vector3 RobotPos = currentData[closestIndex].Position;
            Quaternion quat = Quaternion.LookRotation((RobotPos - transform.position).normalized);

            if (Mathf.Abs(Mathf.DeltaAngle(quat.eulerAngles.y + 90, GetOwnData().TankAngle)) <= Mathf.Abs(Mathf.DeltaAngle(quat.eulerAngles.y - 90, GetOwnData().TankAngle)))
            {
				float distance = Vector3.Distance(GetOwnData().Position, currentData[0].Position);

				if (distance <= minDistance)
                {
                    //If the enemy is too close, strafe away
                    if (currentPower >= 0)
                    {
                        SetTankAngle(quat.eulerAngles.y + 120);
                    }
                    else
                    {
                        SetTankAngle(quat.eulerAngles.y + 60);
                    }
                } else
                {
                    //If the enemy is not too close just strafe
                    SetTankAngle(quat.eulerAngles.y + 90);
                }
            } else
            {
				float distance = Vector3.Distance(GetOwnData().Position, currentData[0].Position);

				if (distance <= minDistance)
                {
                    //If the enemy is too close, strafe away
                    if (currentPower >= 0)
                    {
                        SetTankAngle(quat.eulerAngles.y - 120);
                    }
                    else
                    {
                        SetTankAngle(quat.eulerAngles.y - 60);
                    }
                } else
                {
                    //If the enemy is not too close just strafe
                    SetTankAngle(quat.eulerAngles.y - 90);
                }
            }

            if (!moving)
            {
                moving = true;
                SwitchMovement();
            }
        }
    }

    //Behaviour part that regulates memory and keeping track of predicting bullets
    private void MemoryStuff()
    {
        //Clear the bullets from last frame
        bullets.Clear();

        //Check if my own health was reduced, which means A bullet hit me
        if (myLastHealth != GetOwnData().Health && memory.Count > 0)
        {
            int closestIndex = 0;
            float closestDelta = 1f;
            //Calculate the moment the enemy has shot from the data
            for (int i = 0; i < memory.Count; i++)
            {
                float deltaTime = ((Vector3.Distance(memory[i].data[0].Position, GetOwnData().Position)) / bulletSpeed) - (Time.timeSinceLevelLoad - memory[i].time);
                if (deltaTime < closestDelta)
                {
                    closestIndex = i;
                    closestDelta = deltaTime;
                }
            }
            timeEnemyShot = memory[closestIndex].time;
        }
        else if (timeEnemyShot >= 0) //calculate from last shot where next shots are
        {
            for (int i = 0; i < memory.Count; i++)
            {
                //Determine if the time is a potential danger
                if (Mathf.Abs((memory[i].time - timeEnemyShot) % 0.5f) < minTimeDelta)
                {
                    //Add a predicted bullet
                    Vector3 bulletDir = Quaternion.Euler(new Vector3(0, memory[i].data[0].GunAngle, 0)) * Vector3.forward;
                    float deltaTime = Time.timeSinceLevelLoad - memory[i].time;
                    Vector3 bulletPos = memory[i].data[0].Position + (bulletDir * deltaTime * bulletSpeed);
                    bullets.Add(new PredictBullet(bulletPos, bulletDir));            
                }
            }
        }

        //Remove old memories
        for (int i = 0; i < memory.Count; i++)
        {
            if (memory[i].time < Time.timeSinceLevelLoad - 5f)
            {
                memory[i] = null;
            }
        }
        memory.RemoveAll(RobotMemory => RobotMemory == null);
    }

    //Switch movement
    private void SwitchMovement()
    {
        if (!moving)
        {
            return;
        }
        currentPower *= -1;
        SetMovePower(currentPower);
        Invoke("SwitchMovement", Random.Range(1.0f, 3.0f));
    }

    //Preset the colors that will be iterated
    private void AssignColors()
    {
        colors[0] = new Color32(255, 0, 0, 0);
        colors[1] = new Color32(255, 0, 255, 0);
        colors[2] = new Color32(0, 0, 255, 0);
        colors[3] = new Color32(0, 255, 255, 0);
        colors[4] = new Color32(0, 255, 0, 0);
        colors[5] = new Color32(255, 255, 0, 0);
    }

    //Pick a color based on a lerp
    private Color PickColor(Color color0, Color color1, float percent)
    {
        return Color.Lerp(color0, color1, percent);
    }

    //Check if the robot hits a wall
    protected override void OnWallCollision()
    {
        if (bullets.Count > 0)
        {
            CancelInvoke("ResetWallHit");
            hitWall = true;
            Invoke("ResetWallHit", 1f);
            return;
        }
        CancelInvoke("SwitchMovement");
        SwitchMovement();
    }

    //Reset the wall hit bool
    private void ResetWallHit()
    {
        hitWall = false;
    }

    //Reset the enemy too close bool
    private void ResetTooClose()
    {
        tooClose = false;
    }

    //Check if an enemy is hit (not with bullets)
    protected override void OnTankCollision()
    {
        CancelInvoke("SwitchMovement");
        SwitchMovement();
    }
}
