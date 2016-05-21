using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ui_transitionFade))]
public class LoadingWindowUI : MonoBehaviour {

    protected List<object> LoadingInstances = new List<object>();

    protected ui_transitionFade m_Fader;

    void Awake()
    {
        SM.LoadingWindow = this;
        this.gameObject.SetActive(false);
        m_Fader = GetComponent<ui_transitionFade>();
    }

    public void Register(object instance)
    {
        if(!LoadingInstances.Contains(instance))
        {
            LoadingInstances.Add(instance);

            if(!this.gameObject.activeInHierarchy)
            {
                m_Fader.CallFadeIn(3f);
            }
        }
    }

    public void Leave(object instance)
    {
        if (LoadingInstances.Contains(instance))
        {
            LoadingInstances.Remove(instance);

            if (LoadingInstances.Count <= 0)
            {
                m_Fader.CallFadeOut(3f);
            }
        }
    }
}
