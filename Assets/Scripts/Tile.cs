using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    public Sprite[] shapes;
    private int shape = 0;

	// Use this for initialization
	void Start ()
    {
        shape = (CollidesWithTileAt(transform.position + new Vector3(1, 0)) ? 1 : 0) +
                (CollidesWithTileAt(transform.position + new Vector3(0, 1)) ? 2 : 0) +
                (CollidesWithTileAt(transform.position + new Vector3(-1, 0)) ? 4 : 0) +
                (CollidesWithTileAt(transform.position + new Vector3(0, -1)) ? 8 : 0);
        GetComponent<SpriteRenderer>().sprite = shapes[shape];
    }

    bool CollidesWithTileAt(Vector3 p)
    {
        var c = Physics.OverlapBox(p, new Vector3(0.1f, 0.1f, 0.1f));
        for(int i = 0; i < c.Length; i++)
            if(c[i].gameObject.tag == "Tile")
                return true;
        return false;
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
