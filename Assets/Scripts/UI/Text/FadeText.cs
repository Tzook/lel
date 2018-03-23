using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeText: MonoBehaviour
{
    [SerializeField]
    public Text text;
    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    // pass 0 for instant
    public void FadeIn(float time)
    {
        if (fadeInCoroutine == null)
        {
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = null;
            }
            fadeInCoroutine = StartCoroutine(FadeInCoroutine(time));
        }
    }

    public void FadeInInstant()
    {
        FadeIn(0);
    }

    // pass 0 for instant
    public void FadeOut(float time)
    {
        if (fadeOutCoroutine == null)
        {
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
                fadeInCoroutine = null;
            }
            fadeOutCoroutine = StartCoroutine(FadeOutCoroutine(time));
        }
    }
    
    public void FadeOutInstant()
    {
        FadeOut(0);
    }

    protected IEnumerator FadeInCoroutine(float time)
    {
        text.gameObject.SetActive(true);
        while (text.color.a < 1.0f)
        {
            float alpha = time > 0 ? text.color.a + (Time.deltaTime / time) : 1f;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return 0;
        }
    }
 
    protected IEnumerator FadeOutCoroutine(float time)
    {
        while (text.color.a > 0.0f)
        {
            float alpha = time > 0 ? text.color.a - (Time.deltaTime / time) : 0f;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return 0;
        }
        text.gameObject.SetActive(false);
    }
}