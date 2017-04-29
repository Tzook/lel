using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldDropWindowUI : MonoBehaviour {

    [SerializeField]
    InputField m_InputField;

    public void DropGold()
    {
        int GoldValue = 0;
        if(int.TryParse(m_InputField.text, out GoldValue))
        {
            if (GoldValue <= Game.Instance.CurrentScene.ClientCharacter.Gold)
            {
                InGameMainMenuUI.Instance.MinilogMessage("Dropped " + GoldValue + " Gold");

                SocketClient.Instance.SendDroppedGold(GoldValue);
                this.gameObject.SetActive(false);
            }
            else
            {
                ShockMessageUI.Instance.CallMessage("Don't have enough gold!");
            }
        }
        else
        {
            ShockMessageUI.Instance.CallMessage("Please type a number in the field.");
        }
    }
}
