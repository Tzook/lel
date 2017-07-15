using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DontFocus : MonoBehaviour, IPointerDownHandler 
{
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(Game.Instance.GetComponent<GameObject>(), null);
        Debug.Log("Mouse down.");
    }
}