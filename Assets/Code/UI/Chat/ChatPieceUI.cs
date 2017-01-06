using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatPieceUI : MonoBehaviour {

    [SerializeField]
    Image m_Background;

    [SerializeField]
    Text m_txtContent;

    public void SetMessage(string text)
    {
        m_txtContent.text = text;
    }
}
