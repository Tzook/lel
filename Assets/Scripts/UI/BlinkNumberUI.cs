using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class BlinkNumberUI : MonoBehaviour {

    [SerializeField]
    Color BlinkColor;

    Color InitColor;

    [SerializeField]
    Text textContent;

    [SerializeField]
    Image imageContent;

    public Coroutine ChangeValueInstance { get; private set; }

    void OnEnable()
    {
        InitColor = imageContent.color;
    }

    public void SetValue(string content)
    {
        if(ChangeValueInstance != null)
        {
            StopCoroutine(ChangeValueInstance);
        }

        ChangeValueInstance = StartCoroutine(ChangeValueRoutine(content));
    }

    private IEnumerator ChangeValueRoutine(string content)
    {
        imageContent.color = InitColor;

        float t = 0f;
        while(t<1f)
        {
            t += 3f * Time.deltaTime;

            imageContent.color = Color.Lerp(InitColor, BlinkColor, t);

            yield return 0;
        }

        textContent.text = content;

        t = 0f;
        while (t < 1f)
        {
            t += 1f * Time.deltaTime;

            imageContent.color = Color.Lerp(BlinkColor, InitColor, t);

            yield return 0;
        }


        ChangeValueInstance = null;
    }
}
