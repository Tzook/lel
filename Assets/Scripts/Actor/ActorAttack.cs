using System.Collections;
using UnityEngine;

public class ActorAttack : MonoBehaviour
{
    public bool InSecondaryMode = false;
    protected Coroutine SecondaryModeInstance;
    
    public void EnsureInSecondaryMode()
    {
        if (SecondaryModeInstance == null)
        {
            SecondaryModeInstance = StartCoroutine(StartSecondaryModeRoutine());
        }
    }


    protected IEnumerator StartSecondaryModeRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        InSecondaryMode = true;
        if (LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.Key == "melee")
        {
            LocalUserInfo.Me.ClientCharacter.Instance.ToggleSecondaryAttackAnimation(true);
            SocketClient.Instance.SendStartedSecondaryMode();
        }
    }

    public void EnsureStopSecondaryMode()
    {
        if (SecondaryModeInstance != null)
        {
            StopCoroutine(SecondaryModeInstance);
            SecondaryModeInstance = null;
            InSecondaryMode = false;
            if (LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.Key == "melee")
            {
                LocalUserInfo.Me.ClientCharacter.Instance.ToggleSecondaryAttackAnimation(false);
                SocketClient.Instance.SendEndedSecondaryMode();
            }
        }
    }
}