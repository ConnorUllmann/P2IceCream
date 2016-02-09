using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlCollider : MonoBehaviour {

	private List<GameObject> hit;

	// Use this for initialization
	void Start () {
		hit = new List<GameObject> ();
	}
	
	void OnTriggerEnter(Collider c)
	{
		hit.Add (c.gameObject);
		if (c.gameObject.tag == "Tile") {
			Enemy e = GetComponentInParent<Enemy> ();
			if (e)
				e.grounded = true;
		}
	}

	void OnTriggerExit(Collider c)
	{
		if (hit.Contains (c.gameObject))
			hit.Remove (c.gameObject);

		bool collidingWithGround = false;
		for (int i = 0; i < hit.Count; i++) {
			if (hit [i] == null) {
				hit.RemoveAt (i--);
				continue;
			}
			if (hit [i].tag == "Tile") {
				collidingWithGround = true;
				break;
			}
		}

		if (!collidingWithGround) {
			Enemy e = GetComponentInParent<Enemy> ();
			if (e)
				e.grounded = false;
		}
	}
}
