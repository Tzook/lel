using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PerkListGetter
{
    private static PerkListGetter _instance; 
    public static PerkListGetter Instance
    { get { return _instance == null ? _instance = new PerkListGetter() : _instance; } }

    public List<DevPerkMap> Get(JSONNode item)
    {
        List<DevPerkMap> perkMapList = new List<DevPerkMap>();

        IEnumerator enumerator = item["perks"].AsObject.GetEnumerator();
        while (enumerator.MoveNext())
        {
            DevPerkMap perkMap = new DevPerkMap();
            perkMap.Key = ((KeyValuePair<string, JSONNode>)enumerator.Current).Key;
            perkMap.Value = item["perks"][perkMap.Key].AsFloat;
            perkMapList.Add(perkMap);
        }
        perkMapList.Sort(SortByPerkKey);

        return perkMapList;
    }

    public int SortByPerkKey(DevPerkMap perkMap1, DevPerkMap perkMap2)
    {
        return perkMap1.Key.CompareTo(perkMap2.Key);
    }
}