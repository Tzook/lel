using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterAreaEntity : MonoBehaviour {

    [SerializeField]
    UnityEvent OnEnter;

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<ActorInstance>() != null)
        {
            if (other.GetComponent<ActorInstance>().Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
            {
                OnEnter.Invoke();
            }
        }
    }
}
