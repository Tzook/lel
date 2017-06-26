using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractShortcuts: MonoBehaviour
{
    public abstract string GetActionsFirstKey();
    public abstract List<KeyAction> GetActions();
}
