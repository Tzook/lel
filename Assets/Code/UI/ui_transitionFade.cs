using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CanvasGroup))]
public class ui_transitionFade : MonoBehaviour {

	protected CanvasGroup m_CanvasGroup;
	public bool StartAsleep = false;
    public bool DisableOnFadeout = false;

	void Awake()
	{
		m_CanvasGroup = GetComponent<CanvasGroup> ();

		if(StartAsleep)
			CallFadeOut(999);
	}

	public void CallFadeIn(float speed)
	{
        this.gameObject.SetActive(true);

        if (speed == 0)
        {
            speed = 999;
        }

        StopAllCoroutines ();
		StartCoroutine (FadeIn (speed));
	}

	public void CallFadeOut(float speed)
	{
		StopAllCoroutines ();

        if(speed==0)
        {
            speed = 999;
        }

		StartCoroutine (FadeOut (speed));
	}

	protected IEnumerator FadeIn(float speed)
	{
		m_CanvasGroup.alpha = 0;
		while (m_CanvasGroup.alpha<1f)
		{
			m_CanvasGroup.alpha+=speed*Time.deltaTime;
			yield return 0;
		}
	}

	protected IEnumerator FadeOut(float speed)
	{
		m_CanvasGroup.alpha = 1;

		float t = 1f;
		while (t>0f)
		{
			t-=speed*Time.deltaTime;
			m_CanvasGroup.alpha = t;
			yield return 0;
		}

        if (DisableOnFadeout)
        {
            this.gameObject.SetActive(false);
        }
	}
}
