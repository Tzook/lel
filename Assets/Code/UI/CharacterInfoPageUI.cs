using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BestHTTP.SocketIO;
using System;

public class CharacterInfoPageUI : MonoBehaviour {

    [SerializeField]
    protected ActorInstance actorInstance;

    [SerializeField]
    protected Text txtName;

    public void SetInfo(ActorInfo info)
    {
        actorInstance.UpdateVisual(info);
        txtName.text = info.Name;
        LocalUserInfo.Me.SelectedCharacter = info;
    }

}
