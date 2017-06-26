using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShortcuts: AbstractShortcuts
{
    public bool isMe = false;

    public override string GetActionsFirstKey()
    {
        return isMe ? "m" : "p";
    }

    public override List<KeyAction> GetActions()
    {
        List<KeyAction> list = new List<KeyAction>();
        
        if (isMe) 
        {
            list.Add(new KeyAction(OpenInfoUI, "i"));
        }
        else 
        {
            list.Add(new KeyAction(OpenInfoUI, "i"));
            list.Add(new KeyAction(SendWhisper, "w"));
        }
        
        return list;
    }

    private void OpenInfoUI()
    {
        Debug.Log("Opening info ui!");
    }

    private void SendWhisper()
    {
        Debug.Log("Sending a whisper arr");
    }
}
