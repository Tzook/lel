using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatPieceUI : MonoBehaviour {

    [SerializeField]
    Image m_Background;

    [SerializeField]
    Text m_txtContent;

    public Color color { get {return m_txtContent.color;} }

    public void SetMessage(string text, Color color)
    {
        m_txtContent.text = text;
        m_txtContent.color = color;
    }
}
