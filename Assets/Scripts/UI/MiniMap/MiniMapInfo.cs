using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MiniMapInfo
{
    public float left;
    public float right;
    public float top;
    public float bottom;
    public List<ColiderGroup> coliders = new List<ColiderGroup>();
}

// We can't have List<List<T>> in the editor so we create an object to wrap the coliders
[System.Serializable]
public class ColiderGroup
{
    public List<Vector2> coliders = new List<Vector2>();
}