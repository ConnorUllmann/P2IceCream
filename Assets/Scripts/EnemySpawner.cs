using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

    public float spawnInterval = 3f; // how often (in seconds) the spawner can spawn an enemy

    private Queue<GameObject> spawnQueue = new Queue<GameObject>(); // the enemies that are queued to spawn

    // this property represents the number of enemies waiting to be spawned
    public int queueSize {
        get {
            return spawnQueue.Count;
        }
    }

	// Use this for initialization
	void Start () {
        // spawns an enemy at the spawn interval
        InvokeRepeating("SpawnEnemy", 0, spawnInterval);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // add an enemy to the spawn queue
    public void QueueEnemy(GameObject enemy) {
        spawnQueue.Enqueue(enemy);
    }

    public void SpawnEnemy() {
        // if the queue is empty, do nothing
        if (spawnQueue.Count == 0) {
            return;
        }

        // spawn an enemy
        GameObject enemy = spawnQueue.Dequeue();
        GameObject enemyGO = Instantiate(enemy);

        // put the enemy in the correct place
        enemyGO.transform.position = this.transform.position;

    }
}
