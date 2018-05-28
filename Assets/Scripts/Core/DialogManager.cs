using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    public bool inDialog;

    public static DialogManager Instance;

    public NPC currentNPC;

    Dialog CurrentDialog;
    GameObject CurrentNPCBubble;
    GameObject CurrentOptionsFrame;

    Coroutine CurrentDialogRoutine;
    Coroutine CurrentGlowProductRoutine;

    int PreviousItemIndex = 0;
    int CurrentVendorItemIndex = 0;

    bool isVendorMode = false;

    BoxCollider2D PreservedRope;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(inDialog)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                currentNPC.ExecuteExitEvent();

                StopDialogMode();

                if (isVendorMode)
                {
                    StopVendorMode();
                }
            }
        }
    }

	public void StartDialogMode(NPC npc)
    {
        currentNPC = npc;

        currentNPC.ShowName(true);

        inDialog = true;

        // we are changing layers, so it will trigger an exit with the rope collider - so remember it and put it back when done
        PreservedRope = Game.Instance.ClientCharacter.GetComponent<ActorController>().CurrentRope;

        SetBlur(true);

        Game.Instance.ClientCharacter.GetComponent<ActorController>().InteractWithNpc();
        
        CurrentNPCBubble = ResourcesLoader.Instance.GetRecycledObject("NPCBubble");
        CurrentNPCBubble.transform.position = npc.ChatBubbleSpot.position;
        CurrentNPCBubble.gameObject.SetActive(false);

        CurrentOptionsFrame = ResourcesLoader.Instance.GetRecycledObject("DialogOptionsFrame");
        CurrentOptionsFrame.transform.position = Game.Instance.ClientCharacter.GetComponent<ActorInstance>().NameLabel.transform.position;
        CurrentOptionsFrame.gameObject.SetActive(false);

        if(!string.IsNullOrEmpty(currentNPC.IntroAnimation))
        {
            currentNPC.TriggerAnimation(currentNPC.IntroAnimation);
        }

       

        StartDialog(npc.DefaultDialog);
    }

    public void StartDialog(string DialogKey)
    {
        StopAllCoroutines();

        StartCoroutine(DialogRoutine(DialogKey));
    }

    public void StopDialogMode()
    {
        if (currentNPC != null)
        {
            if (!string.IsNullOrEmpty(currentNPC.EndAnimation))
            {
                currentNPC.TriggerAnimation(currentNPC.EndAnimation);
            }

            currentNPC.HideName(true);
        }

        StopAllCoroutines();

        StartCoroutine(StopDialogRoutine());

    }

    private IEnumerator StopDialogRoutine()
    {
        yield return 0;

        inDialog = false;

        SetBlur(false);
        currentNPC = null;

        Game.Instance.ClientCharacter.GetComponent<ActorController>().CurrentRope = PreservedRope;
        Game.Instance.IsChattingWithNpc = false;

        HideNPCBubble();

        HideOptionsBubble();
    }

    public void HideNPCBubble()
    {
        if (CurrentNPCBubble != null)
        {
            CurrentNPCBubble.gameObject.SetActive(false);
            CurrentNPCBubble = null;
        }
    }

    public void HideOptionsBubble()
    {
        if (CurrentOptionsFrame != null)
        {
            CurrentOptionsFrame.gameObject.SetActive(false);
            CurrentOptionsFrame = null;
        }
    }

    private IEnumerator DialogRoutine(string dialogKey)
    {
        CurrentDialog = currentNPC.GetDialog(dialogKey);

        CurrentNPCBubble.gameObject.SetActive(true);

        CurrentOptionsFrame.SetActive(true);

        CurrentOptionsFrame.GetComponent<DialogOptionsFrameUI>().StopWavingInstruction();

        ClearCurrentOptionsFrame();

        Text BubbleText = CurrentNPCBubble.transform.GetChild(0).GetChild(0).GetComponent<Text>();

        for(int i=0;i<CurrentDialog.Pieces.Count;i++)
        {

            BubbleText.text = "...";

            yield return 0;

            BubbleText.text = "";

            yield return 0;

            CurrentNPCBubble.transform.position = currentNPC.ChatBubbleSpot.position;

            //Fade in bubble
            CurrentNPCBubble.GetComponent<CanvasGroup>().alpha = 0f;
            while(CurrentNPCBubble.GetComponent<CanvasGroup>().alpha < 1f)
            {
                CurrentNPCBubble.GetComponent<CanvasGroup>().alpha += 8f * Time.deltaTime;
                yield return 0;
            }

            if(!string.IsNullOrEmpty(CurrentDialog.Pieces[i].AnimationTrigger))
            {
                currentNPC.TriggerAnimation(CurrentDialog.Pieces[i].AnimationTrigger);
            }

            if(!string.IsNullOrEmpty(CurrentDialog.Pieces[i].SoundKey))
            {
                AudioControl.Instance.Play(CurrentDialog.Pieces[i].SoundKey);
            }

            //Display text on bubble
            while (BubbleText.text.Length < CurrentDialog.Pieces[i].Content.Length)
            {
                BubbleText.text += CurrentDialog.Pieces[i].Content[BubbleText.text.Length];

                if (BubbleText.text.Length % 3 == 0)
                {
                    AudioControl.Instance.PlayWithPitch("talksound", CurrentDialog.Pieces[i].Pitch);
                }

                if (ContinueChat())
                {
                    // skipping - make the letters come twice a frame
                    if (BubbleText.text.Length % 2 == 0)
                    {
                        yield return 0;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(CurrentDialog.Pieces[i].LetterDelay);
                }
            }

            if (ContinueChat())
            {
                // skipping - delay it a bit, to finish reading briefly
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return 0;
            }

            CurrentOptionsFrame.GetComponent<DialogOptionsFrameUI>().StartWavingInstruction("Press 'Space' to continue...");

            while (true)
            {
                if (ContinueChat())
                {
                    if (!string.IsNullOrEmpty(CurrentDialog.Pieces[i].PostPieceEvent))
                    {
                        currentNPC.ExecuteEvent(CurrentDialog.Pieces[i].PostPieceEvent, CurrentDialog.Pieces[i].PostPieceEventValue);
                        yield break;
                    }

                    break;
                }

                yield return 0;
            }

            CurrentOptionsFrame.GetComponent<DialogOptionsFrameUI>().StopWavingInstruction();

            //Fade out bubble
            CurrentNPCBubble.GetComponent<CanvasGroup>().alpha = 1f;
            while (CurrentNPCBubble.GetComponent<CanvasGroup>().alpha > 0f)
            {
                CurrentNPCBubble.GetComponent<CanvasGroup>().alpha -= 8f * Time.deltaTime;
                yield return 0;
            }

            if(CurrentDialog == null)
            {
                yield break;
            }

            if (!string.IsNullOrEmpty(CurrentDialog.Pieces[i].PostPieceEvent))
            {
                currentNPC.ExecuteEvent(CurrentDialog.Pieces[i].PostPieceEvent, CurrentDialog.Pieces[i].PostPieceEventValue);
                yield break;
            }
        }

        //Spawn dialog options on frame.
        GameObject tempOption;
        CanvasGroup tempCG;
        for (int i = CurrentDialog.Options.Count - 1; i >= 0 ;i--)
        {
            if (CanShowOption(CurrentDialog.Options[i]))
            {
                tempOption = ResourcesLoader.Instance.GetRecycledObject("DialogOption");
                tempOption.transform.SetParent(CurrentOptionsFrame.transform.GetChild(0), false);
                tempOption.GetComponent<Button>().onClick.RemoveAllListeners();

                if (CurrentDialog.Options[i].CLR == Color.white)
                {
                    tempOption.GetComponent<Outline>().effectColor = Color.clear;
                }
                else
                {
                    tempOption.GetComponent<Outline>().effectColor = CurrentDialog.Options[i].CLR;
                }

                tempOption.transform.GetChild(0).GetComponent<Text>().text = CurrentDialog.Options[i].Title;

                AddDialogOptionEvent(tempOption.GetComponent<Button>(), CurrentDialog.Options[i].Event, CurrentDialog.Options[i].Value);

                tempCG = tempOption.GetComponent<CanvasGroup>();
                tempCG.alpha = 0f;

                while (tempCG.alpha < 1f)
                {
                    tempCG.alpha += 8f * Time.deltaTime;
                    yield return 0;
                }
            }
        }

        CurrentOptionsFrame.GetComponent<DialogOptionsFrameUI>().StartWavingInstruction("Choose your response...");


    }

    private static bool ContinueChat()
    {
        return Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
    }

    public bool CanShowOption(DialogOption option)
    {
        if(string.IsNullOrEmpty(option.Condition))
        {
            return true;
        }

        switch(option.Condition)
        {
            case "QuestAvailable":
                {
                    if(Content.Instance.GetQuest(option.ConditionValue).IsAvailable(LocalUserInfo.Me.ClientCharacter))
                    {
                            return true;
                    }

                    break;
                }
            case "QuestInProgress":
                {
                    Quest tempQuest = LocalUserInfo.Me.ClientCharacter.GetQuest(option.ConditionValue);
                    if (tempQuest != null && !tempQuest.CanBeCompleted)
                    {
                        return true;
                    }

                    break;
                }
            case "QuestComplete":
                {
                    Quest tempQuest = LocalUserInfo.Me.ClientCharacter.GetQuest(option.ConditionValue);

                    if (tempQuest != null && tempQuest.CanBeCompleted)
                    {
                        return true;
                    }

                    break;
                }
            case "QuestCompleted":
                {
                    return LocalUserInfo.Me.ClientCharacter.CompletedQuests.Contains(option.ConditionValue);
                }
            case "ItemEquipped":
                {
                    return LocalUserInfo.Me.ClientCharacter.Equipment.isEquipping(option.ConditionValue);
                }
            case "HasItem":
                {
                    return (LocalUserInfo.Me.ClientCharacter.Inventory.GetItem(option.ConditionValue) != null);
                }
            case "DungeonUnready":
                {
                    return !Content.Instance.GetDungeon(option.ConditionValue).isReady;
                }
            case "DungeonReady":
                {
                    return Content.Instance.GetDungeon(option.ConditionValue).isReady;
                }
        }

        return false;
    }

    public void AddDialogOptionEvent(Button btn, string eventKey, string eventValue)
    {
        btn.onClick.AddListener(delegate
        {
            currentNPC.ExecuteEvent(eventKey, eventValue);
        });
    }

    public void ClearCurrentOptionsFrame()
    {
        //Clear current options container from leftover options
        while (CurrentOptionsFrame.transform.GetChild(0).childCount > 0)
        {
            CurrentOptionsFrame.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            CurrentOptionsFrame.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
            CurrentOptionsFrame.transform.GetChild(0).transform.GetChild(0).SetParent(CurrentOptionsFrame.transform);
        }
    }

    public void SetBlur(bool state)
    {
        ActorInstance actorInstance = Game.Instance.ClientCharacter.GetComponent<ActorInstance>();
        GameCamera.Instance.SetBlurMode(state);
        if (state)
        {
            currentNPC.SetLayerInChildren(13);
            actorInstance.SetRenderingLayer(13);
        }
        else
        {
            currentNPC.SetLayerInChildren(0);
            actorInstance.SetRenderingLayer(8);
        }
    }

    public void StartVendorMode()
    {
        isVendorMode = true;

        CurrentNPCBubble.gameObject.SetActive(false);
        CurrentNPCBubble = null;

        CurrentOptionsFrame.gameObject.SetActive(false);
        CurrentOptionsFrame = null;

        CurrentVendorItemIndex = 0;
        RefreshVendorItem();
    }

    public void NextItem()
    {
        PreviousItemIndex = CurrentVendorItemIndex;
        CurrentVendorItemIndex++;

        if (CurrentVendorItemIndex >= currentNPC.SellingItems.Count)
        {
            CurrentVendorItemIndex = 0;
        }

        RefreshVendorItem();
    }

    public void PreviousItem()
    {
        PreviousItemIndex = CurrentVendorItemIndex;
        CurrentVendorItemIndex--;

        if (CurrentVendorItemIndex < 0)
        {
            CurrentVendorItemIndex = currentNPC.SellingItems.Count - 1;
        }

        RefreshVendorItem();
    }

    public void RefreshVendorItem()
    {
        AudioControl.Instance.Play("swosh");

        GameCamera.Instance.FocusOnTransform(currentNPC.SellingItems[CurrentVendorItemIndex].ItemObject.transform);
        InGameMainMenuUI.Instance.ShowVendorPanel(currentNPC.SellingItems[CurrentVendorItemIndex].itemKey, currentNPC);

        if(CurrentGlowProductRoutine != null)
        {
            StopCoroutine(CurrentGlowProductRoutine);
        }

        CurrentGlowProductRoutine = StartCoroutine(GlowProductItem(currentNPC.SellingItems[CurrentVendorItemIndex].ItemObject));
    }

    IEnumerator GlowProductItem(GameObject item)
    {
        SpriteRenderer previousImg = currentNPC.SellingItems[PreviousItemIndex].ItemObject.GetComponent<SpriteRenderer>();
        previousImg.color = new Color(previousImg.color.r, previousImg.color.g, previousImg.color.b, 1f);
        
        SpriteRenderer img = item.GetComponent<SpriteRenderer>();

        float t;
        while (true)
        {
            t = 1f;
            while (t > 0f)
            {
                t -= 3f * Time.deltaTime;

                img.color = new Color(img.color.r, img.color.g, img.color.b, t);

                yield return 0;
            }

            t = 0f;
            while (t < 1f)
            {
                t += 3f * Time.deltaTime;

                img.color = new Color(img.color.r, img.color.g, img.color.b, t);

                yield return 0;
            }
        }
    }

    public void StopVendorMode()
    {
        isVendorMode = false;
        GameCamera.Instance.FocusDefault();
        InGameMainMenuUI.Instance.HideVendorPanel();

        if (CurrentGlowProductRoutine != null)
        {
            StopCoroutine(CurrentGlowProductRoutine);
            CurrentGlowProductRoutine = null;
        }

        SpriteRenderer lastItemImg = currentNPC.SellingItems[CurrentVendorItemIndex].ItemObject.GetComponent<SpriteRenderer>();
        lastItemImg.color = new Color(lastItemImg.color.r, lastItemImg.color.g, lastItemImg.color.b, 1f);

        StopDialogMode();
    }

}
