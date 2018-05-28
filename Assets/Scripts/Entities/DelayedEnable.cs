using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEnable : MonoBehaviour {

    [SerializeField]
    float Time = 0.5f;

    [SerializeField]
    UnityEvent Elapsed;

    private void Start()
    {
        StartCoroutine(DelayedRoutine());
    }

    IEnumerator DelayedRoutine()
    {
        yield return new WaitForSeconds(Time);

        Elapsed.Invoke();
    }
}
