using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetStateEntity : MonoBehaviour {

    [SerializeField]
    public string Key;

    [SerializeField]
    public string CurrentState;

    [SerializeField]
    List<NetStateCondition> States = new List<NetStateCondition>();

    public void OnNetStateChange(string givenState)
    {
        CurrentState = givenState;

        Refresh();
    }

    public void SetCurrentState(string state)
    {
        CurrentState = state;
        SocketClient.Instance.SendUpdateRoomState(this.Key, CurrentState);

        Refresh();
    }

    public void Refresh()
    {
        foreach(NetStateCondition state in States)
        {
            state.CheckState(CurrentState);
        }
    }

}

[System.Serializable]
public class NetStateCondition
{
    [SerializeField]
    string ConditionKey;

    [SerializeField]
    UnityEvent Callbacks;

    public void CheckState(string givenState)
    {
        if(givenState == ConditionKey)
        {
            Callbacks.Invoke();
        }
    }
}
