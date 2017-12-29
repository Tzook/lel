using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStayStrobe : MonoBehaviour
{
    Coroutine CurrentRoutine;
    [SerializeField]
    SpriteRenderer CurrentRenderer;

    [SerializeField]
    Color OnColor;

    [SerializeField]
    Color OffColor;

	public void Activate()
    {
        if(CurrentRoutine != null)
        {
            StopCoroutine(CurrentRoutine);
        }

        CurrentRoutine = StartCoroutine(StrobeRoutine());
    }

    public void Deactivate()
    {
        if (CurrentRoutine != null)
        {
            StopCoroutine(CurrentRoutine);
            CurrentRoutine = null;
        }

        CurrentRenderer.color = OnColor;
    }

    IEnumerator StrobeRoutine()
    {
        float t;
        while(true)
        {
            t = 0f;
            while(t<1f)
            {
                t += 1f * Time.deltaTime;

                CurrentRenderer.color = Color.Lerp(OffColor, OnColor, t);

                yield return 0;
            }

            t = 0f;
            while (t < 1f)
            {
                t += 1f * Time.deltaTime;

                CurrentRenderer.color = Color.Lerp(OnColor, OffColor, t);

                yield return 0;
            }
        }
    }

}
