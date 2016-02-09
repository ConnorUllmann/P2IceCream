using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    private Camera camera;

	// Use this for initialization
	void Start () {
       camera = gameObject.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        var z = camera.transform.position.z;
        camera.transform.position += (Player.S.transform.position +
                                    (Utils.mouse - Player.S.transform.position) * 0.25f - camera.transform.position) * 0.25f;
        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, z);
	}
}
