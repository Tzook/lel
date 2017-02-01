using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyInfo {

    public string Name;
    public EnemyType Type;

    public EnemyInfo(string name = "Unknown", EnemyType type = EnemyType.Static)
    {
        this.Name = name;
        this.Type = type;
    }

    public enum EnemyType
    {
        Static
    }
}
