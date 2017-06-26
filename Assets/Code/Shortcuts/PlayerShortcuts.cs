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

        ActorInstance actorInstance = GetComponent<ActorInstance>();
        
        if (isMe) 
        {
            list.Add(new KeyAction(OpenInfoUI, "i", "Info of " + actorInstance.Info.Name));
        }
        else 
        {
            list.Add(new KeyAction(OpenInfoUI, "i", "Info of " + actorInstance.Info.Name));
            list.Add(new KeyAction(SendWhisper, "w", "Whisper " + actorInstance.Info.Name));
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
