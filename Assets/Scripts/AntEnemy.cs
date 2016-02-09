using UnityEngine;
using System.Collections;

public class AntEnemy : Enemy {

	public Sprite[] spriteWalk;
	public int spriteWalkSpeed;
	public float walkSpeed;
	public float damage;

	public bool _______________________;

	public int tilePhysicsLayerMask;

	// Use this for initialization
	override public void Start() 
	{
		base.Start ();
		state_machine.ChangeState (new StateAntEnemyNormal (this));

		// Set tilePhysicsLayerMask
		tilePhysicsLayerMask = LayerMask.GetMask("Tile");
	}

	// Update is called once per frame
	override public void Update()
	{
		base.Update ();
	}

	void OnCollisionEnter(Collision c) 
	{
		if (c.gameObject.tag == "Player") 
		{
			Player.S.Damage (damage);

			transform.Rotate (0, 180, 0);
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

			// Temporary: handling bounds when enemy should be destroyed
			if (p.transform.position.y > 5.9f || p.transform.position.x > 35.8f) {
				Destroy (p.gameObject);
			}
		}
	}
}
