using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentsWindowUI : MonoBehaviour {

    [SerializeField]
    GameObject WarriorPanel;

	public void Show()
    {
        this.gameObject.SetActive(true);

        switch(LocalUserInfo.Me.ClientCharacter.Class)
        {
            case "warrior":
                {
                    WarriorPanel.gameObject.SetActive(true);
                    break;
                }
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);

        switch (LocalUserInfo.Me.ClientCharacter.Class)
        {
            case "warrior":
                {
                    WarriorPanel.gameObject.SetActive(false);
                    break;
                }
        }
    }
}
