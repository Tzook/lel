using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeadEntity : MonoBehaviour {

    ActorInstance CurrentActor;


    public bool Invoked = false;

    [SerializeField]
    UnityEvent OnDeath;

    private void OnTriggerEnter2D(Collider2D col)
    {
        ActorInstance tempInstnace = col.gameObject.GetComponent<ActorInstance>();
        if (tempInstnace != null && tempInstnace.Info.ID != LocalUserInfo.Me.ClientCharacter.ID)
        {
            CurrentActor = tempInstnace;
        }
    }

    private void Update()
    {
        if(CurrentActor != null && CurrentActor.isDead && !Invoked)
        {
            Invoked = true;

            OnDeath.Invoke();
        }
    }

    public void ResetInvokedFlag()
    {
        Invoked = false;
    }
}
