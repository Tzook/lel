using UnityEngine;
using System.Collections;

public class DontDestroyMeOnLoad : MonoBehaviour {

	void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        Game.Instance.UndestroyedObjects.Add(this);
    }
}
