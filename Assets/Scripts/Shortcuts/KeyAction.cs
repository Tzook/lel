using System;

public class KeyAction
{
    public Action action;
    public string key;
    public string text;

    public KeyAction(Action action, string key, string text)
    {
        this.action = action;
        this.key = key;
        this.text = text;
    }
}