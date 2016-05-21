using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharspotUI : MonoBehaviour {

    public ActorInstance Actor;

    [SerializeField]
    protected Text txtName;

    public void Load(ActorInfo info)
    {
        Actor.UpdateVisual(info);
        txtName.text = info.Name;
    }

}
