using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public static Player S;

    public GameObject topScoop;
    public GameObject middleScoop;
    public GameObject face;
    public GameObject jumpCollider;
    public GameObject topScoopThrowPrefab;
    public GameObject middleScoopThrowPrefab;

    public float topScoopMassNormal = 1;
    public float topScoopMass = 1;
    public float topScoopLaunchVelocity = 16;
    public float middleScoopMassNormal = 2;
    public float middleScoopMass = 2;
    public float middleScoopLaunchVelocity = 8;

    public float walkSpeed;

    public bool _____________________________;

    public StateMachine state_machine = new StateMachine();

    public bool grounded = true;
    public bool topScoopReadyToThrow = false;
    public bool middleScoopReadyToThrow = false;

    public int lastDirection = 1;

    public Vector3 topScoopTo;
    public Vector3 middleScoopTo;

	// Use this for initialization
	void Start () {
        S = this;
        state_machine.ChangeState(new StatePlayerMove(this));
	}
	
	// Update is called once per frame
	void Update () {
        state_machine.Update();

        var topScoopPos = topScoop.transform.position;
        var middleScoopPos = middleScoop.transform.position;

        topScoopPos += (topScoopTo - topScoopPos) * 0.25f;
        middleScoopPos += (middleScoopTo - middleScoopPos) * 0.25f;

        topScoop.transform.position = topScoopPos;
        middleScoop.transform.position = middleScoopPos;
    }

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
            var rb = p.GetComponent<Rigidbody>();
            var vel = rb.velocity;

            float h_input = Input.GetAxis("Horizontal");
            bool v_input = Input.GetKeyDown(KeyCode.W);

            /*if(h_input != 0f)
                p.lastDirection = h_input > 0 ? 1 : -1;*/
            p.lastDirection = Utils.Sign(Utils.mouse.x - p.transform.position.x, false);
            if (v_input && p.grounded)
            {
                vel = new Vector3(vel.x, Mathf.Max(10, vel.y), 0);
                p.grounded = false;
            }


            p.topScoopTo = new Vector3(p.lastDirection * (0.3f - 0.5f * (p.topScoopReadyToThrow ? 1 : 0)), 0.55f - 0.15f * (h_input != 0f ? 1 : 0), 0) + pos;
            p.middleScoopTo = new Vector3(p.lastDirection * (-0.35f - 0.5f * (p.middleScoopReadyToThrow ? 1 : 0)), 0.5f - 0.2f * (p.middleScoopReadyToThrow ? 1 : 0) - 0.15f * Mathf.Min(Mathf.Abs(h_input), 1f), 0) + pos;
            p.jumpCollider.transform.position = new Vector3(0, -0.6f, 0) + pos;
            p.jumpCollider.transform.rotation = Quaternion.identity;
            p.face.transform.position = new Vector3(p.lastDirection * (0.4f), -0.1f - 0.1f * Mathf.Min(Mathf.Abs(h_input), 1f) + 0.15f * Mathf.Sign(vel.y) * Mathf.Min(Mathf.Max(Mathf.Abs(vel.y / 5), 0), 1), 0) + pos;
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
                    o.GetComponent<ScoopThrown>().ResetScaleToMass();
                    o.transform.position = p.topScoopTo;
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
                o.GetComponent<ScoopThrown>().ResetScaleToMass();
                o.transform.position = p.middleScoopTo;
                o.GetComponent<Rigidbody>().mass = Mathf.Min(Mathf.Max(p.middleScoopMass / 2f, 0.5f), p.middleScoopMass);
                p.middleScoopMass -= o.GetComponent<Rigidbody>().mass;

                var angleToMouse = Utils.GetAngleToMouse(o.transform.position);
                o.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(angleToMouse), Mathf.Sin(angleToMouse)) * p.middleScoopLaunchVelocity;// + p.GetComponent<Rigidbody>().velocity;

                o.GetComponent<ScoopThrown>().ExplodeInDirection();

                p.middleScoopReadyToThrow = false;
            }

            p.transform.position = pos;
        }
    }
}
