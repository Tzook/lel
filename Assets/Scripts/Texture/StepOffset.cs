using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class StepOffset : MonoBehaviour
{

    MeshRenderer mRenderer;

    [SerializeField]
    float Distance;

    [SerializeField]
    float Speed;

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
    }

    public void Step(Vector2 direction)
    {
        StepOffset stepOffsetComponent;
        for (int i = 0; i < transform.childCount; i++)
        {
            stepOffsetComponent = transform.GetChild(i).GetComponent<StepOffset>();

            if (stepOffsetComponent != null)
            {
                stepOffsetComponent.Step(direction);
            }
        }

        StopAllCoroutines();
        StartCoroutine(StepRoutine(direction));
    }

    IEnumerator StepRoutine(Vector2 direction)
    {
        Vector2 OffsetStart = mRenderer.material.mainTextureOffset;
        Vector2 OffsetDestination = OffsetStart + (direction * Distance);

        float t = 0f;
        while(t<1f)
        {
            t += Speed * Time.deltaTime;

            mRenderer.material.mainTextureOffset = Vector2.Lerp(mRenderer.material.mainTextureOffset, OffsetDestination, t);

            yield return 0;
        }
    }
}
