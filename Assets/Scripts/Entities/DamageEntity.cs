using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEntity : MonoBehaviour {

    [SerializeField]
    List<DevPerkMap> Perks = new List<DevPerkMap>();

    [SerializeField]
    bool IgnoreInvincible = false;

	public void Hurt(int Damage)
    {
        if (!LocalUserInfo.Me.ClientCharacter.Instance.InputController.Invincible || IgnoreInvincible)
        {
            SocketClient.Instance.SendWorldDMG(Damage, Perks);
        }
    }
}
