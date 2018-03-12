using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WikiItems : MonoBehaviour {

    [SerializeField]
    Transform Container;

    [SerializeField]
    GameObject ItemObject;

    private void OnEnable()
    {
        ShowType("chest");
    }

    public void ShowType(string type)
    {
        for (int i = 0; i < Container.childCount; i++)
        {
            Destroy(Container.GetChild(i).gameObject, 0.1f);
        }

        Content.Instance.Items.Sort((x, y) => x.Stats.RequiresLVL.CompareTo(y.Stats.RequiresLVL));

        GameObject tempObj;
        for (int i = 0; i < Content.Instance.Items.Count; i++)
        {
            if (Content.Instance.Items[i].Type == type)
            {
                tempObj = (GameObject)Instantiate(ItemObject);
                tempObj.transform.SetParent(Container, false);

                tempObj.transform.GetChild(0).GetComponent<Text>().text = Content.Instance.Items[i].Name;
                tempObj.transform.GetChild(1).GetComponent<Image>().sprite = Content.Instance.Items[i].IconPlaceable;
                tempObj.transform.GetChild(5).GetComponent<Text>().text = Content.Instance.Items[i].Stats.RequiresLVL.ToString();
            }
        }

    }
}
