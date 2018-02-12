using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trail : MonoBehaviour {

    public static List<Trail> trails = new List<Trail>();
    public CVector3 position;
    public bool active = true;

	// Use this for initialization
	void Start () {
        trails.Add(this);
	}

    public void Disable()
    {
        active = false;
        transform.position = Vector3.up * 100;
    }

    public void CFixedUpdate()
    {
        if (active)
        {
            float opt = 0;
            transform.position = CVector3.toV3(position, Masses.scale, TrailPool.cameraScale, TrailPool.cameraX, TrailPool.cameraY, opt, out opt);
        }
    }

    public void OnDestroy()
    {
        trails.Remove(this);
    }
}
