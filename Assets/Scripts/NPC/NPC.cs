using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour {

    public List<Dialog> Dialogs = new List<Dialog>();

    [Popup(/* AUTO_GENERATED_QUESTS_START */ "NO VALUE", "ABeanyRequest", "aidingTheCommunity", "ancientRabbitholes", "bigOnes", "bigOnesDivine", "bigOnesIce", "bigOnesMelting", "bigOnesScimitar", "blockingTheView", "breakIntoSpa", "bustNuts", "carrotSupply", "CleaningUp", "divinePlace", "examineLostSupplies", "FatAlbert", "findAlex", "findCosmo", "findCosmo2", "findCosmo3", "findKaren", "frostPractice", "FrostTest", "hairPotion", "helpAlex", "helpJaxTheDog", "helpJaxTheDog2", "helpMaya", "jacksVengeance", "jacksVengeance2", "jacksVengeance3", "joinShrine", "mayaSpikedTurtles", "OldFriends", "petRansom", "petRansom2", "picnicSupplies", "PirateBay", "piratesAmbush", "practiceHealing", "rabbitRaids", "rangerPractice", "RangerPractice2", "RangerTest", "ruinedPainting", "sacrificeFrostGod", "summonAnAngel", "thisIsNecessary1", "thisIsNecessary2", "thisIsNecessary3", "turtleProblem", "TurtleQuizz", "turtleSoup", "untieNurtle" /* AUTO_GENERATED_QUESTS_END */)]
    public List<string> GivingQuests = new List<string>();
    
    [Popup(/* AUTO_GENERATED_QUESTS_START */ "NO VALUE", "ABeanyRequest", "aidingTheCommunity", "ancientRabbitholes", "bigOnes", "bigOnesDivine", "bigOnesIce", "bigOnesMelting", "bigOnesScimitar", "blockingTheView", "breakIntoSpa", "bustNuts", "carrotSupply", "CleaningUp", "divinePlace", "examineLostSupplies", "FatAlbert", "findAlex", "findCosmo", "findCosmo2", "findCosmo3", "findKaren", "frostPractice", "FrostTest", "hairPotion", "helpAlex", "helpJaxTheDog", "helpJaxTheDog2", "helpMaya", "jacksVengeance", "jacksVengeance2", "jacksVengeance3", "joinShrine", "mayaSpikedTurtles", "OldFriends", "petRansom", "petRansom2", "picnicSupplies", "PirateBay", "piratesAmbush", "practiceHealing", "rabbitRaids", "rangerPractice", "RangerPractice2", "RangerTest", "ruinedPainting", "sacrificeFrostGod", "summonAnAngel", "thisIsNecessary1", "thisIsNecessary2", "thisIsNecessary3", "turtleProblem", "TurtleQuizz", "turtleSoup", "untieNurtle" /* AUTO_GENERATED_QUESTS_END */)]

    public List<string> EndingQuests = new List<string>();

    public List<TeleportableScene> TeleportableScenes = new List<TeleportableScene>();

    public List<StoreProduct> SellingItems = new List<StoreProduct>();

    public string DefaultDialog;

    public string IntroAnimation;
    public string EndAnimation;

    public string Name;
    public string Key;

    public string ExitEvent;
    public string ExitEventValue;

    [SerializeField]
    Transform Body;

    [SerializeField]
    Animator Anim;

    [SerializeField]
    Text NameTag;

    FadeText NameTagFader;

    public Transform ChatBubbleSpot;

    public Transform QuestSpot;

    public GameObject CurrentQuestBubble;


    private void OnEnable()
    {
        NameTagFader = gameObject.AddComponent<FadeText>();
        NameTagFader.text = NameTag;
    }

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
        HideName(true);

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
                    StartQuest(eventValue);
                    break;
                }
            case "CompleteQuest":
                {
                    CompleteQuest(eventValue);
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
            case "StartDungeon":
                {
                    DialogManager.Instance.StopDialogMode();

                    SocketClient.Instance.SendStartedDungeon(eventValue);

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

    public void ShowName(bool instant)
    {
        NameTagFader.FadeIn(instant ? 0 : 0.1f);
    }

    public void HideName(bool instant)
    {
        NameTagFader.FadeOut(instant ? 0 : 0.1f);
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

    public void ExecuteExitEvent()
    {
        if(string.IsNullOrEmpty(ExitEvent))
        {
            return;
        }

        ExecuteEvent(ExitEvent, ExitEventValue);
    }

    void OnMouseOver()
    {
        ShowName(false);
    }

    void OnMouseExit()
    {
        if (!DialogManager.Instance.inDialog || DialogManager.Instance.currentNPC != this)
        {
            HideName(false);
        }
    }

    public void StartQuest(string eventValue)
    {
        DialogManager.Instance.StopDialogMode();
        SocketClient.Instance.SendQuestStarted(eventValue, Key);
        Game.Instance.HandleOkRoutines(eventValue);
    }

    public void CompleteQuest(string eventValue)
    {
        DialogManager.Instance.StopDialogMode();
        InGameMainMenuUI.Instance.RecieveQuestReward(Content.Instance.GetQuest(eventValue), Key);
    }
}

[System.Serializable]
public class StoreProduct
{
    [Popup(/* AUTO_GENERATED_LOOT_START */ "NO VALUE", "acolytehood", "acolyterobe", "acorn", "adventurerShirt", "apprenticeRobeBlack", "apprenticeRobeWhite", "archershat", "batWing", "bigFish", "blackbandana", "blackclothshirt", "blackGloves", "blackJellyBean", "blackkercheif", "blackPants", "blackpeasentshirt", "blackShoes", "blueBerries", "blueJellyBean", "bluekercheif", "blueMushroomCap", "bluepeasentshirt", "blueponcho", "brownpeasentshirt", "brownponcho", "cabbage", "captainhat", "captianscoat", "carrot", "carrotSack", "chainlinkhelmet", "charredrobe", "clothPants", "commonaxe", "commonschimitar", "commonsword", "cosmoTunnelKey", "cozyslippers", "cutlass", "dirk", "divinebook", "drainingstaff", "executionerbandana", "Fishing Rod", "forbiddendirk", "gold", "greenGloves", "greenJellyBean", "greenPants", "greenpeasentshirt", "greenponcho", "icystaff", "idoloftrust", "improvedshortbow", "leatherarmor", "leatherarmor", "leatherGloves", "leatherpants", "leatherShoes", "leatherVest", "lightgambesson", "longdagger", "longhammer", "magicCarrotSeeds", "mailarmor", "mailpants", "mailsabatons", "mailshoes", "meltingrod", "metalbow", "metalhelmet", "nutVaultKey", "oldTurtleShell", "orangeclothshirt", "orangeJellyBean", "orbofenergy", "peasantHat", "pinkclothshirt", "pinkJellyBean", "PirateSupply", "plantFlower", "priestrobe", "pyromancerrobe", "rabbitBossEars", "rabbitCostume", "rabbitEar", "rabbitfurshoes", "rabbitFurVest", "rabbitLandEntrancePremission", "redApple", "redbandna", "redBerries", "redGloves", "redJellyBean", "redpeasentshirt", "shortAxe", "shortBow", "shortClub", "shortcutlass", "shortDagger", "shortScimitar", "shortSword", "slipers", "smallFish", "snailhat", "spear", "squirrelBossMustache", "strapShoes", "strawHat", "swordOfElad", "tatteredblackpants", "tatteredbrownpants", "tatteredgreenpants", "tatteredwhitepants", "tauntingfork", "threateningfork", "tomato", "torch", "turtleShell", "turtleshellarmor", "turtleShellOld", "turtleShellSpiked", "turtleSoup", "twohandedaxe", "twohandedschimitar", "twohandedsword", "VilePetal", "wand", "wandoffrost", "warbow", "warlockrobe", "whiteclothshirt", "whiteGloves", "whiteJellyBean", "wizardhood", "wizardrobe", "woodenhammer", "woodenpole", "wormBossAntenna", "yellowbandana", "yellowJellyBean", "yellowkercheif", "yellowshoes" /* AUTO_GENERATED_LOOT_END */)]
    public string itemKey;
    public GameObject ItemObject;
}

[System.Serializable]
public class TeleportableScene
{
    public string sceneKey;
    public string portalKey;
    public bool allowParty;
}
