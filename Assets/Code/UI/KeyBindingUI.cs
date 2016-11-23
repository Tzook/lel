using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;

public class KeyBindingUI : MonoBehaviour {

    [SerializeField]
    Transform SpareKeysContainer;

    void Start()
    {
        RefreshState();
    }

    public void RefreshState()
    {
        ClearKeysContainer();
        
        GameObject tempObj;

        for(int i=0;i<InputMap.Map.Keys.Count;i++)
        {
            if (InputMap.Map[InputMap.Map.Keys.ElementAt(i)] == KeyCode.None)
            {
                tempObj = ResourcesLoader.Instance.GetRecycledObject("BindingKey");
                tempObj.transform.SetParent(SpareKeysContainer, false);
                tempObj.transform.GetChild(0).GetComponent<Text>().text = InputMap.Map.Keys.ElementAt(i);
            }
        }

    }

    private void ClearKeysContainer()
    {
        for(int i=0;i< SpareKeysContainer.childCount;i++)
        {
            SpareKeysContainer.GetChild(i).gameObject.SetActive(false);
        }
    }
}
