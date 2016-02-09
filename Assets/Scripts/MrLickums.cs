using UnityEngine;
using UnityEditor;
using System.Collections;

public class MrLickums : Enemy
{
    private static float bounceOffPlayerSpeed = 3f;

    public Sprite[] spriteWalk;
    public Sprite[] spriteStand;
    public Sprite spriteAirUp;
    public Sprite spriteAirDown;

    public float flySpeed;

    public float damage;
    public float health = 3;
    public float moveWaitTime = 2; // Seconds waited before changing velocity
    public float moveWaitTimer; // Actual timer that counts down
    public bool stuckOnWall = false;

    public bool ______________________;

    // Use this for initialization
    override public void Start()
    {
        base.Start();
        state_machine.ChangeState(new StateMrLickumsNormal(this));
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Player")
        {
            Player.S.Damage(damage);

            var v = (transform.position - Player.S.transform.position).normalized;
            rb().velocity += v * bounceOffPlayerSpeed;
        }
        else if (c.gameObject.tag == "ScoopProjectile")
        {
            health--;
        }
        else if (c.gameObject.tag == "Tile")
        {
            stuckOnWall = true;
        }
    }

    public class StateMrLickumsNormal : State
    {
        MrLickums p;
        private float totalTime;

        public StateMrLickumsNormal(MrLickums _p)
        {
            p = _p;
            totalTime = 0;
        }

        public override void OnUpdate(float time_delta_fraction)
        {
            p.moveWaitTimer -= Time.deltaTime;

            // Make movement look better by having slow down
            if (p.moveWaitTimer / p.moveWaitTime <= 0.25f)
            {
                p.rb().velocity = p.rb().velocity * (p.moveWaitTimer / p.moveWaitTime);
            }

            if (p.health <= 0)
            {
                Destroy(p.gameObject);
            }

            if (p.stuckOnWall && p.rb().velocity.magnitude <= 0.25f)
            {
                Vector3 tempVel = p.rb().velocity;
                tempVel.y = 1;
                p.rb().velocity = tempVel;
                p.moveWaitTimer = 1;
                p.stuckOnWall = false;
            }
            
            if (p.moveWaitTimer <= 0)
            {
                p.rb().velocity = (Player.S.transform.position - p.rb().position).normalized * p.flySpeed;
                p.moveWaitTimer = p.moveWaitTime;
            }
        }
    }
}