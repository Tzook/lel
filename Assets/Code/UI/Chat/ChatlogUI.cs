using UnityEngine;
using System.Collections;
using System;

public class ChatlogUI : MonoBehaviour {

    public static ChatlogUI Instance;

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
        AddRow(actorInfo.Name + ": \"" + message + " \"");
    }

    internal void AddWhisperTo(string name, string message)
    {
        // TODO think better how to display whispers
        AddRow(name + ">>: \"" + message + " \"");
    }

    internal void AddWhisperFrom(string name, string message)
    {
        // TODO think better how to display whispers
        AddRow(name + "<<: \"" + message + " \"");
    }

    internal void AddWhisperFail(string name)
    {
        AddRow("Failed sending message to " + name);
    }

    protected void AddRow(string message)
    {
        GameObject tempObj = Instantiate(ResourcesLoader.Instance.GetObject("ChatLogPiece"));
        tempObj.transform.SetParent(Container, false);
        tempObj.transform.SetAsFirstSibling();
        tempObj.GetComponent<ChatPieceUI>().SetMessage(message);

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
