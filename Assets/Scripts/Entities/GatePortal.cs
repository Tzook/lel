using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatePortal : MonoBehaviour {

    public string Key;
    public string TargetPortal;
    public string TargetLevel;

    [SerializeField]
    bool StartShut = false;

    public List<string> RequiresItems = new List<string>();

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

        if(StartShut)
        {
            this.gameObject.SetActive(false);
        }
    }
}
