using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageEntity : MonoBehaviour {

    [SerializeField]
    List<DevPerkMap> Perks = new List<DevPerkMap>();

    [SerializeField]
    bool IgnoreInvincible = false;

    [SerializeField]
    UnityEvent OnHurt;

	public void Hurt(int Damage)
    {
        if (!LocalUserInfo.Me.ClientCharacter.Instance.InputController.Invincible || IgnoreInvincible)
        {
            SocketClient.Instance.SendWorldDMG(Damage, Perks);
            OnHurt.Invoke();
        }
    }
}
