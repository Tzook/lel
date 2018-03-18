using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportEntity : MonoBehaviour {
    
    public void Activate()
    {
        LocalUserInfo.Me.ClientCharacter.Instance.transform.position = transform.position;
    }
}
