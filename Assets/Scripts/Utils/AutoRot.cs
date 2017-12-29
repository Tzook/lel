using UnityEngine;
using System.Collections;

public class AutoRot : MonoBehaviour {

    public float Speed = 50f;

    void Update()
    {
        transform.Rotate(transform.forward * Speed * Time.deltaTime);
    }
}
