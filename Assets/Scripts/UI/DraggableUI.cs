using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class DraggableUI : MonoBehaviour
{
    Coroutine DragRoutineInstance;

    void Awake()
    {
        EventTrigger.Entry CurrentTrigger1 = new EventTrigger.Entry();
        CurrentTrigger1.eventID = EventTriggerType.BeginDrag;

        CurrentTrigger1.callback.AddListener(delegate {
            DragRoutineInstance = StartCoroutine(DragRoutine());
        });


        EventTrigger.Entry CurrentTrigger2 = new EventTrigger.Entry();
        CurrentTrigger2.eventID = EventTriggerType.EndDrag;

        CurrentTrigger2.callback.AddListener(delegate {
            StopCoroutine(DragRoutineInstance);
        });

        EventTrigger.Entry CurrentTrigger3 = new EventTrigger.Entry();
        CurrentTrigger3.eventID = EventTriggerType.PointerDown;

        CurrentTrigger3.callback.AddListener(delegate {
            Game.Instance.isDraggingWindow = true;
        });

        EventTrigger.Entry CurrentTrigger4 = new EventTrigger.Entry();
        CurrentTrigger4.eventID = EventTriggerType.PointerUp;

        CurrentTrigger4.callback.AddListener(delegate {
            Game.Instance.isDraggingWindow = false;
        });


        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger1);
        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger2);
        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger3);
        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger4);
    }

    private IEnumerator DragRoutine()
    {
        // remember the distance from where we clicked on the object and keep it from the updated mouse position
        Vector3 offset = gameObject.transform.position - GameCamera.MousePosition;

        while (true)
        {
            transform.position = GameCamera.MousePosition + offset;
            yield return 0;
        }
    }
}
