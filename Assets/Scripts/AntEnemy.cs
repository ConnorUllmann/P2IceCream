using UnityEngine;
using System.Collections;

public class AntEnemy : Enemy {

	public Sprite[] spriteWalk;
	public int spriteWalkSpeed;
	public float walkSpeed;
	public float damage;
	public float health;
	public float bounceOffPlayerSpeed = 3f;
	public bool dropToggle;
	public int campfireCount = 0;
	public GameObject campfirePrefab;

	public bool _______________________;

	public int tilePhysicsLayerMask;

	// Use this for initialization
	override public void Start() 
	{
		base.Start ();
		state_machine.ChangeState (new StateAntEnemyDrop (this));

		// Set tilePhysicsLayerMask
		tilePhysicsLayerMask = LayerMask.GetMask("Tile");
	}

	// Update is called once per frame
	override public void Update()
	{
		base.Update ();
	}

	public void Damage(float _damage)
	{
		if (health > 0) {
			health -= _damage;
			StartCoroutine (Flash ());
			if (health <= 0) {
				Player.Screenshake (0.3f, 1);
				Destroy (this.gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision c) 
	{
		if (c.gameObject.tag == "Player") 
		{
			Player.S.Damage (damage);

			if (grounded) {
				transform.Rotate (0, 180, 0);
			} else {
				var v = (transform.position - Player.S.transform.position).normalized;
				rb ().velocity += v * bounceOffPlayerSpeed;
			}
		}
	}

	IEnumerator Flash() {
		for (int i = 0; i < 2; ++i) {
			sprend ().enabled = false;
			yield return new WaitForSeconds (0.05f);
			sprend ().enabled = true;
		}
	}

	public class StateAntEnemyNormal : State
	{
		AntEnemy p;
		private float totalTime;
		public int prevAction = 1; // 0: rotate left, 1: straight, 2: rotate right

		public StateAntEnemyNormal(AntEnemy _p)
		{
			p = _p;
			totalTime = 0;
			p.dropToggle = false;
		}

		public override void OnStart() {
			p.rb ().useGravity = false;
		}

		public override void OnUpdate(float time_delta_fraction)
		{
			var vel = p.rb ().velocity;
			bool moveTo = true; // Whether the enemy should attempt to move

			RaycastHit hitInfo;
			var hit = Physics.Raycast (p.transform.position, -1 * p.transform.right, out hitInfo, 0.5f, p.tilePhysicsLayerMask);

			if (hit) {
				p.transform.Rotate (0, 0, 270);
				prevAction = 2;
			} else if (p.grounded) {
				prevAction = 1;
			} else {
				if (prevAction != 0) {
					p.transform.Rotate (0, 0, 90);
					prevAction = 0;
				}
			}
			vel = p.transform.right * -1 * p.walkSpeed;

			if (p.isRunning ()) {
				p.sprend ().sprite = p.spriteWalk [(int)Mathf.Abs (totalTime * p.spriteWalkSpeed / p.walkSpeed) % p.spriteWalk.Length];
			}
				
			p.rb ().velocity = vel;
			totalTime = Mathf.Max (totalTime + Mathf.Abs (time_delta_fraction), 0);

			if (Mathf.Abs (p.transform.position.x - Player.S.transform.position.x) <= 0.5
				&& p.transform.position.y > Player.S.transform.position.y
				&& Mathf.Abs(p.transform.rotation.z) == 1) {
				p.state_machine.ChangeState (new StateAntEnemyDrop (p));
			}

			// Builds campfire
			++p.campfireCount;
			if (p.campfireCount >= 200) {
				if (p.transform.rotation.z == 0) {
					GameObject campfireGO = Instantiate<GameObject> (p.campfirePrefab);
					campfireGO.transform.position = p.transform.position;
				}

				p.campfireCount = 0;
			}
		}
	}

	public class StateAntEnemyDrop : State
	{
		AntEnemy p;

		private float totalTime;

		public StateAntEnemyDrop(AntEnemy _p)
		{
			p = _p;
			totalTime = 0;
			p.dropToggle = true;
		}

		public override void OnStart() {
			p.rb().useGravity = true;
			p.rb().velocity = new Vector3(0,0,0);
			p.transform.rotation = Quaternion.identity;

		}

		public override void OnUpdate(float time_delta_fraction)
		{
			if (Player.S.transform.position.x > p.transform.position.x) {
				p.transform.rotation = Quaternion.Euler (0, 180, 0);
			}
			p.sprend ().sprite = p.spriteWalk [(int)Mathf.Abs (totalTime * p.spriteWalkSpeed * p.walkSpeed / p.walkSpeed) % p.spriteWalk.Length];
			totalTime = Mathf.Max (totalTime + Mathf.Abs (time_delta_fraction), 0);

			if (p.grounded) {
				p.state_machine.ChangeState (new StateAntEnemyNormal (p));
			}
		}

	}
}
