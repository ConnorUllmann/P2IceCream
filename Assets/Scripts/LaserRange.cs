using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserRange : MonoBehaviour {

	public Laser laser;

	private List<GameObject> hitObject;

	// Use this for initialization
	void Start () {
		laser = transform.parent.Find ("Gun").GetComponent<Laser> ();

		hitObject = new List<GameObject> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "Drop" || c.gameObject.tag == "Player") {
			hitObject.Add (c.gameObject);
			laser.ActivateLaser ();
			laser.Fire (c.transform.position);
		}
	}

	void OnTriggerStay(Collider c) {
		if (c.gameObject.tag == "Player") {
			laser.Fire (c.transform.position);
		}
	}

	void OnTriggerExit(Collider c) {
		if (hitObject.Contains (c.gameObject)) {
			hitObject.Remove (c.gameObject);
		}

		for (int i = 0; i < hitObject.Count; i++) {
			if (hitObject [i] == null) {
				hitObject.RemoveAt (i--);
			}
		}

		if (hitObject.Count == 0) {
			laser.DisableLaser ();
		}
	}
}
