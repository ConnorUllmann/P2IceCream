using UnityEngine;
using System.Collections;

public class ParticleFade : MonoBehaviour {

    private float time;
    private float timeLast;
    private float timeMax;
    public void SetTime(float _t)
    {
        timeLast = Time.time;
        time = timeMax = _t;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        time -= Time.time - timeLast;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, time / timeMax);
        if (time <= 0)
            Destroy(this.gameObject);
        timeLast = Time.time;
    }
}
