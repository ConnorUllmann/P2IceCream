using UnityEngine;
using System.Collections;

public class PlayerTrigger : MonoBehaviour {

    public Drop drop;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            if(drop.isTopScoop)
                Player.S.topScoopMass += drop.StealMass();
            else
                Player.S.middleScoopMass += drop.StealMass();
        }
    }
}
