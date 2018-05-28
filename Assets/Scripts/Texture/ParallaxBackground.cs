using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ParallaxBackground : MonoBehaviour {

    public MeshRenderer mRenderer;

    [SerializeField]
    Vector2 Speed;

    [SerializeField]
    bool Loop = false;

    [SerializeField]
    float LoopingSpeed;

    float DeltaX
    {
        get
        {
            return LastX - transform.position.x;
        }
    }
    float LastX;

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        LastX = transform.position.x;
    }

    private void Update()
    {
        if (Loop)
        {
            mRenderer.material.mainTextureOffset += (LoopingSpeed * Speed * Time.deltaTime);

            return;
        }

        mRenderer.material.mainTextureOffset = mRenderer.material.mainTextureOffset + ((-DeltaX) * Speed * Time.deltaTime);

    }

    private void LateUpdate()
    {
        LastX = transform.position.x;
    }
}
