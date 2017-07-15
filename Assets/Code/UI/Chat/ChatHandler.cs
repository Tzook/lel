using UnityEngine;

public class ChatHandler: MonoBehaviour
{
    public static ChatHandler Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ReceiveChatMessage(string actorID, string message)
    {
        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(actorID);

        if (actorInfo != null && actorInfo.Instance)
        {
            ShowMessage(actorInfo.Instance, message, ChatConfig.COLOR_AREA);
        }
        else
        {
            Debug.LogError(actorID + " is no longer in the room for display chat.");
        }
    }

    public void ReceivePartyMessage(string name, string message)
    {
        KnownCharacter character = LocalUserInfo.Me.GetKnownCharacter(name);
        ActorInstance actorInstance = null;
        if (character != null)
        {
            string partyMemberId = character.Info.ID;
            ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(partyMemberId);
            if (actorInfo != null)
            {
                actorInstance = actorInfo.Instance;
            }
        }
        else 
        {
            Debug.LogError(name + " is no longer a known character for party message.");
        }

        ShowMessage(actorInstance, message, ChatConfig.COLOR_PARTY, name);
    }

    public void ReceiveWhisper(string name, string message)
    {
        // TODO handle
    }

    public void SendChatMessage(string text)
    {
        SocketClient.Instance.SendChatMessage(text);
        ShowMyMessage(text, ChatConfig.COLOR_AREA);
    }

    public void SendPartyMessage(string text)
    {
        SocketClient.Instance.SendPartyMessage(text);
        ShowMyMessage(text, ChatConfig.COLOR_PARTY);
    }

    public void SendWhisper(string text, string targetName)
    {
        SocketClient.Instance.SendWhisper(text, targetName);
        ShowMyMessage(text, ChatConfig.COLOR_WHISPER, targetName + ">>");
    }

    private void ShowMyMessage(string text, Color color, string name = "")
    {
        ActorInstance actor = Game.Instance.ClientCharacter.GetComponent<ActorInstance>();
        ShowMessage(actor, text, color, name);
    }

    private void ShowMessage(ActorInstance actor, string text, Color color, string name = "")
    {
        if (actor != null)
        {
            actor.ChatBubble(text, color);
            name = name.Length > 0 ? name : actor.Info.Name;
        }
        ChatlogUI.Instance.AddMessage(name, text, color);
        InGameMainMenuUI.Instance.SetLastChatMessage(name + ": \"" + text + "\"", color);
    }
}