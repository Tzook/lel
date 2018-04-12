using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyInfo {

    public string ID;
    public string Name;
    public EnemyType Type;

    public float CurrentHealth;
    public float MaxHealth;

    public bool isBoss;

    public EnemyInfo(DevMonsterInfo info, string id = "noID")
    {
        this.ID = id;
        this.Name = info.MonsterName;
        this.Type = info.Type;

        this.CurrentHealth = info.MonsterHP;
        this.MaxHealth = info.MonsterHP;

        this.isBoss = info.isBoss;
    }
}

public enum EnemyType
{
    Static, Moving
}
