using UnityEngine;
using System.Collections;

public class ThrownScoop : MonoBehaviour {

    public GameObject diePrefab;
    public GameObject dropPrefab;
    
    public Drop.IceCream type;
    private float scoopMassNormal;

    private Vector3 lastPos;

	// Use this for initialization
	void Start () {
        //type = gameObject.name.Contains("Top") ? Player.S.topScoopType : Player.S.middleScoopType; //Hacky way to auto-detect that this scoop is a top scoop or bottom scoop.
        

        ResetScaleToMass();
        lastPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        ResetScaleToMass();
        lastPos = transform.position;
    }

    public void ResetScaleToMass()
    {
        scoopMassNormal = Drop.massNormals[(int)type];
        var n = Mathf.Pow(GetComponent<Rigidbody>().mass, 0.3333f) / Mathf.Pow(scoopMassNormal, 0.3333f);
        GetComponent<SphereCollider>().radius = n / 2;
        GetComponent<SpriteRenderer>().transform.localScale = n * Vector3.one;
    }

    void OnCollisionEnter(Collision c)
    {
		if (c.gameObject.tag == "Tile" || c.gameObject.tag == "BasicEnemy" || c.gameObject.tag == "MrLickums" || c.gameObject.tag == "AntEnemy")
        {
            var rb = GetComponent<Rigidbody>();
            var massToReleaseStart = rb.mass;// * (Random.value * 0.9f + 0.1f);
            var massToRelease = massToReleaseStart;
            int numDrops = (int)(Random.value * 5 + 10);

            var contact = c.contacts[0];
            
            var pos = transform.position;
            transform.position = lastPos;
            while (massToRelease > 0)
            {
                float massForThisDrop = Mathf.Min(massToReleaseStart / numDrops * (0.5f + Random.value), massToRelease);
                SpawnDrop(massForThisDrop, Mathf.Atan2(contact.normal.y, contact.normal.x));
                massToRelease -= massForThisDrop;
            }
            transform.position = pos;
            CreateDeathPrefabInstance();
            Destroy(this.gameObject);
            //rb.mass -= massToReleaseStart;
        }
    }

    void CreateDeathPrefabInstance()
    {
        var d = Instantiate<GameObject>(diePrefab);
        d.GetComponent<ParticleFade>().SetTime(0.05f);
        d.transform.position = transform.position;
        d.transform.localScale = transform.localScale;
        d.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        d.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
    }

    public void ExplodeInDirection()
    {
        var rb = GetComponent<Rigidbody>();
        var massToReleaseStart = rb.mass;
        var massToRelease = massToReleaseStart;
        int numDrops = type == Drop.IceCream.Pink ? (int)(Random.value * 20 + 20) : (int)(Random.value * 5 + 10);

        while (massToRelease > 0)
        {
            float massForThisDrop = Mathf.Min(massToReleaseStart / numDrops * (0.1f + 1.9f * Random.value), massToRelease);
            SpawnDropDirection(massForThisDrop, GetComponent<Rigidbody>().velocity.magnitude, Mathf.Atan2(GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.x));
            massToRelease -= massForThisDrop;
        }
        Destroy(this.gameObject);
    }

    void SpawnDropDirection(float _mass, float _speed, float _angle)
    {
        var o = Instantiate<GameObject>(dropPrefab);
        o.GetComponent<Drop>().Initialize(transform.position, _mass, type);
        o.GetComponent<Drop>().LaunchDirection(transform.position, _speed, _angle + Mathf.PI / 20 * (-1 + 2 * Random.value));
    }

    void SpawnDrop(float _mass, float _angle)
    {
        var o = Instantiate<GameObject>(dropPrefab);
        o.GetComponent<Drop>().Initialize(transform.position, _mass, type);
        o.GetComponent<Drop>().LaunchRandomRadially(transform.position, 5 * Random.value + 10f);
    }
}
