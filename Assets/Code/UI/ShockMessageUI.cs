using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ShockMessageUI : MonoBehaviour {

    [SerializeField]
    Text m_txtContent;

    [SerializeField]
    CanvasGroup m_CanvasGroup;

    public static ShockMessageUI Instance;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

	public void CallMessage(string content)
    {
        this.gameObject.SetActive(true);

        m_txtContent.text = content;

        if(ShockInstance!=null)
        {
            StopCoroutine(ShockInstance);
        }

        ShockInstance = StartCoroutine(ShockRoutine());
    }

    Coroutine ShockInstance;
    private IEnumerator ShockRoutine()
    {
        m_CanvasGroup.alpha = 0f;
        while(m_CanvasGroup.alpha<1f)
        {
            m_CanvasGroup.alpha += 6f * Time.deltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds((0.1f*m_txtContent.text.Length));

        m_CanvasGroup.alpha = 1f;
        while (m_CanvasGroup.alpha > 0f)
        {
            m_CanvasGroup.alpha -= 1f * Time.deltaTime;
            yield return 0;
        }

        ShockInstance = null;

        this.gameObject.SetActive(false);
    }

}
