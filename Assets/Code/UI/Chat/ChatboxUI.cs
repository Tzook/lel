using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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
                    Debug.Log("Chat message");
                    ChatHandler.Instance.SendChatMessage(m_txtField.text);
                    break;
                case (int)ChatConfig.TYPE_PARTY:
                    Debug.Log("Chat Party");
                    ChatHandler.Instance.SendPartyMessage(m_txtField.text);
                    break;
                case (int)ChatConfig.TYPE_WHISPER:
                    Debug.Log("Chat Whisper");
                    // TODO
                    // ChatHandler.Instance.SendWhisper(m_txtField.text);
                    break;

            }
        }
        m_txtField.text = "";
        Hide();
    }

    private void OpenChat()
    {
        ActivateChat();
        FocusChat();
    }

    public void FocusChat()
    {
        m_txtField.Select();
        Game.Instance.InChat = true;
    }

    private void ActivateChat()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            m_txtField.ActivateInputField();
        }
    }

    public void Hide()
    {
        Game.Instance.InChat = false;
        gameObject.SetActive(false);
    }
}
