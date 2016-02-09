using UnityEngine;
using UnityEditor;
using System.Collections;

public class BasicEnemy : Enemy {

    private static float JumpSpeedLong = 5.5f;
    private static float JumpSpeedShort = 3.5f;
    private static float JumpSpeedHigh = 5;
    private static float JumpSpeedHigher = 8;
    private static float JumpSpeedHighest = 9;
    private static float bounceOffPlayerSpeed = 3f;

    public Sprite[] spriteWalk;
    public Sprite[] spriteStand;
    public Sprite spriteAirUp;
    public Sprite spriteAirDown;

    public int spriteWalkSpeed;
    public int spriteStandSpeed;

    public float walkSpeed;

    public float damage;

    public bool ______________________;

    // Use this for initialization
    override public void Start ()
    {
        base.Start();
        state_machine.ChangeState(new StateBasicEnemyAir(this));
	}
	
	// Update is called once per frame
	override public void Update ()
    {
        base.Update();


	}
    
    public void Damage(float _damage)
    {
        Destroy(this.gameObject);
        /*
        if (hurtTimer <= 0) //This means we aren't currently hurt and invincible
        {
            ExplodeInDirection(_damage);
            hurtTimer = hurtTimerMax;
        }*/
    }

    void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Player")
        {
            Player.S.Damage(damage);

            var v = (transform.position - Player.S.transform.position).normalized;
            rb().velocity += v * bounceOffPlayerSpeed;
        }
    }

    public class StateBasicEnemyNormal : State
    {
        BasicEnemy p;
        private float totalTime;

        public StateBasicEnemyNormal(BasicEnemy _p)
        {
            p = _p;
            totalTime = 0;
        }

        public override void OnUpdate(float time_delta_fraction)
        {
            p.sprend().color = new Color(1, 1, 1, 1);

            p.direction = p.directionToTarget;

            var vel = p.rb().velocity;
            bool moveTo = true; //Whether the enemy should attempt to move

            RaycastHit hitInfo;
            var hit = Physics.Raycast(p.transform.position, new Vector3(p.direction, 0, 0), out hitInfo);

            if (hit)
            {
                //Debug.DrawLine(p.transform.position, hitInfo.point);
                float distanceJump = 1.25f;
                if (hitInfo.distance <= distanceJump)
                {
                    //Debug.DrawLine(hitInfo.point, new Vector3(Mathf.Round(hitInfo.point.x), Mathf.Round(hitInfo.point.y) + 1));
                    for (int i = 1; i <= 3; i++)
                    {
                        Collider[] collided = Physics.OverlapBox(new Vector3(Mathf.Round(hitInfo.point.x + p.direction * 0.25f), Mathf.Round(hitInfo.point.y) + i), new Vector3(0.4f, 0.4f, 0.1f));
                        if (collided.Length == 0)
                        {
                            float jumpSpeed = 0;
                            float multSpeedX = 1;
                            switch (i)
                            {
                                case 1:
                                    //Debug.Log("Jump up! = Short");
                                    jumpSpeed = JumpSpeedHigh;
                                    multSpeedX = 0.6f * hitInfo.distance / distanceJump;// 0.3f;
                                    //EditorApplication.isPaused = true;
                                    break;
                                case 2:
                                    //Debug.Log("Jump up! = Mid");
                                    jumpSpeed = JumpSpeedHigher;
                                    multSpeedX = 0.4f * hitInfo.distance / distanceJump;// 0.135f;
                                    break;
                                case 3:
                                    //Debug.Log("Jump up! = High");
                                    jumpSpeed = JumpSpeedHighest;
                                    multSpeedX = 0.4f * hitInfo.distance / distanceJump;// 0.2f;
                                    break;
                            }
                            
                            vel = new Vector3(multSpeedX * p.walkSpeed * p.direction, Mathf.Max(vel.y, jumpSpeed));
                            p.rb().velocity = vel;
                            var s = new StateBasicEnemyAir(p);
                            p.state_machine.ChangeState(s);
                            return;
                        }
                    }
                }
                else
                {
                    int N = (int)hitInfo.distance;
                    float[] depth = new float[N];
                    RaycastHit downHitInfo;
                    for(int i = 0; i < N; i++)
                    {
                        var start = new Vector3(Mathf.Round(p.transform.position.x) + i * p.direction, Mathf.Round(p.transform.position.y));
                        var downHit = Physics.Raycast(start, new Vector3(0, -1, 0), out downHitInfo);
                        if(downHit)
                        {
                            depth[i] = downHitInfo.distance;
                            //Debug.DrawLine(start, downHitInfo.point);
                        }
                        else
                        {
                            depth[i] = float.MaxValue;
                            //Debug.DrawRay(start, new Vector3(0, -1000f, 0));
                        }
                    }
                    if (depth.Length >= 3)
                    {
                        //If there's a pit immediately in front of us and a tile on the other side, jump over to it.
                        if (depth[1] > 0.75f && depth[2] <= 0.75f)
                        {
                            //Debug.Log("Short jump!");
                            vel = new Vector3(vel.x, Mathf.Max(vel.y, JumpSpeedShort));
                            p.state_machine.ChangeState(new StateBasicEnemyAir(p));
                        }
                        //If there's a pit in the two spots immediately in front of us and a tile on the other side, jump over to it.
                        else if (depth.Length >= 4 && depth[1] > 0.75f && depth[2] == 0.75f && depth[3] <= 0.75f)
                        {
                            //Debug.Log("Long jump!");
                            vel = new Vector3(vel.x, Mathf.Max(vel.y, JumpSpeedLong));
                            p.state_machine.ChangeState(new StateBasicEnemyAir(p));
                        }
                    }
                }
            }

            if(p.isRunning())
            {
                p.sprend().flipX = p.direction == -1;
                p.sprend().sprite = p.spriteWalk[(int)Mathf.Abs(totalTime * p.spriteWalkSpeed * p.rb().velocity.x / p.walkSpeed) % p.spriteWalk.Length];
            }
            else
            {
                p.sprend().sprite = p.spriteStand[(int)(totalTime * p.spriteStandSpeed) % p.spriteStand.Length];
            }

            if (p.distanceToTargetX() < 1)
                moveTo = false;

            if (p.grounded)
            {
                if (moveTo)
                    vel += (new Vector3(p.walkSpeed * p.direction, vel.y, 0) - vel) * 0.25f;
            }
            else
            {
                p.state_machine.ChangeState(new StateBasicEnemyAir(p));
                return;
            }
            p.rb().velocity = vel;
            totalTime = Mathf.Max(totalTime + Mathf.Abs(time_delta_fraction), 0);
        }
    }

    public class StateBasicEnemyAir : State
    {
        BasicEnemy p;

        public float speedX;

        public StateBasicEnemyAir(BasicEnemy _p, float _speedX=-1)
        {
            p = _p;
            speedX = _speedX;
            if (speedX != -1)
                p.rb().velocity = new Vector3(speedX, p.rb().velocity.y, 0);
            else
                speedX = p.rb().velocity.x;
        }

        bool turnedNegative = false;
        public override void OnUpdate(float time_delta_fraction)
        {
            var rb = p.rb();
            var sprend = p.sprend();

            if (!turnedNegative && rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(speedX, rb.velocity.y);
                turnedNegative = true;
            }

            //Control animation
            if (rb.velocity.y < 0)
                sprend.sprite = p.spriteAirDown;
            else
                sprend.sprite = p.spriteAirUp;

            if (p.grounded)
                p.state_machine.ChangeState(new StateBasicEnemyNormal(p));
        }
    }
}
