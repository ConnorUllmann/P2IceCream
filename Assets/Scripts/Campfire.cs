using UnityEngine;
using System.Collections;

public class Campfire : MonoBehaviour {

	public float damage = 1f;
	public float health = 1f;
	public float dropAmount = 0.1f;

	public GameObject dropPrefab;

	public bool _____________________;

	public ParticleSystem fireParticle;
	public SpriteRenderer wood;

	// Use this for initialization
	void Start () {
		fireParticle = transform.Find ("Particle System").GetComponent<ParticleSystem> ();
		wood = transform.Find ("modelBonfire").GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Damage(float _damage) {
		if (health > 0) {
			health -= _damage;
			fireParticle.startSize = health;
			GetComponent<CapsuleCollider> ().radius = 0.2f * health;
			if (health <= 0) {
				LeaveDrops ();
				Destroy (this.gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "Player") {
			Player.S.Damage (damage);
		}
	}

	public void LeaveDrops() {
		int rand = Random.Range (1, 4);
		float total = 0;
		while (total <= dropAmount) {
			float dropS = Random.Range (0.01f, 0.05f);
			Vector3 pos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
			GameObject dropGO = Instantiate<GameObject> (dropPrefab);
			switch (rand) {
			case 1:
				dropGO.GetComponent<Drop> ().Initialize (pos, dropS, Utils.IceCream.White);
				break;
			case 2:
				dropGO.GetComponent<Drop> ().Initialize (pos, dropS, Utils.IceCream.Pink);
				break;
			case 3:
				dropGO.GetComponent<Drop> ().Initialize (pos, dropS, Utils.IceCream.Brown);
				break;
			}
			dropAmount -= dropS;
		}
	}
}
