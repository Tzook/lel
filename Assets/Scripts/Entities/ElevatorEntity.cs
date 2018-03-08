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
        float t = 0f;
        while(t<1f)
        {
            t += Speed * Time.deltaTime;
            rigid.position = Vector3.Lerp(rigid.position, target.position, t);

            yield return 0;
        }

        MovementInstance = null;
    }
}
