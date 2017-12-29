using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisabler : MonoBehaviour {

    [SerializeField]
    float InitTime;

    float CurrentTime;

    void OnEnable()
    {
        CurrentTime = InitTime;
    }

    void Update()
    {
        if(CurrentTime > 0)
        {
            CurrentTime -= 1f * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
