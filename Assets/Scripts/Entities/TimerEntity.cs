using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerEntity : MonoBehaviour {

    [SerializeField]
    UnityEvent OnReachZero;

    [SerializeField]
    float CurrentTime;

    bool Activated = true;

    public void SetTime(float Time)
    {
        CurrentTime = Time;
        Activated = false;
    }

    private void Update()
    {
        if(CurrentTime > 0)
        {
            CurrentTime -= 1f * Time.deltaTime;
        }
        else
        {
            if(!Activated)
            {
                Activated = true;
                OnReachZero.Invoke();
            }
        }
    }
}
