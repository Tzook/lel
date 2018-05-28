using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopableScenery : MonoBehaviour {

    [SerializeField]
    public Transform EndPoint;

    void OnValidate()
    {
        EndPoint = transform.Find("End");
    }
}
