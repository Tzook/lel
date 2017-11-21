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
        Game.Instance.Focus();

        // 2. mark the game as clicking the ui until mouse releases, so it can disable things like attacking
        Game.Instance.isClickingOnUI = true;
        Game.Instance.StartCoroutine(Game.Instance.RemoveClickingFlagWhenDone());
    }
}