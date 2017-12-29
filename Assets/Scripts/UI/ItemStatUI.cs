using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatUI : MonoBehaviour {

    [SerializeField]
    Image IconImageA;
    [SerializeField]
    Image IconImageB;

    [SerializeField]
    Text ContentText;


    public void SetInfo(string text, Sprite icon, Color clr)
    {
        ContentText.text = text;
        IconImageA.sprite = icon;
        IconImageB.sprite = icon;

        GetComponent<Image>().color = clr;
    }
}
