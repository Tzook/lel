using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAreaEntity : MonoBehaviour {

    [SerializeField]
    UnityEvent OnEnter;

    [SerializeField]
    UnityEvent OnExit;

    public ActorInstance CurrentPlayer;

    public bool onlyPlayer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CurrentPlayer == null)
        {
            if (collision.GetComponent<ActorInstance>() != null)
            {
                if (onlyPlayer)
                {
                    if(collision.GetComponent<ActorInstance>().Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
                    {
                        CurrentPlayer = collision.gameObject.GetComponent<ActorInstance>();
                        if (CurrentPlayer != null)
                        {
                            OnEnter.Invoke();
                        }
                    }
                }
                else
                {
                    CurrentPlayer = collision.gameObject.GetComponent<ActorInstance>();
                    if (CurrentPlayer != null)
                    {
                        OnEnter.Invoke();
                    }
                }

                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CurrentPlayer != null)
        {
            if (collision.GetComponent<ActorInstance>() != null)
            {
                if (CurrentPlayer == collision.gameObject.GetComponent<ActorInstance>())
                {
                    OnExit.Invoke();

                    CurrentPlayer = null;
                }
            }
        }
    }
}
