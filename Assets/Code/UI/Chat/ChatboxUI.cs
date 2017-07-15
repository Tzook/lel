using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ChatboxUI : MonoBehaviour 
{
    [SerializeField]
    InputField m_txtField;

    [SerializeField]
    Dropdown m_chatType;

    public void onDeselectChat()
    {
        // we are no longer in chat, even though the chat is still open.
        Game.Instance.InChat = false;
    }

    public void onSelectChat()
    {
        Game.Instance.InChat = true;
    }

    public void ChatClicked()
    {
        if (Game.Instance.InChat)
        {
            SendChat();
        }
        else
        {
            OpenChat();
        }
    }

    private void SendChat()
    {
        if (!string.IsNullOrEmpty(m_txtField.text))
        {
            switch (m_chatType.value) 
            {
                case (int)ChatConfig.TYPE_AREA:
                    ChatHandler.Instance.SendChatMessage(m_txtField.text);
                    m_txtField.text = "";
                    break;
                case (int)ChatConfig.TYPE_PARTY:
                    ChatHandler.Instance.SendPartyMessage(m_txtField.text);
                    m_txtField.text = "";
                    break;
                case (int)ChatConfig.TYPE_WHISPER:
                    // whisper is built like this: "name text part" => ["name", "text part"]
                    string[] text = m_txtField.text.Split(new char[] { ' ' }, 2);
                    if (text.Length == 2 && text[0].Length > 0 && text[1].Length > 0)
                    {
                        ChatHandler.Instance.SendWhisper(text[1], text[0]);
                        m_txtField.text = text[0] + " ";

                    }
                    break;

            }
        }
        Hide();
    }

    private void OpenChat()
    {
        ActivateChat();
        FocusChat();
    }

    private void ActivateChat()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            m_txtField.ActivateInputField();
        }
    }

    public void FocusChat()
    {
        m_txtField.Select();
        Game.Instance.InChat = true;
        StartCoroutine(MoveCursorToEnd());
    }

    private IEnumerator MoveCursorToEnd()
    {
        yield return 0;
        m_txtField.MoveTextEnd(false);
    }

    public void Hide()
    {
        Game.Instance.InChat = false;
        gameObject.SetActive(false);
    }
}
