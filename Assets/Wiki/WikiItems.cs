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
        for(int i=0;i<Container.childCount;i++)
        {
            Destroy(Container.GetChild(i).gameObject, 0.1f);
        }

        DevItemInfo tempItem;

        for (int i = 0; i < Content.Instance.Items.Count; i++)
        {
            tempItem = Content.Instance.Items[i];
            if (tempItem.Type == "misc")
            {
                tempItem = Content.Instance.Items[i];
                Content.Instance.Items.RemoveAt(i);
                Content.Instance.Items.Insert(0, tempItem);
            }
        }

        for (int i=0;i<Content.Instance.Items.Count;i++)
        {
            tempItem = Content.Instance.Items[i];
            if (tempItem.Type == "weapon")
            {
                tempItem = Content.Instance.Items[i];
                Content.Instance.Items.RemoveAt(i);
                Content.Instance.Items.Insert(0, tempItem);
            }
        }

        for (int i = 0; i < Content.Instance.Items.Count; i++)
        {
            tempItem = Content.Instance.Items[i];
            if (tempItem.Type == "chest")
            {
                tempItem = Content.Instance.Items[i];
                Content.Instance.Items.RemoveAt(i);
                Content.Instance.Items.Insert(0, tempItem);
            }
        }

        for (int i = 0; i < Content.Instance.Items.Count; i++)
        {
            tempItem = Content.Instance.Items[i];
            if (tempItem.Type == "legs")
            {
                tempItem = Content.Instance.Items[i];
                Content.Instance.Items.RemoveAt(i);
                Content.Instance.Items.Insert(0, tempItem);
            }
        }

        for (int i = 0; i < Content.Instance.Items.Count; i++)
        {
            tempItem = Content.Instance.Items[i];
            if (tempItem.Type == "gloves")
            {
                tempItem = Content.Instance.Items[i];
                Content.Instance.Items.RemoveAt(i);
                Content.Instance.Items.Insert(0, tempItem);
            }
        }

        for (int i = 0; i < Content.Instance.Items.Count; i++)
        {
            tempItem = Content.Instance.Items[i];
            if (tempItem.Type == "shoes")
            {
                tempItem = Content.Instance.Items[i];
                Content.Instance.Items.RemoveAt(i);
                Content.Instance.Items.Insert(0, tempItem);
            }
        }

        for (int i = 0; i < Content.Instance.Items.Count; i++)
        {
            tempItem = Content.Instance.Items[i];
            if (tempItem.Type == "head")
            {
                tempItem = Content.Instance.Items[i];
                Content.Instance.Items.RemoveAt(i);
                Content.Instance.Items.Insert(0, tempItem);
            }
        }

        GameObject tempObj;
        for(int i=0;i<Content.Instance.Items.Count;i++)
        {
            tempObj = (GameObject)Instantiate(ItemObject);
            tempObj.transform.SetParent(Container, false);

            tempObj.transform.GetChild(0).GetComponent<Text>().text = Content.Instance.Items[i].Name;
            tempObj.transform.GetChild(1).GetComponent<Image>().sprite = Content.Instance.Items[i].IconPlaceable;
        }

    }
}
