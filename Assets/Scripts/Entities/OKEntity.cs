using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OKEntity : MonoBehaviour {

    [SerializeField]
    string QuestKey;

    [SerializeField]
    string OKKey;

    public void Add(int Value)
    {
        Set(LocalUserInfo.Me.ClientCharacter.GetQuest(QuestKey).GetConditionByType(OKKey).CurrentProgress + Value);
    }

    public void Set(int Value)
    {
        SocketClient.Instance.SendQuestOK(OKKey, Value);
    }
}
