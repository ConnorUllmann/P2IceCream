using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpCollider : MonoBehaviour {

    public Vector3 offset;

	// Use this for initialization
	void Start ()
    {
        hit = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = offset * (transform.parent.gameObject.GetComponent<Player>() != null ? Player.S.n3 : 1) + transform.parent.position;
        transform.rotation = Quaternion.identity;
    }

    private List<GameObject> hit;
    
    void OnTriggerEnter(Collider c)
    {
        hit.Add(c.gameObject);
        switch (c.gameObject.tag)
        {
            case "Tile":
                Player p = GetComponentInParent<Player>();
                if(p)
                    p.grounded = true;
                else
                {
                    Enemy e = GetComponentInParent<Enemy>();
                    if (e)
                        e.grounded = true;
                }
                break;
            default:
                break;
        }
    }
    void OnTriggerExit(Collider c)
    {
        if (hit.Contains(c.gameObject))
            hit.Remove(c.gameObject);

        bool collidingWithGround = false;
        for(int i = 0; i < hit.Count; i++)
        {
            if(hit[i] == null)
            {
                hit.RemoveAt(i--);
                continue;
            }
            if (hit[i].tag == "Tile")
            {
                collidingWithGround = true;
                break;
            }
        }

        if(!collidingWithGround)
        {
            Player p = GetComponentInParent<Player>();
            if (p)
                p.grounded = false;
            else
            {
                Enemy e = GetComponentInParent<Enemy>();
                if (e)
                    e.grounded = false;
            }
        }
    }
}
