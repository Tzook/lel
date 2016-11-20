using UnityEngine;
using System.Collections;
using System;

public class ChatLogUI : MonoBehaviour {

    public static ChatLogUI Instance;

    [SerializeField]
    Transform Container;

    [SerializeField]
    int LogCap = 15;

    void Awake()
    {
        Instance = this;
    }

    internal void AddMessage(ActorInfo actorInfo, string message)
    {
        GameObject tempObj = Instantiate(ResourcesLoader.Instance.GetObject("ChatLogPiece"));
        tempObj.transform.SetParent(Container, false);
        tempObj.transform.SetAsFirstSibling();
        tempObj.GetComponent<ChatPieceUI>().SetMessage(actorInfo, message);

        if(Container.childCount > LogCap)
        {
            Destroy(Container.GetChild(0));
        }
    }

    public void ClearLog()
    {
        while(Container.childCount > 0)
        {
            Destroy(Container.transform.GetChild(0).gameObject);
        }
    }
}
