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



        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger1);
        GetComponent<EventTrigger>().triggers.Add(CurrentTrigger2);
    }

    private IEnumerator DragRoutine()
    {
        float deltaX = Input.mousePosition.x;
        float deltaY = Input.mousePosition.y;

        while (true)
        {
            transform.position += new Vector3(Input.mousePosition.x - deltaX, Input.mousePosition.y - deltaY, 0f);

            deltaX = Input.mousePosition.x;
            deltaY = Input.mousePosition.y;

            yield return 0;
        }
    }
}
