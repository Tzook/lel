using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterInfoPageUI : MonoBehaviour {

    [SerializeField]
    protected ActorInstance ActorInstance;

    [SerializeField]
    protected Text txtName;

    public void SetInfo(ActorInfo info)
    {
        ActorInstance.UpdateVisual(info);
        txtName.text = info.Name;
    }
}
