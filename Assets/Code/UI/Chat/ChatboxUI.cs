using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ChatboxUI : MonoBehaviour 
{
    public static ChatboxUI Instance;

    [SerializeField]
    InputField m_txtField;

    [SerializeField]
    Text m_placeholderField;

    [SerializeField]
    Toggle m_remainOpenToggle;

    [SerializeField]
    Dropdown m_chatType;

    public void Awake()
    {
        Instance = this;
        TypeChanged();
    }

    public void onDeselectChat()
    {
        // we are no longer in chat, even though the chat is still open.
        Game.Instance.InChat = false;
        Game.Instance.Focus();
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
        if (!String.IsNullOrEmpty(m_txtField.text))
        {
            switch (m_chatType.value) 
            {
                case ChatConfig.TYPE_AREA:
                    ChatHandler.Instance.SendChatMessage(m_txtField.text);
                    m_txtField.text = "";
                    break;
                case ChatConfig.TYPE_PARTY:
                    ChatHandler.Instance.SendPartyMessage(m_txtField.text);
                    m_txtField.text = "";
                    break;
                case ChatConfig.TYPE_WHISPER:
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
        onDeselectChat();
        if (!m_remainOpenToggle.isOn)
        {
            gameObject.SetActive(false);
        }
    }

    public void CheckShortcuts()
    {
        int index = Array.IndexOf(ChatConfig.SHORTCUTS, m_txtField.text);
        if (index > -1)
        {
            m_chatType.value = index;
            m_txtField.text = "";
        }
    }

    public void TypeChanged()
    {
        m_placeholderField.text = ChatConfig.PLACEHOLDERS[m_chatType.value];
    }
}
