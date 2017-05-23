using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressQuestsWindowUI : MonoBehaviour {

	public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
