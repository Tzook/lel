using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    public List<Dialog> Dialogs = new List<Dialog>();

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
        }
    }

    public void TriggerAnimation(string triggerKey)
    {
        Anim.SetTrigger(triggerKey);
    }
}
