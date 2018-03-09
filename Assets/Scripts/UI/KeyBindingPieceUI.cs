using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KeyBindingPieceUI : MonoBehaviour {

    [SerializeField]
    Text m_txtTitle;

    [SerializeField]
    Text m_txtKey;

    [SerializeField]
    Image m_Image;

    [SerializeField]
    public Button m_btn;

    public KeyCode CurrentKey;

    public bool isWaitingForKey = false;
    Event keyEvent;
    Color initColor;

    public void SetInfo(string title, KeyCode key)
    {
        CurrentKey = key;
        m_txtTitle.text = title;
        m_txtKey.text = CurrentKey.ToString();
    }

    public void OnClick()
    {
        SetBinding();
    }

    protected void SetBinding()
    {
        StopAllCoroutines();
        isWaitingForKey = true;
        initColor = m_Image.color;
        m_Image.color = Color.yellow;
        Game.Instance.InChat = true;
    }

    public void CloseBinding()
    {
        isWaitingForKey = false;
        m_Image.color = initColor;

        StartCoroutine(DelayedInChatDisable());
    }

    void OnGUI()
    {
        if(isWaitingForKey)
        {
            keyEvent = Event.current;
            if (keyEvent.isKey)
            {
                if(keyEvent.keyCode != KeyCode.Escape)
                {
                    InputMap.Map[m_txtTitle.text] = keyEvent.keyCode;
                    InputMap.SaveMap();
                    SetInfo(m_txtTitle.text, keyEvent.keyCode);
                }

                CloseBinding();
            }
        }
    }

    private IEnumerator DelayedInChatDisable()
    {
        // give the button a small delay before it starts working since the user just clicked it to set it
        yield return new WaitForSeconds(0.2f);

        Game.Instance.InChat = false;
    }
}
