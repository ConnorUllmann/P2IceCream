using UnityEngine;
using System.Collections;

public class Fountain : MonoBehaviour {

    public Utils.IceCream iceCreamType = Utils.IceCream.White; // the type of ice cream that drops
    public float rechargeTime = 10f; // the amount of time it takes to recharge the fountain
    public float amount = 0.5f; // the total mass of ice cream that drops on activation
    public float dropSize = 0.01f; // the mass of each ice cream droplet
    public GameObject dropPrefab; // the drop prefab

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

    void dropIceCream() {
        if (!recharged) {
            return; // don't spawn ice cream if the fountain isn't charged
        }
        float amountSpawned = 0f;
        while (amountSpawned < this.amount) { // until the correct amount of ice cream has been dropped
            GameObject drop = Instantiate(dropPrefab); // instantiate a drop
            drop.GetComponent<Drop>().type = this.iceCreamType; // change the drop to the correct type
            drop.GetComponent<Rigidbody>().mass = this.dropSize; // change the drop to the correct size
            drop.GetComponent<Rigidbody>().velocity = this.sprayDirection; // spray the drop in the correct direction
            drop.transform.position = this.transform.position; // change the drop to the correct position
            amountSpawned += this.dropSize; // track how much has been dropped
        }
        Discharge(); // mark the fountain as used
        Invoke("Recharge", this.rechargeTime); // recharge the fountain in 10 seconds
    }
}
