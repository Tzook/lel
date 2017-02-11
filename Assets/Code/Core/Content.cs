using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Content : MonoBehaviour {

    public static Content Instance;

    void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    List<ContentPiece> InfoBank = new List<ContentPiece>();

    public ContentPiece GetInfo(string Key)
    {
        for(int i=0;i<InfoBank.Count;i++)
        {
            if(InfoBank[i].Title == Key)
            {
                return InfoBank[i];
            }
        }

        return null;
    }
}

[System.Serializable]
public class ContentPiece
{
    public string Title;

    [TextArea(16, 9)]
    public string Description;

    public Sprite Icon;
}
