using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycler : MonoBehaviour {

    [SerializeField]
    public bool Day;

    [SerializeField]
    Color NightColor;

    [SerializeField]
    bool SetOnInit;

    Color SunColor;
    Light Sun;

    Light[] lights = null;
    ParallaxBackground[] bgs = null;

    private void Start()
    {
        if(SetOnInit)
        {
            SetState();
        }
    }

    public void SetState()
    {
        GameCamera.Instance.Cam.backgroundColor = Day ? SunColor : NightColor;

        if (lights == null)
        {
            lights = (Light[])FindObjectsOfType(typeof(Light));
        }

        if (bgs == null)
        {
            bgs = (ParallaxBackground[])FindObjectsOfType(typeof(ParallaxBackground));
        }

        if (Sun == null)
        {
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    Sun = light;
                    SunColor = Sun.color;
                    break;
                }
            }
        }

        foreach (Light light in lights)
        {
            if (light == Sun)
            {
                Sun.color = Day? SunColor : NightColor;
                continue;
            }

            light.enabled = Day;
        }

        foreach(ParallaxBackground bg in bgs)
        {
            bg.gameObject.SetActive(Day);
        }
    }


    [SerializeField]
    bool DEBUGToggle = false;

    private void Update()
    {
        if(DEBUGToggle)
        {
            Day = !Day;
            SetState();

            DEBUGToggle = false;
        }
    }
}
