using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class ChatHistory : MonoBehaviour 
{
    public static ChatHistory Instance;
 
    private bool navigating = false;

    // a list of the chat history sent by the user. looks like this: ["", "third", "second", "first"]
    private List<string> history = new List<string>();

    private int index = 0;

    public void Awake()
    {
        Instance = this;
        // add an empty history value - so when you click down it will put empty if it's the first
        history.Add("");
    }

    public void Update()
    {
        if (!navigating && history.Count > 0 && ChatboxUI.Instance.IsInputFocused())
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (index < history.Count - 1)
                {
                    string current = history[++index];
                    ChatboxUI.Instance.SetInputValue(current);
                }
                StartCoroutine(ThrottleNavigation());

            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                string current = index > 0 ? history[--index] : "";
                ChatboxUI.Instance.SetInputValue(current);                
                StartCoroutine(ThrottleNavigation());
            }
        }
    }

    private IEnumerator ThrottleNavigation()
    {
        // disable the input for a few milliseconds, so spamming up / down won't go so much in history
        navigating = true;
        yield return new WaitForSeconds(.1f);
        navigating = false;
    }

    public void PushHistoryRow(string text)
    {
        // put the row after the empty line
        history.Insert(1, text);
    }

    public void ResetHistoryIndex()
    {
        index = 0;
    }
}