using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	LineRenderer line;
	public float damage = 1f;
	public float distance = 3f;

	// Use this for initialization
	void Start () {
		line = gameObject.GetComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.SetColors (Color.green, Color.green);
		line.enabled = false;
	}


	// Update is called once per frame
	void Update() {
		
	}

	public void Fire(Vector3 targetPos) {

		Ray ray = new Ray (transform.position, targetPos - transform.position);
		RaycastHit hit;

		line.SetPosition (0, ray.origin);

		if (Physics.Raycast (ray, out hit, distance)) {
			line.SetPosition (1, hit.point);
			if (hit.collider.tag == "Drop") {
				hit.collider.transform.parent.GetComponent<Drop> ().StealMass (0.9f);
			}
			if (hit.collider.tag == "Player") {
				Player.S.Damage (damage);
			}
		} else {
			line.SetPosition (1, ray.GetPoint (distance));
		}
	}

	public void ActivateLaser() {
		if (!line.enabled) {
			line.enabled = true;
		}
	}

	public void DisableLaser() {
		if (line.enabled) {
			line.enabled = false;
		}
	}
}
