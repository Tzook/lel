using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string DialogKey;
    public List<DialogPiece> Pieces = new List<DialogPiece>();
    public List<DialogOption> Options = new List<DialogOption>();
}

[System.Serializable]
public class DialogPiece
{
    [@TextAreaAttribute(15, 20)]
    public string Content;

    public float LetterDelay = 0.05f;

    public float Pitch = 1f;

    public string AnimationTrigger = "";
}

[System.Serializable]
public class DialogOption
{
    public string Title;
    public string Event;
    public string Value;
}
