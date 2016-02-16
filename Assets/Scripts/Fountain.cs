using UnityEngine;
using System.Collections;

public class Fountain : MonoBehaviour {

    public Utils.IceCream iceCreamType = Utils.IceCream.White; // the type of ice cream that drops
    public float rechargeTime = 10f; // the amount of time it takes to recharge the fountain
    public float amount = 0.5f; // the total mass of ice cream that drops on activation
    public float dropSize = 0.01f; // the mass of each ice cream droplet
    public float spawnVelocity = 0.5f; // the speed with which the ice cream droplets are expelled
    public float spawnInterval = 0.1f; // the amount of time between droplet spawns
    public GameObject dropPrefab; // the drop prefab

    private float amountDropped = 0f; // the amount of ice cream dropped since last recharge
    private bool recharged = true;
    private Vector3 sprayDirection; // the direction in which to spray ice cream


	// Use this for initialization
	void Start () {
        Invoke("Recharge", rechargeTime); // start uncharged, recharge in rechargeTime

        this.sprayDirection = ChooseDirection(); // set the direction of the fountain spray
	}

    // chooses a direction for the fountain spray based on which sides of the fountain connect with tiles
    Vector3 ChooseDirection() {
        Utils.Connector conn = Utils.ConnectsTo(this.transform.position);
        Vector3 direction;

        // spray in the opposite direction of the tiles the fountain is connected to
        switch(conn)
        {
            case Utils.Connector.All:
            case Utils.Connector.EastWest:
            case Utils.Connector.None:
            case Utils.Connector.South:
            case Utils.Connector.SouthEastWest:
                direction = new Vector3(0, 1, 0); // spray North
                break;
            case Utils.Connector.North:
            case Utils.Connector.NorthEastWest:
                direction = new Vector3(0, -1, 0); // spray South
                break;
            case Utils.Connector.NorthSouthWest:
            case Utils.Connector.West:
                direction = new Vector3(1, 0, 0); // spray East
                break;
            case Utils.Connector.East:
            case Utils.Connector.NorthSouth:
            case Utils.Connector.NorthSouthEast:
                direction = new Vector3(-1, 0, 0); // spray West
                break;
            case Utils.Connector.NorthEast:
                direction = new Vector3(-1, -1, 0); // spray SouthWest
                break;
            case Utils.Connector.NorthWest:
                direction = new Vector3(1, -1, 0); // spray SouthEast
                break;
            case Utils.Connector.SouthEast:
                direction = new Vector3(-1, 1, 0); // spray NorthWest
                break;
            case Utils.Connector.SouthWest:
                direction = new Vector3(1, 1, 0); // spray NorthEast
                break;
            default:
                print("The fountain was detected to connect to an impossible number of tiles @.@");
                direction = new Vector3(0, 1, 0); // spray North
                break;

        }

        return direction;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // recharge the fountain
    void Recharge() {
        this.recharged = true;
    }

    // mark the fountain as inactive
    void Discharge() {
        this.recharged = false;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") { // when the player is near
            dropIceCream(); // drop ice cream
        }
    }

    Vector3 RandomizeSpray()
    {
        Vector3 direction = this.sprayDirection; // get the initial direction
        float rotation = Random.Range(-45f, 45f); // get a random angle
        direction = Quaternion.Euler(0, 0, rotation) * direction; // apply the random rotation to the spray direction
        return direction;
    }

    void dropIceCream() {
        if (!recharged) {
            return; // don't spawn ice cream if the fountain isn't charged
        }
        InvokeRepeating("spawnDrop", 0.0f, this.spawnInterval); // start dropping ice cream
    }

    void spawnDrop()
    {
        GameObject drop = Instantiate(dropPrefab); // instantiate a drop
        drop.GetComponent<Drop>().Initialize(this.transform.position, this.dropSize, this.iceCreamType); // tell the drop its parameters
        //initialize the drop's parameters
        drop.GetComponent<Drop>().type = this.iceCreamType; // ice cream type
        drop.transform.position = this.transform.position; // position
        drop.GetComponent<Rigidbody>().mass = this.dropSize; // mass
        drop.GetComponent<Rigidbody>().velocity = RandomizeSpray() * spawnVelocity; // spray direction and velocity

        this.amountDropped += this.dropSize; // track how much has been dropped

        if (this.amountDropped > this.amount) // if enough droplets have spawned
        {
            // complete the drop process
            CancelInvoke("spawnDrop"); // stop dropping
            Discharge(); // mark the fountain as used
            Invoke("Recharge", this.rechargeTime); // recharge the fountain in 10 seconds
        }
    }
}
