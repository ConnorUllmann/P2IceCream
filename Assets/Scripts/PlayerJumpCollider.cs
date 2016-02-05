using UnityEngine;
using System.Collections;

public class PlayerJumpCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    
    void OnTriggerEnter(Collider c)
    {
        switch (c.gameObject.tag)
        {
            case "Tile":
                Player.S.grounded = true;
                break;
            default:
                break;
        }
    }
}
