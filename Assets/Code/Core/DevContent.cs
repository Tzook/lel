using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevContent : MonoBehaviour {

    public List<MonsterInfo> Monsters = new List<MonsterInfo>();

    void Start()
    {
        Content.Instance.Monsters.InsertRange(0, Monsters);
    }
}

[System.Serializable]
public class MonsterInfo
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
