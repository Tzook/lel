using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcceptDeclineWindowUI : MonoBehaviour {

    [SerializeField]
    CanvasGroup m_CG;

    [SerializeField]
    Text txtContent;

    [SerializeField]
    float FadeSpeed = 2f;


    List<AcceptDeclineMessage> PopsQue = new List<AcceptDeclineMessage>();

    Decision LatestDecision;

    Coroutine QueRoutine;


    public void AddMessage(string content,string key, Action<string> acceptCallback)
    {
        if (ContainsMessage(content))
        {
            return;
        }

        PopsQue.Add(new AcceptDeclineMessage(content, key, acceptCallback));

        if(QueRoutine == null)
        {
            QueRoutine = StartCoroutine(ShowQueMessages());
        }
    }

    public bool ContainsMessage(string content)
    {
        foreach(AcceptDeclineMessage msg in PopsQue)
        {
            if(msg.Content == content)
            {
                return true;
            }
        }

        return false;
    }

    public void SetLastDecisionAccept()
    {
        LatestDecision = Decision.Accept;
    }

    public void SetLastDecisionDecline()
    {
        LatestDecision = Decision.Decline;
    }

    IEnumerator ShowQueMessages()
    {
        while(PopsQue.Count > 0)
        {
            yield return StartCoroutine(ShowMessage(PopsQue[0]));
            PopsQue.RemoveAt(0);
        }

        QueRoutine = null;
    }

    IEnumerator ShowMessage(AcceptDeclineMessage acceptDeclineMessage)
    {
        m_CG.alpha = 0f;

        while (m_CG.alpha < 1)
        {
            m_CG.alpha += FadeSpeed * Time.deltaTime;
            yield return 0;
        }

        LatestDecision = Decision.Pending;

        txtContent.text = acceptDeclineMessage.Content;

        while(LatestDecision == Decision.Pending)
        {
            yield return 0;
        }

        if(LatestDecision == Decision.Accept)
        {
            acceptDeclineMessage.Callback(acceptDeclineMessage.Key);
        }

        m_CG.alpha = 1f;



        while (m_CG.alpha > 0)
        {
            m_CG.alpha -= FadeSpeed * Time.deltaTime;
            yield return 0;
        }
    }

    enum Decision
    {
        Pending,Accept,Decline
    }
}

public class AcceptDeclineMessage
{
    public string Content;
    public string Key;

    public Action<string> Callback;

    public AcceptDeclineMessage(string content,string key, Action<string> callback)
    {
        this.Content = content;
        this.Key = key;

        this.Callback = callback;
    }
}
