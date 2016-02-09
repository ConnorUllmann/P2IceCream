using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public int difficulty = 1; // the estimated difficulty of this enemy
    public int waveRequirement = 1; // the wave at which this enemy begins to spawn
    public int spawnWeight = 10; // the relative frequency at which this enemy spawns

    public Vector3? target;
    public int direction = 1;
    public int directionToTarget { get { if (!target.HasValue) return 0; return Utils.Sign(target.Value.x - transform.position.x); } }
    public float distanceToTarget()
    {
        if (!target.HasValue)
            return -1;
        return (target.Value - transform.position).magnitude;
    }
    public float distanceToTargetX()
    {
        if (!target.HasValue)
            return -1;
        return Mathf.Abs(target.Value.x - transform.position.x);
    }

    public StateMachine state_machine = new StateMachine();

	public bool grounded = true;

    // Use this for initialization
    public virtual void Start ()
    {
	}
	
	// Update is called once per frame
	public virtual void Update ()
    {
        target = Player.S.transform.position;
        state_machine.Update();
    }

    public bool isRunning()
    {
        return rb().velocity.x != 0;
    }

    // when this enemy is destroyed/killed
    void OnDestroy() {
        // track difficulty
        EnemyManager.S.currentDifficulty -= this.difficulty;
    }

    public SpriteRenderer sprend() { return GetComponent<SpriteRenderer>(); }
    public Rigidbody rb() { return GetComponent<Rigidbody>(); }
}
