using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    public int targetDifficulty = 10; // the difficulty that the manager aspires to reach
    public string[] enemySpawnerNames; // the names of the enemy spawners placed on the stage

    public float waveInterval = 30f; // the time interval between wave spawns (this is not a rest period, it always ticks down)

    public GameObject[] enemies; // the enemies that are available to be spawned

    public static EnemyManager S = null; // the singleton for this class

    private GameObject[] enemySpawnerGOs; // the actual game objects of the enemy spawners
    private int currentDifficulty = 10; // tracks the current difficulty of the stage
    private int waveNumber = 0; // this is the current wave number

	// contains initialization subroutines for enemy management
	void Awake () {
        if (S != null) { // if an enemy manager already exists
            Destroy(this);
            return;
        }
        S = this; // set the singleton instance

        // gets the spawner GameObjects by name
        enemySpawnerGOs = new GameObject[enemySpawnerNames.Length];
        for (int i = 0; i < enemySpawnerNames.Length; i++) {
            enemySpawnerGOs[i] = GameObject.Find(enemySpawnerNames[i]);
        }

        // starts wave 1 at start, and starts a new wave at every wave interval
        InvokeRepeating("NewWave", 0, waveInterval);
	}

    // This modifies the target difficulty depending on which wave is being fought
    void NewWave() {
        this.waveNumber++; // increment the wave counter
        this.targetDifficulty = currentDifficulty + 5 * waveNumber + (waveNumber * waveNumber) / 2; // this statement determines the difficulty increase per wave
        SpawnEnemies();
    }

    // spawns enemies until the difficulty is appropriate
    void SpawnEnemies() {
        // do nothing if the stage is currently too difficult
        if (currentDifficulty >= targetDifficulty) {
            return;
        }

        while (targetDifficulty - currentDifficulty > 0) { // until the game is at the correct difficulty
            GameObject newEnemy = ChooseEnemy(); // choose an appropriate enemy
            GameObject spawner = ChooseSpawner(); // choose an appropriate spawner
            spawner.GetComponent<EnemySpawner>().QueueEnemy(newEnemy); // add the enemy to the spawn queue
            currentDifficulty = newEnemy.GetComponent<Enemy>().difficulty; // update the current difficulty to reflect the change
        }
    }

    // chooses an enemy based on the wave number and available difficulty units
    GameObject ChooseEnemy() {
        // find which enemies are available to spawn on this wave
        List<GameObject> validEnemies = new List<GameObject>();
        int totalWeight = 0;
        foreach (GameObject enemy in this.enemies) {
            if (enemy.GetComponent<Enemy>().waveRequirement <= this.waveNumber // if the enemy can spawn this wave
                            && enemy.GetComponent<Enemy>().difficulty < targetDifficulty - currentDifficulty) { // and we have enough free difficulty units
                validEnemies.Add(enemy); // add the enemy as an option
                totalWeight += enemy.GetComponent<Enemy>().spawnWeight; // add its weight to the total spawn weight
            }
        }

        // if no enemies are available to spawn (or their weights are unset)
        if (totalWeight == 0 || validEnemies.Count == 0) {
            print("No enemies are available to spawn. Something is wrong.");
        }

        int chosenNumber = Random.Range(0, totalWeight); // choose a random integer between 0 and the total weight

        // use the chosen number to grab a random valid enemy with probability proportional to its weight
        GameObject chosenEnemy = null;
        foreach (GameObject enemy in validEnemies) { // for each valid enemy
            if (chosenNumber < enemy.GetComponent<Enemy>().spawnWeight) { // if the chosen number represents this enemy
                chosenEnemy = enemy; // choose this enemy
                break;
            }
            else {
                chosenNumber -= enemy.GetComponent<Enemy>().spawnWeight; // reduce the chosenNumber to reflect the next enemy's position in the weight pool
            }
        }

        if (chosenEnemy == null) { // if an enemy wasn't chosen
            print("No enemy was chosen to spawn, something has gone wrong."); // something is wrong
        }

        return chosenEnemy;

    }

    // chooses the spawner with the smallest spawn queue
    GameObject ChooseSpawner() {
        int minQueueSize = enemySpawnerGOs[0].GetComponent<EnemySpawner>().queueSize; // set a reasonable minimum
        GameObject minSpawner = enemySpawnerGOs[0];

        // check each spawner to find the one with the smallest spawn queue
        foreach (GameObject spawner in enemySpawnerGOs) {
            if (spawner.GetComponent<EnemySpawner>().queueSize < minQueueSize) {
                minSpawner = spawner;
                minQueueSize = spawner.GetComponent<EnemySpawner>().queueSize;
            }
        }

        return minSpawner; // return the chosen spawner
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
