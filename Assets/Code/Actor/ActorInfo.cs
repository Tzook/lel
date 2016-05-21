using UnityEngine;
using System.Collections;

[System.Serializable]
public class ActorInfo
{
    public string ID;
    public string Name;
    public Gender Gender;
    public ActorInstance Instance;
}

public enum Gender
{
    Male, Female
}
