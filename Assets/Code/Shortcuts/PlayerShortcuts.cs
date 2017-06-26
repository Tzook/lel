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
            list.Add(new KeyAction(SomeOtherAction, "x", "Some other action"));
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
        InGameMainMenuUI.Instance.ShowCharacterInfo(GetComponent<ActorInstance>().Info);
    }

    private void SomeOtherAction()
    {
        Debug.Log("Some other action!");
    }

    private void SendWhisper()
    {
        Debug.Log("Sending a whisper arr");
    }
}
