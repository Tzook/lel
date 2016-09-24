using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageBubbleUI : MonoBehaviour {

    [SerializeField]
    Text m_txtField;

    [SerializeField]
    CanvasGroup m_CanvasGroup;

    public void PopMessage(string content)
    {
        StopAllCoroutines();
        this.gameObject.SetActive(true);
        m_txtField.text = content;
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

        yield return new WaitForSeconds((m_txtField.text.Length / 4f));

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
