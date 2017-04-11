using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevContent : MonoBehaviour {

    public List<DevMonster> Monsters = new List<DevMonster>();
}

[System.Serializable]
public class DevMonster
{
    [SerializeField]
    public string MonsterKey;

    [SerializeField]
    public string MonsterName;

    [SerializeField]
    public int MonsterHP;

    [SerializeField]
    public int MonsterLevel;

    [SerializeField]
    public int MinDMG;

    [SerializeField]
    public int MaxDMG;
}
