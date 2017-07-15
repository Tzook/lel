using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ClickableUI: MonoBehaviour, IPointerDownHandler 
{
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        // 1. never focus an item, so we can still continue playing
        EventSystem.current.SetSelectedGameObject(Game.Instance.GetComponent<GameObject>(), null);

        // 2. mark the game as clicking the ui until mouse releases, so it can disable things like attacking
        Game.Instance.isClickingOnUI = true;
        StartCoroutine(RemoveClickingFlagWhenDone());
    }

    private IEnumerator RemoveClickingFlagWhenDone()
    {
        while (Input.GetMouseButton(0))
        {
            yield return 0;
        }
        Game.Instance.isClickingOnUI = false;
    }
}