using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAlphaGroup : MonoBehaviour {

    public float Alpha;

    [SerializeField]
    Color OriginalColor = Color.clear;

    private void Awake()
    {
        if (OriginalColor == Color.clear)
        {
            OriginalColor = Color.white;
        }
    }

    public void SetAlpha(float fValue)
    {
        Alpha = fValue;

        RefreshChildrenAlpha(transform);
    }

    public void FadeIn(float Speed = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine(Speed));
    }

    public void FadeOut(float Speed = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(Speed));
    }

    private IEnumerator FadeOutRoutine(float speed)
    {
        Alpha = 1f;
        while (Alpha > 0f)
        {
            SetAlpha(Alpha - (speed * Time.deltaTime));
            yield return 0;
        }
    }

    private IEnumerator FadeInRoutine(float speed)
    {
        Alpha = 0f;
        while(Alpha < 1f)
        {
            SetAlpha(Alpha + (speed * Time.deltaTime));
            yield return 0;
        }
    }

    protected void RefreshChildrenAlpha(Transform currentChild)
    {
        SpriteRenderer renderer = currentChild.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Alpha);
        }

        for(int i=0;i<currentChild.transform.childCount;i++)
        {
            RefreshChildrenAlpha(currentChild.transform.GetChild(i));
        }
    }


    protected void SetChildrenColor(Transform currentChild, Color clr)
    {
        SpriteRenderer renderer = currentChild.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = clr;
        }

        for (int i = 0; i < currentChild.transform.childCount; i++)
        {
            SetChildrenColor(currentChild.transform.GetChild(i), clr);
        }
    }

    Coroutine BlinkDamageInstance;
    public void BlinkDamage()
    {
        if(BlinkDamageInstance != null)
        {
            StopCoroutine(BlinkDamageInstance);
        }

        BlinkDamageInstance = StartCoroutine(BlinkDamageRoutine());
    }

    private IEnumerator BlinkDamageRoutine()
    {
        SetChildrenColor(transform, Color.black);

        yield return new WaitForSeconds(0.05f);

        SetChildrenColor(transform, OriginalColor);

        BlinkDamageInstance = null;
    }
}
