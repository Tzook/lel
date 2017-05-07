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

    Coroutine CurrentDialogRoutine;

    void Awake()
    {
        Instance = this;
    }

	public void StartDialogMode(NPC npc)
    {
        currentNPC = npc;

        inDialog = true;

        SetBlur(true);

        Game.Instance.ClientCharacter.GetComponent<ActorController>().CanInput = false;
        Game.Instance.CanUseUI = false;

        CurrentNPCBubble = ResourcesLoader.Instance.GetRecycledObject("NPCBubble");
        CurrentNPCBubble.transform.position = npc.ChatBubbleSpot.position;
        CurrentNPCBubble.gameObject.SetActive(false);

        StartDialog(npc.DefaultDialog);
    }

    public void StartDialog(string DialogKey)
    {
        StopAllCoroutines();

        StartCoroutine(DialogRoutine(DialogKey));
    }

    public void StopDialogMode()
    {
        StopAllCoroutines();

        inDialog = false;

        SetBlur(false);
        currentNPC = null;

        Game.Instance.ClientCharacter.GetComponent<ActorController>().CanInput = true;
        Game.Instance.CanUseUI = true;

        CurrentNPCBubble.gameObject.SetActive(false);
        CurrentNPCBubble = null;
    }

    private IEnumerator DialogRoutine(string dialogKey)
    {
        CurrentDialog = currentNPC.GetDialog(dialogKey);

        CurrentNPCBubble.gameObject.SetActive(true);

        Text BubbleText = CurrentNPCBubble.transform.GetChild(0).GetChild(0).GetComponent<Text>();

        for(int i=0;i<CurrentDialog.Pieces.Count;i++)
        {
            //Fade in bubble
            CurrentNPCBubble.GetComponent<CanvasGroup>().alpha = 0f;
            while(CurrentNPCBubble.GetComponent<CanvasGroup>().alpha < 1f)
            {
                CurrentNPCBubble.GetComponent<CanvasGroup>().alpha += 6f * Time.deltaTime;
                yield return 0;
            }

            BubbleText.text = "";

            //Display text on bubble
            while (BubbleText.text.Length < CurrentDialog.Pieces[i].Content.Length)
            {
                BubbleText.text += CurrentDialog.Pieces[i].Content[BubbleText.text.Length];

                if (BubbleText.text.Length % 3 == 0)
                {
                    AudioControl.Instance.PlayWithPitch("talksound", CurrentDialog.Pieces[i].Pitch);
                }

                yield return new WaitForSeconds(CurrentDialog.Pieces[i].LetterDelay);
            }

            BubbleText.text = CurrentDialog.Pieces[i].Content;

            while(true)
            {
                if(Input.GetKey(KeyCode.Space))
                {
                    break;
                }

                yield return 0;
            }

            //Fade out bubble
            CurrentNPCBubble.GetComponent<CanvasGroup>().alpha = 1f;
            while (CurrentNPCBubble.GetComponent<CanvasGroup>().alpha > 0f)
            {
                CurrentNPCBubble.GetComponent<CanvasGroup>().alpha -= 6f * Time.deltaTime;
                yield return 0;
            }
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
