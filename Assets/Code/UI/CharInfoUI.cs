using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharInfoUI : MonoBehaviour {

    [SerializeField]
    Text m_txtName;

    [SerializeField]
    Text m_txtGender;

    [SerializeField]
    Transform CharSpot;

    ActorInstance CharInstance;

    public void Open(ActorInfo Info)
    {
        this.gameObject.SetActive(true);


        StartCoroutine(OpenRoutine(Info));

    }

    private IEnumerator OpenRoutine(ActorInfo Info)
    {
        m_txtName.text = Info.Name;
        m_txtGender.text = "Gender: " + Info.Gender.ToString();

        if (CharSpot.childCount > 0)
        {
            Destroy(CharSpot.GetChild(0).gameObject);
        }

        yield return 0;
        
        if (Info.Gender == Gender.Male)
        {
            Instantiate(ResourcesLoader.Instance.GetObject("actor_male")).transform.SetParent(CharSpot);
        }
        else
        {
            Instantiate(ResourcesLoader.Instance.GetObject("actor_female")).transform.SetParent(CharSpot);
        }

        CharSpot.GetChild(0).position = CharSpot.position;
        CharSpot.GetChild(0).transform.localScale = Vector3.one;
        CharInstance = CharSpot.GetChild(0).GetComponent<ActorInstance>();

        CharInstance.Info = Info;
        CharInstance.nameHidden = true;

        CharInstance.SetElementsUILayer();
        CharInstance.UpdateVisual();
    }

}
