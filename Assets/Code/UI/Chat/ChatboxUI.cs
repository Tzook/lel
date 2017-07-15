using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatboxUI : MonoBehaviour {

    [SerializeField]
    InputField m_txtField;

    public void onDeselectChat()
    {
        // we are no longer in chat, even though the chat is still open.
        Game.Instance.InChat = false;
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
            Game.Instance.SendChatMessage(m_txtField.text);
        }
        m_txtField.text = "";
        Hide();
    }

    private void OpenChat()
    {
        ActivateChat();
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
