using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyUpdater: MonoBehaviour
{
    public static EnemyUpdater Instance;
    private Dictionary<string, MobMovementData> mobsToUpdate = new Dictionary<string, MobMovementData>();
    private Coroutine updateMobsInstance;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateMob(string mobId, Vector3 position ,float velocity)
    {
        if(!mobsToUpdate.ContainsKey(mobId))
        {
            mobsToUpdate.Add(mobId, new MobMovementData());
        }

        mobsToUpdate[mobId].UpdateData(position, velocity);
        if (updateMobsInstance == null) 
        {
            updateMobsInstance = StartCoroutine(UpdateMobsCoroutine());
        }
    }

    protected IEnumerator UpdateMobsCoroutine()
    {
        // skip current frame - next frame add all mobs
        yield return 0;
        SocketClient.Instance.SendMobsMove(mobsToUpdate);
        mobsToUpdate.Clear();
        updateMobsInstance = null;
    }
}


public class MobMovementData
{
    public Vector3 position;
    public float velocity;

    public void UpdateData(Vector3 position, float velocity)
    {
        this.position = position;
        this.velocity = velocity;
    }


}