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

    public string lastWhisperer = "";

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
                    string[] parts = GetWhisperParts();
                    if (parts.Length == 2 && parts[0].Length > 0 && parts[1].Length > 0)
                    {
                        ChatHandler.Instance.SendWhisper(parts[1], parts[0]);
                        InitWhisperInput(parts[0]);
                    }
                    break;

            }
        }
        Hide();
    }

    private string[] GetWhisperParts()
    {
        // whisper is built like this: "name text part" => ["name", "text part"]        
        return m_txtField.text.Split(new char[] { ' ' }, 2);
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
        MoveCursorToEnd(true);
    }


    private void MoveCursorToEnd(bool coroutine)
    {
        StartCoroutine(MoveCursorToEndCoroutine(coroutine));
    }

    private IEnumerator MoveCursorToEndCoroutine(bool coroutine)
    {
        if (coroutine) 
        {
            yield return 0;
        }
        m_txtField.MoveTextEnd(false);
        yield return 1;
    }

    public void Hide()
    {
        ChatHistory.Instance.ResetHistoryIndex();
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
            // if reply, auto fill whisper
            if (ChatConfig.SHORTCUTS[index] == ChatConfig.SHORTCUT_REPLY)
            {
                m_chatType.value = ChatConfig.TYPE_WHISPER;
                InitWhisperInput(lastWhisperer);
                MoveCursorToEnd(true);
            }
            else
            {
                m_chatType.value = index;
                m_txtField.text = "";
            }
        }
    }

    private void InitWhisperInput(string name)
    {
        m_txtField.text = name.Length > 0 ? name + " " : "";
    }

    public void TypeChanged()
    {
        m_placeholderField.text = ChatConfig.PLACEHOLDERS[m_chatType.value];
    }

    public bool IsInputFocused()
    {
        return m_txtField.isFocused;
    }

    public void SetInputValue(string value)
    {
        if (m_chatType.value == ChatConfig.TYPE_WHISPER)
        {
            string[] parts = GetWhisperParts();
            InitWhisperInput(parts[0]);
        }
        else 
        {
            m_txtField.text = "";
        }
        m_txtField.text += value;
        MoveCursorToEnd(false);
    }
}
