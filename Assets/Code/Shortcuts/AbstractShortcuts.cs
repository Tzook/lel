using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractShortcuts: MonoBehaviour
{
    private const float VERTICAL_SPACE = 0.6f;
    private int index = 0;

    public abstract string GetActionsFirstKey();
    public abstract List<KeyAction> GetActions();

    public void ResetPosition()
    {
        this.index = 0;
    }
    public Vector3 GetPosition()
    {
        return new Vector3(transform.position.x, transform.position.y - VERTICAL_SPACE * this.index++);
    }
}
