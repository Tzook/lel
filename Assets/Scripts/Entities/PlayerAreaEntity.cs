using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAreaEntity : MonoBehaviour {

    [SerializeField]
    UnityEvent OnEnter;

    [SerializeField]
    UnityEvent OnExit;

    ActorInstance CurrentPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CurrentPlayer == null)
        {
            CurrentPlayer = collision.gameObject.GetComponent<ActorInstance>();

            if(CurrentPlayer != null)
            {
                OnEnter.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CurrentPlayer != null)
        {
            if (CurrentPlayer == collision.gameObject.GetComponent<ActorInstance>())
            {
                OnExit.Invoke();

                CurrentPlayer = null;
            }
        }
    }
}
