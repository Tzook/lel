using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatboxUI : MonoBehaviour {

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    InputField m_txtField;



	public void Open()
    {
        this.gameObject.SetActive(true);
        m_Animator.SetTrigger("Show");
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
        m_Animator.SetTrigger("Hide");
        Game.Instance.InChat = false;
    }
    
    public void CanShut()
    {
        this.gameObject.SetActive(false);
    }


}
