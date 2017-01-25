using UnityEngine;
using System.Collections;
using System;

public class StatsWindowUI : MonoBehaviour
{
    internal void Show(ActorInfo info)
    {
        this.gameObject.SetActive(true);
    }

    internal void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
