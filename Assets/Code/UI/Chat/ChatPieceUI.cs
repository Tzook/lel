using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatPieceUI : MonoBehaviour {

    [SerializeField]
    Image m_Background;

    [SerializeField]
    Text m_txtContent;
    
    public void SetMessage(ActorInfo from, string Content)
    {
        m_txtContent.text = from.Name + ": \"" + Content + " \"";
    }
}
