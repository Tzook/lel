using UnityEngine;
using System.Collections;

public class GatePortal : MonoBehaviour {

    public string TargetLevel;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        while(Game.Instance.CurrentScene==null)
        {
            yield return 0;
        }

        Game.Instance.CurrentScene.AddScenePortal(this); 
    }
}
