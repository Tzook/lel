using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    public bool inDialog;

    public static DialogManager Instance;

    NPC currentNPC;
    Dialog CurrentDialog;
    GameObject CurrentNPCBubble;
    GameObject CurrentOptionsFrame;

    Coroutine CurrentDialogRoutine;

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
                StopDialogMode();
            }
        }
    }

	public void StartDialogMode(NPC npc)
    {
        currentNPC = npc;

        currentNPC.ShowName();

        inDialog = true;

        SetBlur(true);

        Game.Instance.ClientCharacter.GetComponent<ActorController>().CanInput = false;
        Game.Instance.CanUseUI = false;

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
        if (!string.IsNullOrEmpty(currentNPC.EndAnimation))
        {
            currentNPC.TriggerAnimation(currentNPC.EndAnimation);
        }

        currentNPC.HideName();

        StopAllCoroutines();

        StartCoroutine(StopDialogRoutine());

    }

    private IEnumerator StopDialogRoutine()
    {
        yield return 0;

        inDialog = false;

        SetBlur(false);
        currentNPC = null;

        Game.Instance.ClientCharacter.GetComponent<ActorController>().CanInput = true;
        Game.Instance.CanUseUI = true;

        CurrentNPCBubble.gameObject.SetActive(false);
        CurrentNPCBubble = null;

        CurrentOptionsFrame.gameObject.SetActive(false);
        CurrentOptionsFrame = null;
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

            //Display text on bubble
            while (BubbleText.text.Length < CurrentDialog.Pieces[i].Content.Length)
            {
                BubbleText.text += CurrentDialog.Pieces[i].Content[BubbleText.text.Length];

                if (BubbleText.text.Length % 3 == 0)
                {
                    AudioControl.Instance.PlayWithPitch("talksound", CurrentDialog.Pieces[i].Pitch);
                }

                if(ContinueChat())
                {
                    break;
                }

                yield return new WaitForSeconds(CurrentDialog.Pieces[i].LetterDelay);
            }

            BubbleText.text = CurrentDialog.Pieces[i].Content;

            yield return 0;

            CurrentOptionsFrame.GetComponent<DialogOptionsFrameUI>().StartWavingInstruction("Press 'Space' to continue...");

            while (true)
            {
                if (ContinueChat())
                {
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
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
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
                    if (LocalUserInfo.Me.ClientCharacter.GetQuest(option.ConditionValue) != null)
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
        GameCamera.Instance.SetBlurMode(state);

        if (state)
        {
            currentNPC.SetLayerInChildren(13);
            Game.Instance.ClientCharacter.GetComponent<ActorInstance>().SetRenderingLayer(13);
        }
        else
        {
            currentNPC.SetLayerInChildren(0);
            Game.Instance.ClientCharacter.GetComponent<ActorInstance>().SetRenderingLayer(8);
        }
    }
}
