using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	LineRenderer line;
	public int counter = 220;
	public float damage = 1f;
	public float distance = 3f;
	public bool laser = true;

	// Use this for initialization
	void Start () {
		line = gameObject.GetComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.SetColors (Color.green, Color.green);
		line.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.parent.GetComponent<AntEnemy> ().dropToggle) {
			DisableLaser ();
		} else {
			counter -= 1;
			if (counter == 0) {
				laser = true;
				StartCoroutine (FireLaser ());
				counter = 220;
			}
		}
	}

	IEnumerator FireLaser() {
		for (int i = 0; i < 72; ++i) {
			if (!laser) {
				break;
			}
			line.enabled = true;

			Ray ray = new Ray (transform.position, -1 * transform.right);
			RaycastHit hit;

			line.SetPosition (0, ray.origin);

			if (Physics.Raycast (ray, out hit, distance)) {
				line.SetPosition (1, hit.point);
				if (hit.collider.tag == "Player") {
					Player.S.Damage (damage);
				}
			} else {
				line.SetPosition (1, ray.GetPoint (distance));
			}

			yield return new WaitForSeconds (0.05f);
			line.enabled = false;
			transform.Rotate (0, 0, -5);
		}

	}

	void DisableLaser() {
		laser = false;
		StopCoroutine (FireLaser ());
		line.enabled = false;

		// Reset laser start position
		transform.rotation = Quaternion.identity;
	}
}
