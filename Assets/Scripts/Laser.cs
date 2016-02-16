using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Laser : MonoBehaviour {

	LineRenderer line;
	public float damage = 1f;
	public float distance = 3f;

	public GameObject destructionPrefab;

	public bool _____________________;

	public Vector3 target;

	void Awake() {
		line = gameObject.GetComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.SetColors (Color.green, Color.green);
		line.enabled = false;
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update() {
		line.SetPosition (0, transform.position);
	}

	public void Fire(Vector3 targetPos) {
		Ray ray = new Ray (transform.position, targetPos - transform.position);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, distance)) {
			target = hit.point;
			line.SetPosition (1, target);
			if (hit.collider.tag == "Drop") {
				GameObject destructGO = Instantiate<GameObject> (destructionPrefab);
				destructGO.transform.position = hit.point;
				Destroy (destructGO, 1.0f);
				hit.collider.transform.parent.GetComponent<Drop> ().StealMass (0.9f);
			}
			if (hit.collider.tag == "Player") {
				Player.S.Damage (damage);
			}
		} else {
			target = ray.GetPoint (distance);
			line.SetPosition (1, target);
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
