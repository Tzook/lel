using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StopperEntity : MonoBehaviour {

    float Timer = 0f;

    bool Running = false;

    [SerializeField]
    Text timeText;

    [SerializeField]
    float TargetNumber;

    [SerializeField]
    UnityEvent OnReachTargetNumber;

    bool Invoked = false;

	public void StartStopper()
    {
        Running = true;
        Timer = 0f;
        Invoked = false;
    }

    private void Update()
    {
        if(Running)
        {
            Timer += 1f * Time.deltaTime;
            timeText.text = Mathf.FloorToInt(Timer).ToString();

            if(!Invoked && Timer >= TargetNumber)
            {
                Invoked = true;

                OnReachTargetNumber.Invoke();
            }
        }
    }

    public void StopStopper()
    {
        Running = false;
        timeText.text = Mathf.FloorToInt(Timer).ToString();
    }
}
