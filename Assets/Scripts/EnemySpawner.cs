using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

    public float spawnInterval = 3f; // how often (in seconds) the spawner can spawn an enemy
    //public float spawnDelay = 1f; // how long it takes to spawn an enemy

    private Queue<GameObject> spawnQueue = new Queue<GameObject>(); // the enemies that are queued to spawn

    // this property represents the number of enemies waiting to be spawned
    public int queueSize {
        get {
            return spawnQueue.Count;
        }
    }

	// Use this for initialization
	void Start () {
        portalStartScale = portalEffect.transform.localScale;
        fireStartScale = fireEffect.transform.localScale;
        fireBackStartScale = fireBackEffect.transform.localScale;
        lastTime = Time.time;
        spawnTimer = spawnInterval;
        stretch = stretchMin;
        UpdatePortalSize();
        // spawns an enemy at the spawn interval
/*<<<<<<< HEAD
        //InvokeRepeating("SpawnEnemy", 0, spawnInterval);
=======
        InvokeRepeating("ManageSpawn", 0, spawnInterval);
>>>>>>> origin/master*/
	}

    public GameObject portalEffect;
    public GameObject fireEffect;
    public GameObject fireBackEffect;
    public Vector3 portalStartScale;
    public Vector3 fireStartScale;
    public Vector3 fireBackStartScale;
    float stretch;
    float stretchMin = 0.0125f;
    float portalTimeOpenBeforeSpawn = 0.6f;
    float portalTimeOpenAfterSpawn = 0f;
    bool open = false;
    float lastTime = 0;
    float spawnTimerWait = 5f; //Amount of time to wait before starting to spawn
    float spawnTimer = 0;
	// Update is called once per frame
	void Update ()
    {
        var dt = Time.time - lastTime;

        if (spawnTimerWait > 0)
            spawnTimerWait -= dt;
        else
        {
            spawnTimer += dt;
            open = spawnQueue.Count > 0 && 
                   (spawnTimer % spawnInterval <= portalTimeOpenAfterSpawn ||
                    spawnTimer % spawnInterval >= spawnInterval - portalTimeOpenBeforeSpawn);
        }
        if(spawnTimer >= spawnInterval)
        {
            spawnTimer %= spawnInterval;
            SpawnEnemy();
        }

        if (open)
        {
            stretch = Mathf.Max(Mathf.Min(stretch + 0.015f, 1), stretchMin);
        }
        else
        {
            stretch = Mathf.Max(Mathf.Min(stretch - 0.01f, 1), stretchMin);
        }
        UpdatePortalSize();

        lastTime = Time.time;
    }

    void UpdatePortalSize()
    {
        var n = Utils.easeOutElastic(stretch);
        transform.localScale = new Vector3(n, transform.localScale.y, 1);
        portalEffect.transform.localScale = new Vector3(portalStartScale.x * n, portalStartScale.y, portalStartScale.z);
        fireEffect.transform.localScale = new Vector3(fireStartScale.x * n, fireStartScale.y, fireStartScale.z);
        fireBackEffect.transform.localScale = new Vector3(fireBackStartScale.x * n, fireBackStartScale.y, fireBackStartScale.z);
    }

    /*void ManageSpawn() {
        // if the queue is empty, do nothing
        if (spawnQueue.Count == 0) {
            return;
        }
        SpawnEffectOn();
        Invoke("SpawnEffectOff", this.spawnDelay);
        Invoke("SpawnEnemy", this.spawnDelay);
    }

    // turns on the spawn effect
    void SpawnEffectOn() {
        this.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
    }

    // turns off the spawn effect
    void SpawnEffectOff() {
        this.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
    }*/

    // add an enemy to the spawn queue
    public void QueueEnemy(GameObject enemy) {
        spawnQueue.Enqueue(enemy);
    }

    public void SpawnEnemy() {
        // if the queue is empty, do nothing
        if (spawnQueue.Count == 0) {
            print("Tried to spawn an enemy with nothing in the spawn queue!");
            return;
        }
        // spawn an enemy
        GameObject enemy = spawnQueue.Dequeue();
        GameObject enemyGO = Instantiate(enemy);

        // put the enemy in the correct place
        enemyGO.transform.position = this.transform.position;

    }
}
