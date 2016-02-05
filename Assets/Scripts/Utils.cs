using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public static int Sign(float x, bool canReturnZero=true)
    {
        if (canReturnZero && x == 0)
            return 0;
        return x >= 0 ? 1 : -1;
    }

    public static Vector3 mouse { get { return Camera.main.ScreenToWorldPoint(Input.mousePosition); } }
    public static float GetAngleToMouse(Vector3 pos) { var m = mouse; return Mathf.Atan2(m.y - pos.y, m.x - pos.x);  }

    /*public static Vector3 GetLaunchVector(Vector3 _start, Vector3 to, float _gravity, float _velocity)
    {
		var g = -_gravity;
        var x = to.x - _start.x;
        var y = to.y - _start.y;
        var v = Mathf.Abs(_velocity);
        var v2 = v* v;
        var sqrt = v2* v2 - g* (g* x * x + 2 * y* v2);
        float angle = 0;
		if (x == 0 && sqrt >= 0)
			angle = -Mathf.PI / 2;
        else if (sqrt >= 0)
			angle = Mathf.Atan((v2 + Mathf.Sqrt(sqrt)) / (g* x));
        else
            return Vector3.zero; 
			
		var p = new Vector3(Mathf.Cos(angle) * Mathf.Sign(x), Mathf.Sin(angle) * Mathf.Sign(x));
		//p.normalize(v);
        p.Normalize();
        p *= v;
        return p;
    }*/
}
