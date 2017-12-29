using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlink : MonoBehaviour {

    [SerializeField]
    Sprite OriginalEyes;

    [SerializeField]
    SpriteRenderer m_Renderer;

    [SerializeField]
    Sprite BlinkEyes;

    [SerializeField]
    float Delay;

    [SerializeField]
    float BlinkTime;

    void Awake()
    {
        OriginalEyes = m_Renderer.sprite;
    }


    void OnEnable()
    {
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return 0;

            m_Renderer.sprite = OriginalEyes;

            yield return new WaitForSeconds(Delay);

            m_Renderer.sprite = BlinkEyes;

            yield return new WaitForSeconds(BlinkTime);

            if(Random.Range(0,4) == 0)
            {
                m_Renderer.sprite = OriginalEyes;

                yield return new WaitForSeconds(BlinkTime);

                m_Renderer.sprite = BlinkEyes;

                yield return new WaitForSeconds(BlinkTime);

            }
        }
    }

}
