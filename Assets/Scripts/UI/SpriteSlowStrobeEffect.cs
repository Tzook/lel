using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteSlowStrobeEffect : MonoBehaviour
{

    [SerializeField]
    Color clr1;

    [SerializeField]
    Color clr2;

    [SerializeField]
    float Speed;

    SpriteRenderer m_SpriteRenderer;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        StartCoroutine(DimRoutine());    
    }

    private IEnumerator DimRoutine()
    {
        float t = 0f;
        while(true)
        {
            t = 0f;
            while(t<1f)
            {
                t += Speed * Time.deltaTime;

                m_SpriteRenderer.color = Color.Lerp(clr1, clr2, t);
                
                yield return 0;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Speed * Time.deltaTime;

                m_SpriteRenderer.color = Color.Lerp(clr2, clr1, t);

                yield return 0;
            }
        }
    }
}
