using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorEntity : MonoBehaviour {

    [SerializeField]
    Transform fromLocation;

    [SerializeField]
    Transform toLocation;

    [SerializeField]
    float Speed = 0.1f;

    [SerializeField]
    Rigidbody2D rigid;

    Coroutine MovementInstance;

    [SerializeField]
    AudioSource LoopSource;

    public void MoveOn()
    {
        if(MovementInstance != null)
        {
            StopCoroutine(MovementInstance);
        }

        MovementInstance = StartCoroutine(MovementRoutine(toLocation));
    }

    public void MoveBack()
    {
        if (MovementInstance != null)
        {
            StopCoroutine(MovementInstance);
        }

        MovementInstance = StartCoroutine(MovementRoutine(fromLocation));
    }

    IEnumerator MovementRoutine(Transform target)
    {
        if(LoopSource != null)
        {
            LoopSource.Play();
        }

        float t = 0f;
        while(t<0.99f)
        {
            t += Speed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target.position, t);

            yield return 0;
        }

        if (LoopSource != null)
        {
            LoopSource.Stop();
        }

        MovementInstance = null;
    }
}
