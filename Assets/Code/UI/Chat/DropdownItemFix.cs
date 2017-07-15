using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DropdownItemFix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    private bool inDropdown = false;
    
    public void Start()
    {
        Blur();
    }

    public void Update()
    {
        // the dropdown item has focus by default. We have to disable it
        if (inDropdown)
        {
            Blur();
        }
    }

    private void Blur()
    {
        EventSystem.current.SetSelectedGameObject(Game.Instance.GetComponent<GameObject>(), null);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        inDropdown = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        inDropdown = false;
    }
}
