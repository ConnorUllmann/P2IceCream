using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public static int Sign(float x, bool canReturnZero = true)
    {
        if (canReturnZero && x == 0)
            return 0;
        return x >= 0 ? 1 : -1;
    }

    public static Vector3 mouse { get { return Camera.main.ScreenToWorldPoint(Input.mousePosition); } }
    public static float GetAngleToMouse(Vector3 pos) { var m = mouse; return Mathf.Atan2(m.y - pos.y, m.x - pos.x); }

    public static void showTrajectory(Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = pVelocity.magnitude;
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;

        Vector3 pt = pStartPosition;
        float offsetX = 0;
        for (int i = 0; i < 30; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 ptNew = new Vector3(pStartPosition.x + offsetX + dx, pStartPosition.y + dy, 2);
            //Debug.DrawLine(pt, ptNew);
            pt = ptNew;
            fTime += 0.1f;
        }
    }

    public static Vector3 trajectoryYGivenX(float x, Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = (x - pStartPosition.x) / velocity / Mathf.Cos(angle * Mathf.Deg2Rad);
        float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
        Vector3 ptNew = new Vector3(x, pStartPosition.y + dy, 2);
        float d = 0.2f;
        //Debug.DrawLine(ptNew + new Vector3(-d, -d, 0), ptNew + new Vector3(d, d, 0));
        //Debug.DrawLine(ptNew + new Vector3(d, -d, 0), ptNew + new Vector3(-d, d, 0));
        return ptNew;
    }
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
