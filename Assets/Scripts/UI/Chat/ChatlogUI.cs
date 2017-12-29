using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ChatlogUI : MonoBehaviour 
{

    public static ChatlogUI Instance;

    [SerializeField]
    Transform Container;

    [SerializeField]
    int LogCap = 15;

    private Dictionary<Color, bool> toggleState;

    void Awake()
    {
        Instance = this;
        toggleState = new Dictionary<Color, bool>();
        toggleState[ChatConfig.COLOR_AREA] = toggleState[ChatConfig.COLOR_PARTY] = toggleState[ChatConfig.COLOR_WHISPER] = true;
        AddRow(ChatConfig.PLACEHOLDER_WELCOME, ChatConfig.COLOR_AREA);
        AddRow(ChatConfig.PLACEHOLDER_HOW_TO_SWITCH, ChatConfig.COLOR_AREA);
    }

    internal void AddMessage(string name, string message, Color color)
    {
        AddRow(name + ": \"" + message + "\"", color);
    }

    protected void AddRow(string message, Color color)
    {
        GameObject tempObj = Instantiate(ResourcesLoader.Instance.GetObject("ChatLogPiece"));
        tempObj.transform.SetParent(Container, false);
        tempObj.transform.SetAsFirstSibling();
        tempObj.GetComponent<ChatPieceUI>().SetMessage(message, color);
        tempObj.SetActive(toggleState[color]);

        if(Container.childCount > LogCap)
        {
            Destroy(Container.GetChild(0));
        }
    }

    public void toggleArea()
    {
        toggleChatType(ChatConfig.COLOR_AREA);
    }

    public void toggleParty()
    {
        toggleChatType(ChatConfig.COLOR_PARTY);
    }

    public void toggleWhisper()
    {
        toggleChatType(ChatConfig.COLOR_WHISPER);
    }

    private void toggleChatType(Color color)
    {
        bool value = !toggleState[color];
        toggleState[color] = value;
        for (int i = 0; i < Container.childCount; i++)
        {
            GameObject child = Container.transform.GetChild(i).gameObject;
            ChatPieceUI chatPiece = child.GetComponent<ChatPieceUI>();
            if (chatPiece.color == color)
            {
                child.SetActive(value);
            }
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
