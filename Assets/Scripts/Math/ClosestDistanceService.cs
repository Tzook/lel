using System.Collections.Generic;
using UnityEngine;

class ClosestDistanceService 
{
    private static ClosestDistanceService _instance; 
    public static ClosestDistanceService Instance
    { get { return _instance == null ? _instance = new ClosestDistanceService() : _instance; } }

    public string GetClosestActor(Dictionary<string, ActorInfo> actors, Enemy mob)
    {
        Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
        foreach (var actorKeyValuePair in actors)
        {
            if (actorKeyValuePair.Value.Instance.MovementController != null)
            {
                positions.Add(actorKeyValuePair.Key, actorKeyValuePair.Value.Instance.MovementController.transform.position);
            } 
            else if (actorKeyValuePair.Value.Instance.InputController != null)
            {
                positions.Add(actorKeyValuePair.Key, actorKeyValuePair.Value.Instance.InputController.transform.position);
            }
        }

        return GetClosest(positions, mob.transform.position);
    }

    public string GetClosest(Dictionary<string, Vector3> positions, Vector3 target)
    {
        string winnerKey = null;
        float minDist = Mathf.Infinity;
        foreach (var transformKeyValue in positions)
        {
            float dist = Vector3.Distance(transformKeyValue.Value, target);
            if (dist < minDist)
            {
                winnerKey = transformKeyValue.Key;
                minDist = dist;
            }
        }
        return winnerKey;
    }
}