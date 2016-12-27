using UnityEngine;
using System.Collections;

public class DontDestroyMeOnLoad : MonoBehaviour {

	void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Game.Instance.DontDestroyMeOnLoadList.Add(this.gameObject);
    }
}
