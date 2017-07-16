using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class ChatHistory : MonoBehaviour 
{
    private static ChatHistory Instance;
 
    private bool navigating = false;

    private List<string> history = new List<string>();

    private int index;

    public void Awake()
    {
        Instance = this;
        // history.Add("One");
        // history.Add("Two");
        // history.Add("Three");
        // index = 2;
    }

    // TODO - 1. add new messages to chat history. 2. clicking up / down should manipulate the chat with history
    public void Update()
    {
        if (!navigating && history.Count > 0 && ChatboxUI.Instance.IsInputFocused())
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (index > 0)
                {
                    index--;
                    string current = history[index];
                    Debug.Log("up - Index is fine - " + current);
                }
                else 
                {
                    Debug.Log("up - Index crosses");
                }
                StartCoroutine(ThrottleNavigation());

            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (index < history.Count - 1)
                {
                    index++;
                    string current = history[index];
                    Debug.Log("down - Index is fine - " + current);
                }
                else 
                {
                    string current = "";
                    Debug.Log("down - Index crosses");
                }
                StartCoroutine(ThrottleNavigation());
            }
        }
    }

    private IEnumerator ThrottleNavigation()
    {
        // disable the input for a few milliseconds, so spamming up / down won't go so much in history
        navigating = true;
        yield return new WaitForSeconds(.3f);
        navigating = false;
    }
}