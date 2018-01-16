using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JumperEntity : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnJump;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<ActorInstance>() != null)
        {
            if(other.GetComponent<ActorInstance>().Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
            {
                StartCoroutine(JumpListener());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<ActorInstance>() != null)
        {
            if (other.GetComponent<ActorInstance>().Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
            {
                StopAllCoroutines();
            }
        }
    }

    IEnumerator JumpListener()
    {
        Coroutine lastRoutineInstance = null;
        while(true)
        {
            if (LocalUserInfo.Me.ClientCharacter.Instance.InputController.JumpRoutineInstance == null 
                && lastRoutineInstance != LocalUserInfo.Me.ClientCharacter.Instance.InputController.JumpRoutineInstance)
            {
                OnJump.Invoke();
            }

            lastRoutineInstance = LocalUserInfo.Me.ClientCharacter.Instance.InputController.JumpRoutineInstance;
            yield return 0;
        }
    }


}
