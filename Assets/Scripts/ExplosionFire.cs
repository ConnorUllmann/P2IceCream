using UnityEngine;
using System.Collections;

public class ExplosionFire : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<ParticleSystem>().particleCount <= 0)
            Destroy(this.gameObject);
	}
}
