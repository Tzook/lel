using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPop : MonoBehaviour {

	float CurrentTime;

    public void Show(float Time)
    {
        this.gameObject.SetActive(true);

        CurrentTime = Time;
    }

    private void Update()
    {
        if(CurrentTime > 0)
        {
            CurrentTime -= 1 * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
