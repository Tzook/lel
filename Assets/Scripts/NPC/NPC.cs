using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour {

    public List<Dialog> Dialogs = new List<Dialog>();

    [Popup(/* AUTO_GENERATED_QUESTS_START */ "", "turtleProblem", "turtleSoup", "TurtleQuizz", "rabbitRaids", "FatAlbert", "carrotSupply", "becomingWarrior", "helpMaya", "mayaSpikedTurtles", "findAlex", "helpAlex", "findKaren", "petRansom", "examineLostSupplies", "helpJaxTheDog", "helpJaxTheDog2", "ruinedPainting", "findCosmo", "findCosmo2", "findCosmo3", "petRansom2", "jacksVengeance", "jacksVengeance2", "jacksVengeance3", "CleaningUp", "ABeanyRequest", "OldFriends", "RangerTest", "FrostTest", "hairPotion", "untieNurtle", "breakIntoSpa" /* AUTO_GENERATED_QUESTS_END */)]
    public List<string> GivingQuests = new List<string>();
    
    [Popup(/* AUTO_GENERATED_QUESTS_START */ "", "turtleProblem", "turtleSoup", "TurtleQuizz", "rabbitRaids", "FatAlbert", "carrotSupply", "becomingWarrior", "helpMaya", "mayaSpikedTurtles", "findAlex", "helpAlex", "findKaren", "petRansom", "examineLostSupplies", "helpJaxTheDog", "helpJaxTheDog2", "ruinedPainting", "findCosmo", "findCosmo2", "findCosmo3", "petRansom2", "jacksVengeance", "jacksVengeance2", "jacksVengeance3", "CleaningUp", "ABeanyRequest", "OldFriends", "RangerTest", "FrostTest", "hairPotion", "untieNurtle", "breakIntoSpa" /* AUTO_GENERATED_QUESTS_END */)]
    public List<string> EndingQuests = new List<string>();

    public List<TeleportableScene> TeleportableScenes = new List<TeleportableScene>();

    public List<StoreProduct> SellingItems = new List<StoreProduct>();

    public string DefaultDialog;

    public string IntroAnimation;
    public string EndAnimation;

    public string Name;
    public string Key;

    [SerializeField]
    Transform Body;

    [SerializeField]
    Animator Anim;

    [SerializeField]
    Text NameTag;

    public Transform ChatBubbleSpot;

    public Transform QuestSpot;

    public GameObject CurrentQuestBubble;
    
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        StartCoroutine(InitRoutine());
    }

    private IEnumerator InitRoutine()
    {
        while(Game.Instance.CurrentScene == null)
        {
            yield return 0;
        }

        Game.Instance.CurrentScene.AddNPC(this);

        NameTag.text = Name;
        NameTag.gameObject.SetActive(false);

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
                    DialogManager.Instance.StopDialogMode();
                    SocketClient.Instance.SendQuestStarted(eventValue, Key);
                    Game.Instance.HandleOkRoutines(eventValue);
                    break;
                }
            case "CompleteQuest":
                {
                    DialogManager.Instance.StopDialogMode();
                    InGameMainMenuUI.Instance.RecieveQuestReward(Content.Instance.GetQuest(eventValue), Key);
                    break;
                }
            case "QuestOK":
                {
                    SocketClient.Instance.SendQuestOK(eventValue);
                    DialogManager.Instance.StopDialogMode();
                    break;
                }
            case "Trade":
                {
                    DialogManager.Instance.StartVendorMode();
                    break;
                }
            case "Teleport":
                {
                    DialogManager.Instance.StopDialogMode();
                    SocketClient.Instance.SendNpcTeleport(Key, eventValue);
                    break;   
                }
            case "TeleportToInstance":
                {
                    DialogManager.Instance.StopDialogMode();
                    SocketClient.Instance.SendNpcTeleport(Key, eventValue, true);
                    break;   
                }
            case "InteractWithNPC":
                {
                    DialogManager.Instance.StopDialogMode();

                    string brokenValueA = "";
                    string brokenValueB = "";

                    for (int i=0;i<eventValue.Length;i++)
                    {
                        if(eventValue[i] == '_')
                        {
                            brokenValueA = eventValue.Substring(0, i);
                            brokenValueB = eventValue.Substring(i + 1, eventValue.Length - (i + 1));

                            Debug.Log(brokenValueA + " | " + brokenValueB);

                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(brokenValueA) && !string.IsNullOrEmpty(brokenValueB))
                    {
                        DialogManager.Instance.HideNPCBubble();
                        DialogManager.Instance.HideOptionsBubble();

                        NPC tempNPC = Game.Instance.CurrentScene.GetNPC(brokenValueA);
                        tempNPC.Interact();
                        DialogManager.Instance.StartDialog(brokenValueB);
                    }
                    break;
                }
            case "TriggerAnimation":
                {
                    DialogManager.Instance.StopDialogMode();

                    TriggerAnimation(eventValue);

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
            CurrentQuestBubble.GetComponent<QuestBubbleCollider>().npc = this;
            CurrentQuestBubble.transform.position = QuestSpot.position;
        }
    }

    public void ShowName()
    {
        NameTag.gameObject.SetActive(true);
    }

    public void HideName()
    {
        NameTag.gameObject.SetActive(false);
    }


    private bool HasAvailableQuest()
    {
        for(int i=0;i<GivingQuests.Count;i++)
        {
            if(Content.Instance.GetQuest(GivingQuests[i]).IsAvailable(LocalUserInfo.Me.ClientCharacter))
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
            if (LocalUserInfo.Me.ClientCharacter.GetQuest(GivingQuests[i]) != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasQuestComplete()
    {
        Quest tempQuest;
        for (int i = 0; i < EndingQuests.Count; i++)
        {
            tempQuest = LocalUserInfo.Me.ClientCharacter.GetQuest(EndingQuests[i]);
            if (tempQuest != null && tempQuest.CanBeCompleted)
            {
                return true;
            }   
        }

        return false;
    }


    public int GetItemIndex(string key)
    {
        for (int i = 0; i < SellingItems.Count; i++)
        {
            if (SellingItems[i].itemKey == key)
            {
                return i;
            }
        }

        return -1;
    }
}

[System.Serializable]
public class StoreProduct
{
    [Popup(/* AUTO_GENERATED_LOOT_START */ "", "leatherVest", "adventurerShirt", "blackGloves", "clothPants", "greenPants", "leatherShoes", "strapShoes", "swordOfElad", "turtleShell", "turtleShellOld", "turtleShellSpiked", "gold", "blackPants", "leatherGloves", "apprenticeRobeWhite", "apprenticeRobeBlack", "shortSword", "shortClub", "shortAxe", "shortScimitar", "shortBow", "rabbitEar", "shortDagger", "batWing", "rabbitFurVest", "oldTurtleShell", "Fishing Rod", "carrotSack", "smallFish", "bigFish", "blackShoes", "whiteJellyBean", "blackJellyBean", "yellowJellyBean", "redJellyBean", "pinkJellyBean", "blueJellyBean", "orangeJellyBean", "greenJellyBean", "plantFlower", "blueMushroomCap", "strawHat", "peasantHat", "rabbitCostume", "whiteGloves", "greenGloves", "redGloves", "cosmoTunnelKey", "rabbitLandEntrancePremission", "magicCarrotSeeds" /* AUTO_GENERATED_LOOT_END */)]
    public string itemKey;
    public GameObject ItemObject;
}

[System.Serializable]
public class TeleportableScene
{
    public string sceneKey;
    public string portalKey;
}
