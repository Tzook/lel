using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class ActorInfo
{
    public string ID;
    public string Name;
    public Gender Gender;
    public ActorInstance Instance;
    public string CurrentRoom;
    public Vector3 LastPosition;
    public string Eyes = "eyes_0a";
    public string Nose = "nose_0";
    public string Mouth= "mouth_0";

    public ActorInfo()
    {
    }

    public ActorInfo(JSONNode node)
    {
        this.ID = node["_id"].Value;
        this.Name = node["name"].Value;
        this.CurrentRoom = node["room"].Value;
        this.LastPosition = new Vector3(node["position"]["x"].AsFloat, node["position"]["y"].AsFloat, node["position"]["z"].AsFloat);

        if (node["looks"]["g"].AsBool)
        {
            this.Gender = Gender.Male;
        }
        else
        {
            this.Gender = Gender.Female;
        }

        Eyes = node["looks"]["eyes"].Value;
        Nose = node["looks"]["nose"].Value;
        Mouth= node["looks"]["mouth"].Value;
    }
}

public enum Gender
{
    Male, Female
}
