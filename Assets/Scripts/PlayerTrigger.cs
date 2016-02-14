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
        if (c.gameObject.tag == "Player" && Player.S.hurtTimer <= 0)
        {
            if (drop.type == Player.S.topScoopType)
                Player.S.topScoopMass += drop.StealMass();
            else if (drop.type == Player.S.middleScoopType)
                Player.S.middleScoopMass += drop.StealMass();
            else if (drop.type == Utils.IceCream.Brown)
                Player.S.AddMass(drop.StealMass());
        }
    }
}
