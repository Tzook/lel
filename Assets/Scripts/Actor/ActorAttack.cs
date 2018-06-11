using System.Collections;
using UnityEngine;

public class ActorAttack : MonoBehaviour
{
    public bool InSecondaryMode = false;
    protected Coroutine SecondaryModeInstance;
    
    public void UpdateSecondaryMode()
    {
        if (SecondaryModeInstance == null)
        {
            if (Input.GetMouseButton(1))
            {
                SecondaryModeInstance = StartCoroutine(SecondaryModeRoutine());
            }
        }
        else
        {
            LocalUserInfo.Me.ClientCharacter.Instance.InputController.Aim();
            if (!Input.GetMouseButton(1))
            {
                LocalUserInfo.Me.ClientCharacter.Instance.InputController.StopAim();
                StopSecondaryMode();
            }
        }
    }

    public void StopSecondaryMode()
    {
        if (SecondaryModeInstance != null)
        {
            StopCoroutine(SecondaryModeInstance);
            SecondaryModeInstance = null;
            InSecondaryMode = false;
        }
    }

    protected IEnumerator SecondaryModeRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        InSecondaryMode = true;
    }
}