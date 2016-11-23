using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ui_pageMenu : MonoBehaviour {

	public List<GameObject> m_listPages = new List<GameObject>();

	public int 	 m_iIndex = 0;
	public float m_fSpeed = 0.5f;
    public float m_fDelay = 0f;

    public bool  m_bUpdateOnStart = false;
	public bool  m_bVertical      = false;
    public bool m_bInstant = false;

    public string m_sPageSwitchSound;

	void Start()
	{
		if(m_bUpdateOnStart)
		{
			UpdatePage();
		}
	}

	//Will switch the page to a provided index.
	public void SwitchTo(int gIndex)
	{
        if (gIndex < m_listPages.Count && gIndex >= 0)
		{
			m_iIndex = gIndex;
			UpdatePage();
		}
	}

    //Will switch the page to a provided index.
    public void InstantSwitch(int gIndex)
    {
        if (gIndex < m_listPages.Count && gIndex >= 0)
        {
            m_iIndex = gIndex;
            m_bInstant = true;
            UpdatePage();
        }
    }



    //Will update the menu to the m_iIndex page.
    protected void UpdatePage()
	{
        AudioControl.Instance.Play(m_sPageSwitchSound);

		StopAllCoroutines();

		m_listPages[m_iIndex].transform.SetAsLastSibling();

		for(int i=0;i<m_listPages.Count;i++)
		{
			StartCoroutine(RelocatePage(i));
		}

	}

	//Will lerp the list item to be in a relative position to the focused index.
	protected IEnumerator RelocatePage(int selfIndex)
	{
        if (!m_bInstant)
        {
            yield return new WaitForSeconds(m_fDelay);

        }


        m_listPages[selfIndex].SetActive(true);

		RectTransform currentRect = m_listPages[selfIndex].GetComponent<RectTransform>();
		float A = 0;
		float B = 0;
		float sizePenalty = 0;
		float change = 0;
		Vector2 targetPos = new Vector2(0,0);

		if(m_bVertical)
		{
			A = currentRect.TransformPoint(currentRect.rect.x+currentRect.rect.width,currentRect.rect.y+currentRect.rect.height,0).y;
			B = currentRect.TransformPoint(currentRect.rect.x,currentRect.rect.y,0).y;
			sizePenalty = A - B;
		}
		else
		{
			A = currentRect.TransformPoint(currentRect.rect.x+currentRect.rect.width,currentRect.rect.y+currentRect.rect.height,0).x;
			B = currentRect.TransformPoint(currentRect.rect.x,currentRect.rect.y,0).x;
			sizePenalty = A - B;
		}

		change = sizePenalty*(selfIndex - m_iIndex);

        if (m_bInstant)
        {
            m_listPages[selfIndex].transform.position = targetPos;
        }
        else
        {

            float t = 0;
            while (t < 1)
            {

                t += 1 * m_fSpeed * Time.deltaTime;

                if (m_bVertical)
                {
                    targetPos = transform.position + transform.TransformDirection(0, change, 0);
                }
                else
                {
                    targetPos = transform.position + transform.TransformDirection(change, 0, 0);
                }

                m_listPages[selfIndex].transform.position = Vector3.Lerp(m_listPages[selfIndex].transform.position, targetPos, t);

                yield return 0;
            }
        }

		if(selfIndex != m_iIndex)
		{
			m_listPages[selfIndex].SetActive(false);
		}

        if(m_bInstant && m_listPages.Count == selfIndex+1)
        {
            m_bInstant = false;
        }
	}

	//DEBUG

	public bool test;

	void Update()
	{
		if(test)
		{
			test = false;
			UpdatePage();
		}
	}

}
