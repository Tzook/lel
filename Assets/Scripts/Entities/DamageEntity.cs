using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEntity : MonoBehaviour {

    [SerializeField]
    List<DevPerkMap> Perks = new List<DevPerkMap>();

	public void Hurt(int Damage)
    {
        SocketClient.Instance.SendWorldDMG(Damage, Perks);
    }
}
