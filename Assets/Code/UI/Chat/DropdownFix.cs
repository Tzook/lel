using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DropdownFix : MonoBehaviour
{  
    private static bool valueChanged;

    public void Start()
    {
        valueChanged = false;
        // The default sorting order that unity puts is 30000 for some reason.
        // We want the sorting order to be inherited, thus we remove the override
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = false;
        }
    }

    public void ValueChanged()
    {
        valueChanged = true;
    }

    public void OnDestroy()
    {
        if (valueChanged)
        {
            ChatboxUI.Instance.FocusChat(true);
        }
    }
}
