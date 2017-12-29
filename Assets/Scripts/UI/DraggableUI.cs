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
        float deltaX = GameCamera.MousePosition.x;
        float deltaY = GameCamera.MousePosition.y;

        while (true)
        {
            transform.position += new Vector3(GameCamera.MousePosition.x - deltaX, GameCamera.MousePosition.y - deltaY, 0f);
            //transform.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
            deltaX = GameCamera.MousePosition.x;
            deltaY = GameCamera.MousePosition.y;

            yield return 0;
        }
    }
}
