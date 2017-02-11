using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyInfo {

    public string ID;
    public string Name;
    public EnemyType Type;

    public EnemyInfo(string id = "noID", string name = "Unknown", EnemyType type = EnemyType.Static)
    {
        this.ID = id;
        this.Name = name;
        this.Type = type;
    }

    public enum EnemyType
    {
        Static, Moving
    }
}
