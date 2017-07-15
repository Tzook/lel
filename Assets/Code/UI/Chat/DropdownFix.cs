using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DropdownFix : MonoBehaviour
{    
    public void Start()
    {
        // The default sorting order that unity puts is 30000 for some reason.
        // We want the sorting order to be inherited, thus we remove the override
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = false;
        }
    }
}
