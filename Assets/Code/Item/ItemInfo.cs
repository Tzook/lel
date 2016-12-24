using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class ItemInfo {

    public string Name;
    public string IconKey;
    public string Description;

    public ItemInfo(JSONNode itemNode)
    {
        this.Name = itemNode["name"].Value;
        this.IconKey = itemNode["icon"].Value;
    }

}
