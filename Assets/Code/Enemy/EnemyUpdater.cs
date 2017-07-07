using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyUpdater: MonoBehaviour
{
    public static EnemyUpdater Instance;
    private Dictionary<string, Vector3> mobsToUpdate = new Dictionary<string, Vector3>();
    private Coroutine updateMobsInstance;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateMob(string mobId, Vector3 position)
    {
        mobsToUpdate[mobId] = position;
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