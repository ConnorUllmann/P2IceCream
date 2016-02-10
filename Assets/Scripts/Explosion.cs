using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public float dieTimerMax;
    public float dieTimer;

    private float lastTime;
    private Vector3 scale;

	// Use this for initialization
	void Start ()
    {
        lastTime = Time.time;
	}

    public void Initialize(float _time, Vector3 _scale)
    {
        dieTimerMax = dieTimer = _time;
        scale = _scale;
        transform.parent.localScale = scale;
    }
	
	// Update is called once per frame
	void Update ()
    {
        var dt = Time.time - lastTime;

        dieTimer -= dt;
        var dieTimerNormal = dieTimer / dieTimerMax;
        if(dieTimerNormal < 0.75f)
            GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        if (dieTimer <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            transform.localScale = scale * dieTimerNormal * 10f;
        }

        lastTime = Time.time;
	}
}
