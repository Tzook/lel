using UnityEngine;
using System.Collections;

public class DontDestroyMeOnLoad : MonoBehaviour {

	void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Game.Instance.DontDestroyMeOnLoadList.Add(this.gameObject);
    }
}
