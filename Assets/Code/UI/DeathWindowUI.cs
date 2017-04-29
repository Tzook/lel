using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWindowUI : MonoBehaviour {

	public void Respawn()
    {
        SocketClient.Instance.SendReleaseDeath();
        this.gameObject.SetActive(false);
    }
}
