using UnityEngine;
using UnityEditor;
using System.Collections;

public class MrLickums : Enemy
{
    private static float bounceOffPlayerSpeed = 3f;

    public Sprite[] spriteLick;

    public float flySpeed;

    public float damage;
    public float health = 3.0f;
    public float moveWaitTime = 0; // Seconds waited before changing velocity
    public float moveWaitTimer; // Actual timer that counts down
    public bool stuckOnWall = false;
    public Vector3 lastPosition;

    public int brownCount = 0;
    public int whiteCount = 0;
    public int pinkCount = 0;

    public GameObject dropPrefab;

    public bool ______________________;

    // Used to set the values for EnemyManager to different numbers than the base class
    public void Reset()
    {
        difficulty = 3; // the estimated difficulty of this enemy
        waveRequirement = 5; // the wave at which this enemy begins to spawn
        spawnWeight = 3; // the relative frequency at which this enemy spawns
    }

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
            totalTime = 0.0f;
        }

        public override void OnUpdate(float time_delta_fraction)
        {
            // Locates the drops on screen and heads for thems
            GameObject[] dropArray = GameObject.FindGameObjectsWithTag("Drop");
            GameObject closestDrop;

            // Move to the closest drop
            if (dropArray.Length != 0)
            {
                closestDrop = dropArray[0];
                foreach (GameObject drop in dropArray)
                {
                    if ((drop.transform.position - p.rb().position).magnitude < (closestDrop.transform.position - p.rb().position).magnitude)
                    {
                        closestDrop = drop;
                    }
                }

                if ((Player.S.transform.position - p.rb().position).magnitude < (closestDrop.transform.position - p.rb().position).magnitude)
                {
                    p.rb().velocity = (Player.S.transform.position - p.rb().position).normalized * p.flySpeed;
                }
                else
                {
                    p.rb().velocity = (closestDrop.transform.position - p.rb().position).normalized * p.flySpeed;
                }

            }
            // Move to the player
            else
            {
                p.rb().velocity = (Player.S.transform.position - p.rb().position).normalized * p.flySpeed;
            }

            p.sprend().sprite = p.spriteLick[(int)Mathf.Abs(totalTime * 8) % p.spriteLick.Length];
            totalTime = Mathf.Max(totalTime + Mathf.Abs(time_delta_fraction), 0);

        }
    }

    public void Damage(float _damage)
    {
        health -= _damage;

        if (health <= 0)
        {
            Destroy(this.gameObject);
            releaseDrops();
        }
    }

    public void collectDrop(Utils.IceCream flavor)
    {
        if (flavor == Utils.IceCream.Brown)
        {
            brownCount++;
        }
        else if (flavor == Utils.IceCream.White)
        {
            whiteCount++;
        }
        else if (flavor == Utils.IceCream.Pink)
        {
            pinkCount++;
        }
    }

    public void releaseDrops()
    {
        if (brownCount > 0)
        {
            GameObject o = Instantiate<GameObject>(dropPrefab);
            o.GetComponent<Drop>().Initialize(transform.position + Vector3.left, brownCount / 20.0f, Utils.IceCream.Brown);
        }

        if (whiteCount > 0)
        {
            GameObject o = Instantiate<GameObject>(dropPrefab);
            o.GetComponent<Drop>().Initialize(transform.position, whiteCount / 20.0f, Utils.IceCream.White);
        }

        if (pinkCount > 0)
        {
            GameObject o = Instantiate<GameObject>(dropPrefab);
            o.GetComponent<Drop>().Initialize(transform.position + Vector3.right, pinkCount / 20.0f, Utils.IceCream.Pink);
        }
    }
}