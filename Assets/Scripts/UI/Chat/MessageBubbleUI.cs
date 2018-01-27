using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageBubbleUI : MonoBehaviour 
{
    const float MINIMUM_CHAT_SECONDS = 3f;
    [SerializeField]
    Text m_txtField;

    [SerializeField]
    Image m_bubbleImage;

    [SerializeField]
    CanvasGroup m_CanvasGroup;

    public void PopMessage(string content, Color color)
    {
        StopAllCoroutines();
        this.gameObject.SetActive(true);
        m_txtField.text = content;
        m_bubbleImage.color = color;
        StartCoroutine(PopEffect());
    }

    private IEnumerator PopEffect()
    {
        float t = 0f;
        while(t<1f)
        {
            t += 3f * Time.deltaTime;
            m_CanvasGroup.alpha = t;
            yield return 0;
        }

        yield return new WaitForSeconds(System.Math.Max(m_txtField.text.Length / 10f, MINIMUM_CHAT_SECONDS));

        t = 0f;
        while (t < 1f)
        {
            t += 3f * Time.deltaTime;
            m_CanvasGroup.alpha = (1f-t);
            yield return 0;
        }

        this.gameObject.SetActive(false);
    }
}
