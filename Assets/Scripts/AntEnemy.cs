using UnityEngine;
using System.Collections;

public class AntEnemy : Enemy {

	public Sprite[] spriteWalk;
	public int spriteWalkSpeed;

	public float walkSpeed;

	public float damage;

	public bool _______________________;

	// Use this for initialization
	override public void Start() 
	{
		base.Start ();
		state_machine.ChangeState (new StateAntEnemyNormal (this));
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

			var v = (transform.position - Player.S.transform.position).normalized;
			rb ().velocity += v * 3f;
		}
	}

	public class StateAntEnemyNormal : State
	{
		AntEnemy p;
		private float totalTime;

		public StateAntEnemyNormal(AntEnemy _p)
		{
			p = _p;
			totalTime = 0;
		}

		public override void OnUpdate(float time_delta_fraction)
		{
			p.direction = p.directionToTarget;

			var vel = p.rb ().velocity;
			bool moveTo = true; // Whether the enemy should attempt to move

			if (p.isRunning ()) {
				p.sprend ().flipX = p.direction == 1;
				p.sprend ().sprite = p.spriteWalk [(int)Mathf.Abs (totalTime * p.spriteWalkSpeed * p.rb ().velocity.x / p.walkSpeed) % p.spriteWalk.Length];
			}

			if (p.distanceToTargetX () < 1)
				moveTo = false;

			if (p.grounded) {
				if (moveTo)
					vel += (new Vector3 (p.walkSpeed * p.direction, vel.y, 0) - vel) * 0.25f;
			}
			p.rb ().velocity = vel;
			totalTime = Mathf.Max (totalTime + Mathf.Abs (time_delta_fraction), 0);
		}
	}
}
