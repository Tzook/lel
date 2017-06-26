using System;

public class KeyAction
{
    public string key;
    public Action action;

    public KeyAction(Action action, string key)
    {
        this.action = action;
        this.key = key;
    }
}