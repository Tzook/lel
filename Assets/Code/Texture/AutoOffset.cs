using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AutoOffset : MonoBehaviour {

    MeshRenderer mRenderer;

    [SerializeField]
    Vector2 Speed;


    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        mRenderer.material.mainTextureOffset = mRenderer.material.mainTextureOffset + (Speed * Time.deltaTime);
    }

}
