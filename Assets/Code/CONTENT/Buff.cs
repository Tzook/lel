using UnityEngine;

public class Buff {

    public string Key;
    public string TargetID;
    public float Duration;
    public Coroutine RunningRoutine;
    public GameObject EffectPrefab;

    public Buff(string key, string targetID, float duration)
    {
        this.Key = key;
        this.TargetID = targetID;
        this.Duration = duration;
    }

}
