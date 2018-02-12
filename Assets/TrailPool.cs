using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrailPool : MonoBehaviour
{
    public static TrailPool singleton;
    private static Stack<Trail> free = new Stack<Trail>();
    public static double time = 0;
    public GameObject prefab;
    public GameObject massPrefab;
    public static double cameraX;
    public double startingCameraX;
    public static double cameraY;
    public double startingCameraY;
    public static double cameraScale = 0.3;
    public double startingCameraScale = 0.3;
    public static double sensetivity = 0.1;
    public int optimization = 4;
    public Text simTimeDisplay;
    public Text realTimeDisplay;
    public Text cyclesDisplay;
    public InputField massNum;
    public InputField massExp;
    public InputField radiusNum;
    public InputField radiusExp;
    public InputField speedNum;
    public Slider angle;
    public Slider simSpeed;
    public static long cycles;

    void Awake()
    {
        cameraScale = startingCameraScale;
        cameraX = startingCameraX;
        cameraY = startingCameraY;
        time = 0;
        cycles = 0;
    }

    public void Start()
    {
        if (simSpeed)
        {
            SimulationRateChanged(simSpeed);
        }
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Multiple TrailPool");
        }
    }

    public static void AddToStack(Trail point)
    {
        free.Push(point);
        point.active = false;
    }

    public void FixedUpdate()
    {
        if (Masses.timeScale != 0)
        {
            time += Time.fixedDeltaTime;
            cycles += (864 / optimization);
            //Debug.Log(time);
            for (int u = 0; u < ((864*2) / optimization); u++)
            {
                for (int i = 0; i < Masses.masses.Count; i++)
                {
                    Masses.masses[i].CFixedUpdate();
                }
                for (int i = 0; i < Masses.masses.Count; i++)
                {
                    Masses.masses[i].C2FixedUpdate();
                }
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < Masses.masses.Count; i++)
        {
            Masses.masses[i].C3FixedUpdate();
        }
        bool changed = false;
        if(Input.GetMouseButton(1))
        {
            cameraX += (Input.GetAxis("Mouse X") * sensetivity)/ (Masses.scale * cameraScale);
            cameraY += (Input.GetAxis("Mouse Y") * sensetivity)/ (Masses.scale * cameraScale);
            changed = true;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scroll = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -0.1f, 0.1f);
            cameraScale *= (1 + scroll);
            //Debug.Log("Camera Scale: " + cameraScale.ToString());
            changed = true;
        }
        if (changed)
        {
            for (int i = 0; i < Trail.trails.Count; i++)
            {
                Trail.trails[i].CFixedUpdate();
            }
        }
        UpdateText();
    }

    private void UpdateText()
    {
        realTimeDisplay.text = "Real Time: " + time.ToString("F1") + "s";
        simTimeDisplay.text = "Sim Rate: " + (Masses.timeScale / (float)optimization).ToString("F2") + " Days/s";
        cyclesDisplay.text = "Physics Steps: " + ((double)cycles).ToString("E2");
    }

    public static Trail Get()
    {
        Trail r;
        if(free.Count == 0) {
            r = Instantiate(singleton.prefab).GetComponent<Trail>();
        }
        else
        {
            r = free.Pop();
        }
        return r;
    }

    public void SimulationRateChanged(Slider slider)
    {
        Masses.timeScale = slider.value * slider.value * (float)optimization;
    }

    public void ChangeScene(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    public void ChangeSceneReletively(int change)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + change);
    }

    public void SpawnMass()
    {
        double mass = float.Parse(massNum.text) * Mathf.Pow(10, int.Parse(massExp.text));
        double radius = float.Parse(radiusNum.text) * Mathf.Pow(10, int.Parse(radiusExp.text));
        double speed = float.Parse(radiusNum.text) * 1000;//text is in km/s
        double angle = this.angle.value;
        //Masses mass = Instantiate(massPrefab,)
    }

    public void OnDestroy()
    {
        free.Clear();
    }
}
