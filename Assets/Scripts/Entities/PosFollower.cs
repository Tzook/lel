using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosFollower : MonoBehaviour {

    [SerializeField]
    Transform Target;

    [SerializeField]
    float LerpSpeed = 5f;

    public void SetTransform(Transform tempTransform)
    {
        Target = tempTransform;
    }

    private void LateUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, Target.position, LerpSpeed * Time.deltaTime);
    }

}
