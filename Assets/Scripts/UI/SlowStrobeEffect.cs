using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlowStrobeEffect : MonoBehaviour
{

    [SerializeField]
    Color clr1;

    [SerializeField]
    Color clr2;

    [SerializeField]
    float Speed;

    [SerializeField]
    float Delay;

    Image m_SpriteRenderer;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<Image>();
    }

    void OnEnable()
    {
        StartCoroutine(DimRoutine());    
    }

    private IEnumerator DimRoutine()
    {
        yield return new WaitForSeconds(Delay);

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
