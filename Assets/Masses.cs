using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Masses : MonoBehaviour
{
    public static double g;
    public static List<Masses> masses = new List<Masses>();
    public bool haveSigMass = true;
    public double mass = 1000000000000000000;
    public double radius = 10000;
    public double xV, yV, zV;
    public double xP, yP, zP;
    public CVector3 velocity;
    public CVector3 position;
    public static double scale = 0.0000000001;
    public static double timeScale = 100;
    public int trail = 50;
    public Queue<Trail> trailObjs = new Queue<Trail>();
    public long cycle;
    public float time = 0.0f;
    private CVector3 lastPos;

    void Awake()
    {
        velocity = new CVector3(xV, yV, zV);
        position = new CVector3(xP, yP, zP);
        g = 6.674 * Mathf.Pow(10, -11);
    }

	// Use this for initialization
	void Start ()
    {
        Debug.Log(Time.fixedDeltaTime + " " + (1.0/Time.fixedDeltaTime));
        if (true)
        {
            masses.Add(this);
        }
	}

    // Update is called once per frame
    public void CFixedUpdate()
    {
        transform.position = CVector3.toV3(CVector3.multiply(position, scale));
        //Debug.Log(CVector3.distance(position, masses[i].position));
        for (int i = 0; i < masses.Count; i++)
        {
            if (masses[i] != this)
            {
                velocity = CVector3.add(
                    velocity
                    , CVector3.multiply(
                        CVector3.subtract(masses[i].position, position)
                        , ((g * masses[i].mass) / Math.Pow(CVector3.distance(position, masses[i].position), 3)) * timeScale));
            }
        }
        
    }

    public void C2FixedUpdate()
    {
        position = CVector3.add(position, CVector3.multiply(velocity, timeScale));
        //cycle++;
    }

    public void C3FixedUpdate()
    {
        float fscale = 0;
        transform.position = CVector3.toV3(position, scale, TrailPool.cameraScale, TrailPool.cameraX, TrailPool.cameraY, radius, out fscale);
        Vector3 oScale = new Vector3(fscale, 0, fscale);
        transform.localScale = oScale;

        if (CVector3.distance(lastPos, position) * scale * TrailPool.cameraScale > .3)
        {
            lastPos = position;
            //Debug.Log(CVector3.mag(velocity));
            //Debug.Log(Time.timeScale);
        }
        else
        {
            return;
        }
        if (trailObjs.Count < trail)
        {
            Trail temp = TrailPool.Get();
            temp.enabled = true;
            temp.position = position;
            temp.CFixedUpdate();
            trailObjs.Enqueue(temp);
        }
        else if (trailObjs.Count > trail)
        {
            Trail temp = trailObjs.Dequeue();
            temp.Disable();
            TrailPool.AddToStack(temp);
        }
        else
        {
            Trail temp = trailObjs.Dequeue();
            temp.enabled = true;
            temp.position = position;
            temp.CFixedUpdate();
            trailObjs.Enqueue(temp);
        }
    }

    public void OnDestroy()
    {
        masses.Remove(this);
    }
}

public struct CVector3
{
    public double x;
    public double y;
    public double z;

    public CVector3(double x,double y,double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static CVector3 multiply(CVector3 vector,double multiplier)
    {
        vector.x = vector.x * multiplier;
        vector.y = vector.y * multiplier;
        vector.z = vector.z * multiplier;
        return vector;
    }

    public static CVector3 add(CVector3 vector, CVector3 vector2)
    {
        vector.x = vector.x + vector2.x;
        vector.y = vector.y + vector2.y;
        vector.z = vector.z + vector2.z;
        return vector;
    }

    public static CVector3 subtract(CVector3 vector, CVector3 vector2)
    {
        vector.x = vector.x - vector2.x;
        vector.y = vector.y - vector2.y;
        vector.z = vector.z - vector2.z;
        return vector;
    }

    public static double distance(CVector3 vector, CVector3 vector2)
    {
        double dis = Math.Sqrt(Math.Pow(vector.x - vector2.x, 2) + Math.Pow(vector.y - vector2.y, 2) + Math.Pow(vector.z - vector2.z, 2));
        return dis;
    }

    public static Vector3 toV3(CVector3 vector)
    {
        //Debug.Log(vector.x);
        return new Vector3((float)vector.x, (float)vector.y, (float)vector.z);
    }

    public static Vector3 toV3(CVector3 vector,double worldScale,double cScale,double cameraX,double cameraY,double radius,out float scale)
    {
        vector.x = (vector.x - cameraX) * worldScale * cScale;
        vector.y = vector.y * worldScale * cScale;
        vector.z = (vector.z - cameraY) * worldScale * cScale;

        radius = radius * cScale * worldScale * 2.000d;
        if(radius < 0.1)
        {
            radius = 0.1;
        }
        scale = (float)radius;
        return toV3(vector);
    }

    public static double mag(CVector3 vector)
    {
        return Math.Sqrt(Math.Pow(vector.x, 2) + Math.Pow(vector.y, 2) + Math.Pow(vector.z, 2));
    }
}
