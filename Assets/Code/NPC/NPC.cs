using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    public List<Dialog> Dialogs = new List<Dialog>();

    public List<string> GivingQuests = new List<string>();

    public string DefaultDialog;

    public string Name;
    public string Key;

    [SerializeField]
    Transform Body;

    [SerializeField]
    Animator Anim;

    public Transform ChatBubbleSpot;

    public Transform QuestSpot;

    public GameObject CurrentQuestBubble;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Game.Instance.CurrentScene.AddNPC(this);
        RefreshQuestState();
    }

    public void Interact()
    {
        DialogManager.Instance.StartDialogMode(this);
    }

    public Dialog GetDialog(string dialogKey)
    {
        for(int i=0;i<Dialogs.Count;i++)
        {
            if(Dialogs[i].DialogKey == dialogKey)
            {
                return Dialogs[i];
            }
        }

        return null;
    }

    public void SetLayerInChildren(int iLayer, Transform body = null)
    {
        if(body == null)
        {
            body = this.Body;
        }

        body.gameObject.layer = iLayer;

        for(int i=0;i<body.childCount;i++)
        {
            SetLayerInChildren(iLayer, body.GetChild(i));
        }
    }

    public void ExecuteEvent(string eventKey, string eventValue)
    {
        switch(eventKey)
        {
            case "EndDialog":
                {
                    DialogManager.Instance.StopDialogMode();
                    break;
                }
            case "StartDialog":
                {
                    DialogManager.Instance.StartDialog(eventValue);
                    break;
                }
            case "StartQuest":
                {
                    SocketClient.Instance.SendQuestStarted(eventValue);
                    DialogManager.Instance.StopDialogMode();
                    break;
                }
        }
    }

    public void TriggerAnimation(string triggerKey)
    {
        Anim.SetTrigger(triggerKey);
    }

    public void RefreshQuestState()
    {
        if (CurrentQuestBubble != null)
        {
            CurrentQuestBubble.gameObject.SetActive(false);
            CurrentQuestBubble = null;
        }

        if(HasQuestComplete())
        {
            CurrentQuestBubble = ResourcesLoader.Instance.GetRecycledObject("QuestCompleteBubble");
        }
        else if(HasAvailableQuest())
        {
            CurrentQuestBubble = ResourcesLoader.Instance.GetRecycledObject("QuestBubble");
        }
        else if(HasQuestInProgress())
        {
            CurrentQuestBubble = ResourcesLoader.Instance.GetRecycledObject("QuestInProgressBubble");
        }

        if(CurrentQuestBubble != null)
        {
            CurrentQuestBubble.transform.position = QuestSpot.position;
        }
    }


    private bool HasAvailableQuest()
    {
        for(int i=0;i<GivingQuests.Count;i++)
        {
            if(Content.Instance.GetQuest(GivingQuests[i]).IsAvailable(LocalUserInfo.Me.SelectedCharacter))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasQuestInProgress()
    {
        for (int i = 0; i < GivingQuests.Count; i++)
        {
            if (LocalUserInfo.Me.SelectedCharacter.GetQuestProgress(GivingQuests[i]) != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasQuestComplete()
    {
        Quest tempQuest;
        for (int i = 0; i < GivingQuests.Count; i++)
        {
            tempQuest = LocalUserInfo.Me.SelectedCharacter.GetQuestProgress(GivingQuests[i]);
            if (tempQuest != null && tempQuest.Complete)
            {
                return true;
            }   
        }

        return false;
    }
}
