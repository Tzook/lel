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

    public Vector2 getPercentLocation(Vector2 position)
    {
        Vector2 percentPosition = new Vector2();
        
        percentPosition.x = (position.x - left) / (right - left);
        percentPosition.y = -(position.y - top) / (bottom - top);

        return percentPosition;
    }
}

// We can't have List<List<T>> in the editor so we create an object to wrap the coliders
[System.Serializable]
public class ColiderGroup
{
    public List<Vector2> coliders = new List<Vector2>();
}