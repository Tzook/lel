using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InputMap : MonoBehaviour {

    [SerializeField]
    List<InitInputObject> InitList = new List<InitInputObject>();
     
    public static Dictionary<string, KeyCode> Map = new Dictionary<string, KeyCode>();

    void Awake()
    {
        Map.Clear();
        Initialize();
        LoadMap();
    }

    private void Initialize()
    {
        for (int i = 0; i < InitList.Count; i++)
        {
            Map.Add(InitList[i].Name, InitList[i].KeyCode);
        }
    }

    public static void LoadMap()
    {
        int i = 0;
        while (PlayerPrefs.HasKey("inputMapKey_" + i))
        {
            if (Map.ContainsKey(PlayerPrefs.GetString("inputMapKey_" + i)))
            {
                Map[PlayerPrefs.GetString("inputMapKey_" + i)] = (KeyCode)PlayerPrefs.GetInt("inputMapValue_" + i);
            }
            else
            {
                Map.Add(PlayerPrefs.GetString("inputMapKey_" + i), (KeyCode)PlayerPrefs.GetInt("inputMapValue_" + i));
            }

            i++;
        }
    }

    public static void SaveMap()
    {
        for (int i = 0; i < Map.Keys.Count; i++)
        {
            PlayerPrefs.SetString("inputMapKey_" + i, Map.Keys.ElementAt(i));
            PlayerPrefs.SetInt("inputMapValue_" + i, (int) Map[Map.Keys.ElementAt(i)]);
        }

        PlayerPrefs.Save();
    }

}

[System.Serializable]
public class InitInputObject
{
    public string Name;
    public KeyCode KeyCode;
}
