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
        Game.Instance.Focus();
    }

    public void Update()
    {
        // the dropdown item has focus by default. We have to disable it
        if (inDropdown)
        {
            Game.Instance.Focus();
        }
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
