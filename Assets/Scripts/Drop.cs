using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Drop : MonoBehaviour {

    public enum IceCream { Brown, White, Pink };
    public static float[] massNormals = new float[3] { 3, 2, 1 };
    public static float[] sizeMults = new float[3] { 2, 1.5f, 1 };
    public Sprite[] sprites;
    public float[] damage = new float[3] { 1, 1, 1 };
    public float[] knockback = new float[3] { 3, 1, 1 };

    public IceCream type;
    private float scoopMassNormal;
    private float scoopSizeMult;

    public GameObject diePrefab;
    public GameObject playerTrigger;

    private bool hitWall = false;
    private bool attached = false; //Don't make this true

    private Vector3 lastPos;

    private float timeWaitToActivateColliders = 0f;
    private float timeStart;
	// Use this for initialization
	void Start ()
    {
        GetComponent<SphereCollider>().radius = 0.0001f;
        GetComponent<SphereCollider>().enabled = false;
        timeStart = Time.time;
        playerTrigger.GetComponent<PlayerTrigger>().drop = this;
    }

    public void Initialize(Vector3 _position, float _mass, IceCream _type)
    {
        lastPos = transform.position = _position;
        GetComponent<SpriteRenderer>().sprite = sprites[(int)_type];
        GetComponent<Rigidbody>().mass = _mass;
        type = _type;
        scoopMassNormal = massNormals[(int)type];
        scoopSizeMult = sizeMults[(int)type];
        ResetScaleToMass();
    }

    // Update is called once per frame
    void Update ()
    {
        if (attached)
        {
            var e = transform.parent.gameObject.GetComponent<BasicEnemy>();
            if (e)
            {
                e.Damage(StealMass(0.001f) * damage[(int)type]);// GetComponent<Rigidbody>().mass * damage[(int)type]);
                ResetScaleToMass();
            }
            return;
        }

        if(Time.time - timeStart >= timeWaitToActivateColliders)
        {
            GetComponent<SphereCollider>().enabled = true;
            ResetScaleToMass();
        }

        if(hitWall && Player.S.hurtTimer <= 0 && 
          (transform.position - Player.S.transform.position).magnitude <= 2f * (playerTrigger.GetComponent<SphereCollider>().radius + Player.S.GetComponent<SphereCollider>().radius))
        {
            transform.position = (Player.S.transform.position - transform.position) * 0.25f + transform.position;
            GetComponent<Rigidbody>().isKinematic = false;
        }

        lastPos = transform.position;
    }

    void ResetScaleToMass()
    {
        var n = Mathf.Pow(GetComponent<Rigidbody>().mass, 0.3333f) / Mathf.Pow(scoopMassNormal, 0.3333f);
        playerTrigger.GetComponent<SphereCollider>().radius = n / 2 * scoopSizeMult;
        GetComponent<SpriteRenderer>().transform.localScale = n * Vector3.one;
    }
    
    void OnTriggerEnter(Collider c)
    {
        if (attached)
            return;
        if (c.gameObject.tag == "BasicEnemy")
        {
            if (!hitWall && (type == IceCream.White || type == IceCream.Pink))
            {
                var v = GetComponent<Rigidbody>().velocity * GetComponent<Rigidbody>().mass * knockback[(int)type];
                v.y = Mathf.Abs(v.y);
                c.gameObject.GetComponent<Rigidbody>().velocity += v;

                c.gameObject.GetComponent<BasicEnemy>().Damage(GetComponent<Rigidbody>().mass * damage[(int)type]);
            }
            else
            {
                var v = StealMass(0.8f, true) * damage[(int)type];
                c.gameObject.GetComponent<BasicEnemy>().Damage(v);
            }
            /*
            GetComponent<Rigidbody>().isKinematic = true;
            attached = true;
            this.transform.parent = c.gameObject.transform;
            RaycastHit hitInfo;
            var hit = Physics.Raycast(lastPos, GetComponent<Rigidbody>().velocity, out hitInfo);
            if(hit)
            {
                this.transform.position = hitInfo.point;
            }*/
        }

		if (c.gameObject.tag == "AntEnemy") {
			if (!hitWall && (type == IceCream.White || type == IceCream.Pink)) {
				var v = GetComponent<Rigidbody> ().velocity * GetComponent<Rigidbody> ().mass * knockback [(int)type];
				v.y = Mathf.Abs (v.y);
				c.gameObject.GetComponent<Rigidbody> ().velocity += v;

				c.gameObject.GetComponent<AntEnemy> ().Damage (GetComponent<Rigidbody> ().mass * damage [(int)type]);
			} else {
				var v = StealMass (0.8f, true) * damage [(int)type];
				c.gameObject.GetComponent<AntEnemy> ().Damage (v);
			}
		}
    }

    void OnTriggerStay(Collider c)
    {
        if (attached)
            return;
        if (c.gameObject.tag == "Tile")
        {
            hitWall = true;

            RaycastHit hitInfo;
            if(Physics.Raycast(lastPos, transform.position - lastPos, out hitInfo, GetComponent<Rigidbody>().velocity.magnitude * 2f))
                transform.position = hitInfo.point;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            //GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (attached)
            return;
        if (c.gameObject.tag == "Tile")
        {
            GetComponent<Rigidbody>().isKinematic = false;
            //GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public float StealMass(float _rate=0.25f, bool always = false)
    {
        if (!always && !hitWall && !attached)
            return 0;

        var m = GetComponent<Rigidbody>().mass;
        var v = m * _rate;
        if (m < 0.1f)
        {
            Destroy(this.gameObject);
            return m;
        }
        else
        {
            GetComponent<Rigidbody>().mass -= v;
        }
        return v;
    }

    //Launches the particle from a position at a speed in a random direction.
    public void LaunchRandomRadially(Vector3 from, float speed)
    {
        LaunchDirection(from, speed, Mathf.PI * 2 * Random.value);
    }
    public void LaunchDirection(Vector3 from, float speed, float angle)
    {
        transform.position = from;
        GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * speed;
    }
}
