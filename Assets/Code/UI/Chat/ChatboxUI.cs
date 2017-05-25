using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatboxUI : MonoBehaviour {

    [SerializeField]
    InputField m_txtField;



	public void Open()
    {
        this.gameObject.SetActive(true);
        m_txtField.Select();
        m_txtField.ActivateInputField();
        Game.Instance.InChat = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(InputMap.Map["Chat"]))
        {
            if (!string.IsNullOrEmpty(m_txtField.text))
            {
                Game.Instance.SendChatMessage(m_txtField.text);
                m_txtField.text = "";
            }

            Hide();
        }

        if (Input.GetKeyDown(InputMap.Map["Chat"]))
        {
            m_txtField.text = "";
            Hide();
        }
    }

    public void Hide()
    {
        Game.Instance.InChat = false;
        this.gameObject.SetActive(false);
    }
   


}
