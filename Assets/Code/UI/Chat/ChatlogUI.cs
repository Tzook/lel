using UnityEngine;
using System.Collections;
using System;

public class ChatlogUI : MonoBehaviour 
{

    public static ChatlogUI Instance;

    [SerializeField]
    Transform Container;

    [SerializeField]
    int LogCap = 15;

    void Awake()
    {
        Instance = this;
    }

    internal void AddMessage(string name, string message, Color color)
    {
        AddRow(name + ": \"" + message + "\"", color);
    }

    internal void AddPartyMessage(string name, string message)
    {
        AddRow(name + ": \"" + message + "\"", ChatConfig.COLOR_PARTY);
    }

    internal void AddWhisper(string name, string message, bool fromTarget)
    {
        AddRow(name + (fromTarget ? ">>" : "<<") + ": \"" + message + "\"", ChatConfig.COLOR_WHISPER);
    }

    protected void AddRow(string message, Color color)
    {
        GameObject tempObj = Instantiate(ResourcesLoader.Instance.GetObject("ChatLogPiece"));
        tempObj.transform.SetParent(Container, false);
        tempObj.transform.SetAsFirstSibling();
        tempObj.GetComponent<ChatPieceUI>().SetMessage(message, color);

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
