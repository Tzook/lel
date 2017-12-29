using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharspotUI : MonoBehaviour {

    public Transform actorSpot;

    [SerializeField]
    protected Text txtName;

    [SerializeField]
    protected Text txtInfo;

    ActorInstance Actor;

    public void Load(ActorInfo info)
    {
        if(Actor!=null)
        {
            Actor.gameObject.SetActive(false);
        }

        GameObject tempObj;
        if (info.Gender == Gender.Male)
        {
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_male"));
        }
        else
        {
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_female"));
        }

        tempObj.transform.SetParent(transform);
        tempObj.transform.position = actorSpot.position;
        tempObj.transform.localScale = actorSpot.localScale;

        Actor = tempObj.GetComponent<ActorInstance>();

        Actor.nameHidden = true;
        Actor.UpdateVisual(info);
        txtName.text = info.Name;

        txtInfo.text = "LVL " + info.LVL + " " + info.Class;
    }

}
