using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class HoverInfoUI : MonoBehaviour
{
    [SerializeField]
    string ContentKey;

    void Awake()
    {
        EventTrigger.Entry CurrentTrigger1 = new EventTrigger.Entry();
        CurrentTrigger1.eventID = EventTriggerType.PointerEnter;

        CurrentTrigger1.callback.AddListener(delegate {
            InGameMainMenuUI.Instance.StatsInfo.Show(Content.Instance.GetInfo(ContentKey));
        });


        EventTrigger.Entry CurrentTrigger2 = new EventTrigger.Entry();
        CurrentTrigger2.eventID = EventTriggerType.PointerExit;

        CurrentTrigger2.callback.AddListener(delegate {
            if (InGameMainMenuUI.Instance.StatsInfo.CurrentPiece.Title == ContentKey)
            {
                InGameMainMenuUI.Instance.StatsInfo.Hide();
            }
        });



        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger1);
        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger2);
    }
}

