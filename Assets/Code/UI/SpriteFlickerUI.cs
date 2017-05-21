using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFlickerUI : MonoBehaviour
{

    [SerializeField]
    Image m_Image;

    [SerializeField]
    Sprite OriginalSprite;

    [SerializeField]
    Sprite FlickerSprite;

    [SerializeField]
    float Delay;

    [SerializeField]
    float StartDelay;

    void Start()
    {
        StartCoroutine(FlickerRoutine());
    }

    void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        yield return new WaitForSeconds(StartDelay);

        while (true)
        {
            m_Image.sprite = OriginalSprite;

            yield return new WaitForSeconds(Delay);

            m_Image.sprite = FlickerSprite;

            yield return new WaitForSeconds(Delay);
        }
    }
}
