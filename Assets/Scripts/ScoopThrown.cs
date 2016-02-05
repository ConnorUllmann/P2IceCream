using UnityEngine;
using System.Collections;

public class ScoopThrown : MonoBehaviour {

    public GameObject diePrefab;
    public GameObject dropPrefab;

    private bool isTopScoop;

    private Vector3 lastPos;

	// Use this for initialization
	void Start () {
        isTopScoop = gameObject.name.Contains("Top"); //Hacky way to auto-detect that this scoop is a top scoop or bottom scoop.
        ResetScaleToMass();
    }
	
	// Update is called once per frame
	void Update () {
        ResetScaleToMass();
        lastPos = transform.position;
    }

    public void ResetScaleToMass()
    {
        var n = Mathf.Pow(GetComponent<Rigidbody>().mass, 0.3333f) / Mathf.Pow(isTopScoop ? Player.S.topScoopMassNormal : Player.S.middleScoopMassNormal, 0.3333f);
        GetComponent<SphereCollider>().radius = n / 2;
        GetComponent<SpriteRenderer>().transform.localScale = n * Vector3.one;
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Tile")
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
        d.GetComponent<ParticleFade>().SetTime(0.2f);
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
        int numDrops = (int)(Random.value * 5 + 10);

        while (massToRelease > 0)
        {
            float massForThisDrop = Mathf.Min(massToReleaseStart / numDrops * (0.1f + 1.9f * Random.value), massToRelease);
            SpawnDropDirection(massForThisDrop, Mathf.Atan2(GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.x));
            massToRelease -= massForThisDrop;
        }
        Destroy(this.gameObject);
    }

    void SpawnDropDirection(float _mass, float _angle)
    {
        var o = Instantiate<GameObject>(dropPrefab);
        o.GetComponent<Drop>().Initialize(transform.position, GetComponent<SpriteRenderer>().sprite, _mass, isTopScoop);
        o.GetComponent<Drop>().LaunchDirection(transform.position, 10, _angle + Mathf.PI / 20 * (-1 + 2 * Random.value));
    }

    void SpawnDrop(float _mass, float _angle)
    {
        var o = Instantiate<GameObject>(dropPrefab);
        var len = 0.5f * (Random.value * 2 - 1);
        var diff = Vector3.zero;// new Vector3(len * Mathf.Cos(_angle + Mathf.PI / 2), len * Mathf.Sin(_angle + Mathf.PI / 2));
        o.GetComponent<Drop>().Initialize(transform.position + diff, GetComponent<SpriteRenderer>().sprite, _mass, isTopScoop);
        //Debug.Log("angle: " + _angle);
        //o.GetComponent<Drop>().LaunchDirection(transform.position, 10, _angle + Mathf.PI / 8 * (-1 + 2 * Random.value));
        o.GetComponent<Drop>().LaunchRandomRadially(transform.position, 5 * Random.value + 10f);
    }
}
