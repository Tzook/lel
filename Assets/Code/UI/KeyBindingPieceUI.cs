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

    public KeyCode CurrentKey;

    bool isWaitingForKey = false;
    Event keyEvent;
    Color initColor;

    public void SetInfo(string title, KeyCode key)
    {
        CurrentKey = key;
        m_txtTitle.text = title;
        m_txtKey.text = CurrentKey.ToString();
    }

    public void SetBinding()
    {
        isWaitingForKey = true;
        initColor = m_Image.color;
        m_Image.color = Color.yellow;
        Game.Instance.InChat = true;
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


                isWaitingForKey = false;
                m_Image.color = initColor;

                StartCoroutine(DelayedInChatDisable());
            }
        }
    }

    private IEnumerator DelayedInChatDisable()
    {
        yield return 0;

        Game.Instance.InChat = false;
    }
}
