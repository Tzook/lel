using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasteryUpgradeWindowUI : MonoBehaviour {

    [SerializeField]
    Text m_txtTitle;

    [SerializeField]
    CanvasGroup m_CG;

    [SerializeField]
    Transform Container;

    [SerializeField]
    List<GameObject> UpgradeBubbles = new List<GameObject>();

    public Ability CurrentAbility = null;

    public void ShowLatest(List<Ability> abilities)
    {
        this.gameObject.SetActive(true);

        for(int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].PerkPool.Count > 0 && abilities[i].Points > 0)
            {
                CurrentAbility = abilities[i];
                break;
            }
        }

        if (CurrentAbility == null)
        {
            Hide();
            return;
        }

        StartCoroutine(ShowRoutine(CurrentAbility));
    }

    IEnumerator ShowRoutine(Ability ability)
    {
        Clear();

        m_CG.alpha = 0f;

        while (m_CG.alpha < 1f)
        {
            m_CG.alpha += 1f * Time.deltaTime;
            yield return 0;
        }

        GameObject tempObj;
        for(int i=0;i<ability.PerkPool.Count;i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("TargetPoint");
            tempObj.transform.SetParent(Container, false);
        }

        for(int i=0;i<ability.PerkPool.Count;i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("Crate");
            tempObj.transform.SetParent(transform, false);
            tempObj.GetComponent<CrateUI>().Set(ability.PerkPool[i], ability.GetPerk(ability.PerkPool[i]));
            tempObj.GetComponent<CrateUI>().Interactable = true;

            UpgradeBubbles.Add(tempObj);

            yield return StartCoroutine(SlidePerkCrate(tempObj, Container.GetChild(i)));
        }

        for(int i=0;i<UpgradeBubbles.Count;i++)
        {
            UpgradeBubbles[i].GetComponent<CrateUI>().Unpack();

            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator SlidePerkCrate(GameObject crateObj, Transform targetTransfrom)
    {
        Vector3 RandomStartingPoint = GameCamera.Instance.Cam.ScreenToWorldPoint(new Vector2(Random.Range(0, Screen.width), -1f));
        RandomStartingPoint = new Vector3(RandomStartingPoint.x, RandomStartingPoint.y, 0f);
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("Crate");
        tempObj.transform.position = RandomStartingPoint;
        UpgradeBubbles.Add(tempObj);

        float t = 0f;
        while(t<1f)
        {
            t += 1.75f * Time.deltaTime;

            crateObj.transform.position = Game.SplineLerp(RandomStartingPoint, targetTransfrom.position, 3f, t);

            yield return 0;
        }

        AudioControl.Instance.Play("sound_reward3");
    }

    public void Clear()
    {
        foreach (GameObject obj in UpgradeBubbles)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        UpgradeBubbles.Clear();

        while (Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void DisableButtons()
    {
        for(int i=0;i<UpgradeBubbles.Count;i++)
        {
            UpgradeBubbles[i].GetComponent<CrateUI>().Interactable = false;
        }
    }
}
