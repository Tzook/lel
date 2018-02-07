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

    public string SoundKey;

    public string AnimationTrigger = "";

    [SerializeField]
    public string PostPieceEvent;

    [SerializeField]
    public string PostPieceEventValue;

}

[System.Serializable]
public class DialogOption
{
    public string Title;
    public string Event;
    public string Value;
    public string Condition;
    public string ConditionValue;
    public Color CLR = Color.white;
}
