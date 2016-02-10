using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private static int JumpVelocity = 8;
    public static Player S;

    public GameObject face;
    public GameObject jumpCollider;
    public GameObject dropPrefab;

    public GameObject topScoop;
    public GameObject topScoopThrowPrefab;
    public Vector3 topScoopTo;
    public Drop.IceCream topScoopType = Drop.IceCream.Pink;
    public bool topScoopReadyToThrow = false;
    public float topScoopMassNormal;
    public float topScoopMass;
    public float topScoopLaunchVelocity;
    public Drop.IceCream middleScoopType = Drop.IceCream.White;
    public Drop.IceCream hurtScoopType = Drop.IceCream.Brown;

    public GameObject middleScoop;
    public GameObject middleScoopThrowPrefab;
    public Vector3 middleScoopTo;
    public bool middleScoopReadyToThrow = false;
    public float middleScoopMassNormal;
    public float middleScoopMass;
    public float middleScoopLaunchVelocity;

    public float walkSpeed;

    public bool _____________________________;

    public StateMachine state_machine = new StateMachine();

    public bool grounded = true;

    public int lastDirection = 1;
    private float normalRadius;
    private float normalMass;

    public SpriteRenderer sprend() { return GetComponent<SpriteRenderer>(); }
    public Rigidbody rb() { return GetComponent<Rigidbody>(); }

    public float hurtTimer = 0;
    private float hurtTimerMax = 0.25f; //How long you are invincible after being hit
    private float hurtTimerFlashInterval = 8f; //Number of flashes over full time span

    private float lastTime;

    // Use this for initialization
    void Start () {
        S = this;
        normalRadius = GetComponent<SphereCollider>().radius;
        normalMass = rb().mass;
        state_machine.ChangeState(new StatePlayerMove(this));
        lastTime = Time.time;
	}

    float shake = 0f;
    float shakeAmount = 0.7f;
    public static void Screenshake(float _time, float _multiplier)
    {
        S.shake = _time;
        S.shakeAmount = _multiplier;
    }

    // Update is called once per frame
    void Update ()
    {
        float dt = Time.time - lastTime;
        
        //Screenshakes
        if (shake > 0)
        {
            Camera.main.transform.position += Random.insideUnitSphere * shakeAmount * shake;
            shake -= dt;
        }
        else
        {
            shake = 0f;
        }

        if (hurtTimer > 0)
        {
            hurtTimer = Mathf.Max(hurtTimer - dt, 0);
            if ((int)(hurtTimer / hurtTimerMax * hurtTimerFlashInterval) % 2 == 1)
            {
                sprend().color = new Color(1, 0, 0, 1);
                topScoop.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                middleScoop.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            }
            else
            {
                sprend().color = new Color(1, 1, 1, 1);
                topScoop.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                middleScoop.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }

        topScoopMassNormal = Drop.massNormals[(int)topScoopType];
        middleScoopMassNormal = Drop.massNormals[(int)middleScoopType];

        state_machine.Update();

        var topScoopPos = topScoop.transform.position;
        var middleScoopPos = middleScoop.transform.position;

        topScoopPos += (topScoopTo - topScoopPos) * 0.25f;
        middleScoopPos += (middleScoopTo - middleScoopPos) * 0.25f;

        topScoop.transform.position = topScoopPos;
        middleScoop.transform.position = middleScoopPos;

        lastTime = Time.time;
    }

    void GameOver()
    {
        //Called when the player dies
    }

    public void AddMass(float _mass)
    {
        rb().mass = Mathf.Min(rb().mass + _mass, normalMass);
    }

    public void Damage(float _damage)
    {
        if (hurtTimer <= 0) //This means we aren't currently hurt and invincible
        {
            ExplodeInDirection(_damage);
            hurtTimer = hurtTimerMax;
        }
    }

    public void ExplodeInDirection(float massToRelease)
    {
        var rb = this.rb();
        var massToReleaseStart = massToRelease;
        int numDrops = (int)(Random.value * 5 + 10);

        while (massToRelease > 0)
        {
            float massForThisDrop = Mathf.Min(massToReleaseStart / numDrops * (0.1f + 1.9f * Random.value), massToRelease);
            SpawnDropRandom(massForThisDrop, Random.value * 5 + 7);
            massToRelease -= massForThisDrop;
        }

        if (rb.mass <= massToReleaseStart)
        {
            rb.mass = 0.1f;
            GameOver();
        }
        else
            rb.mass -= massToReleaseStart;
    }

    void SpawnDropRandom(float _mass, float _speed)
    {
        var o = Instantiate<GameObject>(dropPrefab);
        o.GetComponent<Drop>().Initialize(transform.position, _mass, hurtScoopType);
        o.GetComponent<Drop>().LaunchRandomRadially(transform.position, _speed);
    }

    public float n3 { get { return Mathf.Pow(rb().mass / normalMass, 0.333f); } }

    public class StatePlayerMove : State
    {
        Player p;

        public StatePlayerMove(Player _p)
        {
            p = _p;
        }


        public override void OnUpdate(float time_delta_fraction)
        {
            Vector3 pos = p.transform.position;
            var rb = p.rb();
            var sprend = p.sprend();
            var vel = rb.velocity;

            float h_input = Input.GetAxis("Horizontal");
            bool v_input = Input.GetKeyDown(KeyCode.W);

            /*
            if(Input.GetKeyDown(KeyCode.S))
                rb.mass += 1;
            else
                rb.mass -= Mathf.Abs(time_delta_fraction);
            sprend.transform.localScale = rb.mass / p.normalMass * Vector3.one;
            p.GetComponent<SphereCollider>().radius = rb.mass / p.normalMass * p.normalRadius;
            */

            p.lastDirection = Utils.Sign(Utils.mouse.x - p.transform.position.x, false);
            if (v_input && p.grounded)
            {
                vel = new Vector3(vel.x, Mathf.Max(JumpVelocity, vel.y), 0);
            }


            p.topScoopTo = p.getTopScoopTo(h_input, p.topScoopReadyToThrow);
            p.middleScoopTo = p.getMiddleScoopTo(h_input, p.middleScoopReadyToThrow);
            
            p.face.transform.position = new Vector3(p.lastDirection * (0.4f), -0.1f - 0.1f * Mathf.Min(Mathf.Abs(h_input), 1f) + 0.15f * Mathf.Sign(vel.y) * Mathf.Min(Mathf.Max(Mathf.Abs(vel.y / 5), 0), 1), 0) * p.n3 + pos;
            p.face.transform.rotation = Quaternion.identity;

            p.topScoopMass = Mathf.Min(p.topScoopMass, p.topScoopMassNormal);
            p.middleScoopMass = Mathf.Min(p.middleScoopMass, p.middleScoopMassNormal);
            if (p.topScoopMass <= 0)
                p.topScoop.GetComponent<SpriteRenderer>().enabled = false;
            else
            {
                p.topScoop.GetComponent<SpriteRenderer>().enabled = true;
                p.topScoop.GetComponent<SpriteRenderer>().transform.localScale = Mathf.Pow(p.topScoopMass, 0.333f) / Mathf.Pow(p.topScoopMassNormal, 0.333f) * Vector3.one;
            }
            if (p.middleScoopMass <= 0)
                p.middleScoop.GetComponent<SpriteRenderer>().enabled = false;
            else
            {
                p.middleScoop.GetComponent<SpriteRenderer>().enabled = true;
                p.middleScoop.GetComponent<SpriteRenderer>().transform.localScale = Mathf.Pow(p.middleScoopMass, 0.333f) / Mathf.Pow(p.middleScoopMassNormal, 0.333f) * Vector3.one;
            }
            if (p.rb().mass > 0)
            {
                p.GetComponent<SpriteRenderer>().enabled = true;
                p.GetComponent<SpriteRenderer>().transform.localScale = Mathf.Pow(p.rb().mass, 0.333f) / Mathf.Pow(p.normalMass, 0.333f) * Vector3.one;
            }
            //if (Mathf.Abs(rb.velocity.x) <= p.walkSpeed * 1.1f)
            vel.x = h_input * p.walkSpeed;

            rb.velocity = vel;
            

            if (p.topScoopMass >= p.topScoopMassNormal / 2f)
            {
                if (Input.GetMouseButtonDown(0))//GetKeyDown(KeyCode.Z))
                {
                    p.topScoopReadyToThrow = true;
                }
                if (Input.GetMouseButtonUp(0))//GetKeyUp(KeyCode.Z) && p.topScoopReadyToThrow)
                {
                    var o = Instantiate<GameObject>(p.topScoopThrowPrefab);
                    o.GetComponent<ThrownScoop>().ResetScaleToMass();
                    o.GetComponent<ThrownScoop>().type = p.topScoopType;
                    o.transform.position = p.getTopScoopTo(0, false);
                    o.GetComponent<Rigidbody>().mass = p.topScoopMass;
                    p.topScoopMass -= o.GetComponent<Rigidbody>().mass;

                    var angleToMouse = Utils.GetAngleToMouse(o.transform.position);
                    o.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(angleToMouse), Mathf.Sin(angleToMouse)) * p.topScoopLaunchVelocity;// + p.GetComponent<Rigidbody>().velocity;
                    p.topScoopReadyToThrow = false;
                }
            }
            if (Input.GetMouseButtonDown(1))//GetKeyDown(KeyCode.X))
            {
                p.middleScoopReadyToThrow = true;
            }
            if (Input.GetMouseButtonUp(1))//GetKeyUp(KeyCode.X) && p.middleScoopReadyToThrow)
            {
                var o = Instantiate<GameObject>(p.middleScoopThrowPrefab);
                o.GetComponent<ThrownScoop>().ResetScaleToMass();
                o.GetComponent<ThrownScoop>().type = p.middleScoopType;
                o.transform.position = p.getMiddleScoopTo(0, false);
                o.GetComponent<Rigidbody>().mass = Mathf.Min(Mathf.Max(p.middleScoopMass / 2f, 0.5f), p.middleScoopMass);
                p.middleScoopMass -= o.GetComponent<Rigidbody>().mass;

                var angleToMouse = Utils.GetAngleToMouse(o.transform.position);
                o.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(angleToMouse), Mathf.Sin(angleToMouse)) * p.middleScoopLaunchVelocity;// + p.GetComponent<Rigidbody>().velocity;

                o.GetComponent<ThrownScoop>().ExplodeInDirection();

                p.middleScoopReadyToThrow = false;
            }

            p.transform.position = pos;
        }
    }

    private Vector3 getTopScoopTo(float h_input, bool _topScoopReadyToThrow)
    {
        return new Vector3(Player.S.lastDirection * (0.3f - 0.5f * (_topScoopReadyToThrow ? 1 : 0)), 0.55f - 0.15f * (h_input != 0f ? 1 : 0), 0) * Player.S.n3 + Player.S.transform.position;
    }

    private Vector3 getMiddleScoopTo(float h_input, bool _middleScoopReadyToThrow)
    {
        return new Vector3(Player.S.lastDirection * (-0.35f - 0.5f * (_middleScoopReadyToThrow ? 1 : 0)), 0.5f - 0.2f * (_middleScoopReadyToThrow ? 1 : 0) - 0.15f * Mathf.Min(Mathf.Abs(h_input), 1f), 0) * Player.S.n3 + Player.S.transform.position;
    }
}
