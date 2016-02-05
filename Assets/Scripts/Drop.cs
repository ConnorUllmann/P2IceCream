using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Drop : MonoBehaviour {

    public bool isTopScoop;

    public GameObject diePrefab;
    public GameObject playerTrigger;

    private bool readyForPickup = false;

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

    public void Initialize(Vector3 _position, Sprite _sprite, float _mass, bool _isTopScoop)
    {
        lastPos = transform.position = _position;
        GetComponent<SpriteRenderer>().sprite = _sprite;
        GetComponent<Rigidbody>().mass = _mass;
        isTopScoop = _isTopScoop;
        ResetScaleToMass();
    }

    // Update is called once per frame
    void Update ()
    {
        if(Time.time - timeStart >= timeWaitToActivateColliders)
        {
            GetComponent<SphereCollider>().enabled = true;
            ResetScaleToMass();
        }

        if(readyForPickup && (transform.position - Player.S.transform.position).magnitude <= 2f * (playerTrigger.GetComponent<SphereCollider>().radius + Player.S.GetComponent<SphereCollider>().radius))
        {
            transform.position = (Player.S.transform.position - transform.position) * 0.25f + transform.position;
            GetComponent<Rigidbody>().isKinematic = false;
        }

        lastPos = transform.position;
    }

    void ResetScaleToMass()
    {
        var n = Mathf.Pow(GetComponent<Rigidbody>().mass, 0.3333f) / Mathf.Pow(isTopScoop ? Player.S.topScoopMassNormal : Player.S.middleScoopMassNormal, 0.3333f);
        playerTrigger.GetComponent<SphereCollider>().radius = n / 2 * (isTopScoop ? 1f : 1.5f);
        GetComponent<SpriteRenderer>().transform.localScale = n * Vector3.one;
    }

    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.tag == "Tile")
        {
            readyForPickup = true;

            RaycastHit hitInfo;
            if(Physics.Raycast(lastPos, transform.position - lastPos, out hitInfo, GetComponent<Rigidbody>().velocity.magnitude * 10f))
                transform.position = hitInfo.point;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            //GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Tile")
        {
            GetComponent<Rigidbody>().isKinematic = false;
            //GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public float StealMass()
    {
        if (!readyForPickup)
            return 0;

        var m = GetComponent<Rigidbody>().mass;
        var v = m / 4f;
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