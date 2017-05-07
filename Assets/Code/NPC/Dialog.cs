﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string DialogKey;
    public List<DialogPiece> Pieces = new List<DialogPiece>();
}

[System.Serializable]
public class DialogPiece
{
    [@TextAreaAttribute(15, 20)]
    public string Content;

    public float LetterDelay = 0.05f;

    public float Pitch = 1f;
}
