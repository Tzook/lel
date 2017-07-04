using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour {

    [SerializeField]
    Text txtTitle;

    [SerializeField]
    Text txtDecription;

    [SerializeField]
    Text txtType;

    [SerializeField]
    Image imgIcon;

    [SerializeField]
    Color bonusColor;

    [SerializeField]
    Color requiredColor;

    public void Show(ItemInfo info)
    {
        if(info==null)
        {
            return;
        }

        ClearStats();

        this.gameObject.SetActive(true);
        transform.position = GameCamera.MousePosition;
        txtTitle.text = info.Name;
        txtDecription.text = info.Description;
        txtType.text = info.Type;

        imgIcon.sprite = ResourcesLoader.Instance.GetSprite(info.IconKey);

        if(FollowMouseRoutine!=null)
        {
            StopCoroutine(FollowMouseRoutine);
        }

        SetStats(info.Stats);

        FollowMouseRoutine = StartCoroutine(FollowMouse());
    }

    public void Hide()
    {
        if (FollowMouseRoutine != null)
        {
            StopCoroutine(FollowMouseRoutine);
        }

        FollowMouseRoutine = null;

        this.gameObject.SetActive(false);
    }

    public void SetStats(ItemStats stats)
    {
        GameObject tempObj;
        if(stats.BonusSTR > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("+" + stats.BonusSTR + " Strength", ResourcesLoader.Instance.GetSprite("sword_of_elad"), bonusColor);
        }

        if (stats.BonusMAG > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("+" + stats.BonusMAG + " Magic", ResourcesLoader.Instance.GetSprite("magicicon"), bonusColor);
        }

        if (stats.BonusDEX > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("+" + stats.BonusDEX + " Dextirity", ResourcesLoader.Instance.GetSprite("arrowsicon"), bonusColor);
        }

        if (stats.BonusHP > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("+" + stats.BonusHP + " Max Health", ResourcesLoader.Instance.GetSprite("xIcon"), bonusColor);
        }

        if (stats.BonusMP > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("+" + stats.BonusMP + " Max Mana", ResourcesLoader.Instance.GetSprite("magicalM"), bonusColor);
        }

        if (stats.RequiresSTR > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("Requires " + stats.RequiresSTR + " Strength", ResourcesLoader.Instance.GetSprite("sword_of_elad"), requiredColor);
        }

        if (stats.RequiresMAG > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("Requires " + stats.RequiresMAG + " Magic", ResourcesLoader.Instance.GetSprite("magicicon"), requiredColor);
        }

        if (stats.RequiresDEX > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("Requires " + stats.RequiresDEX + " Dextirity", ResourcesLoader.Instance.GetSprite("arrowsicon"), requiredColor);
        }

        if (stats.RequiresLVL > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("Minimum Level " + stats.RequiresLVL, ResourcesLoader.Instance.GetSprite("fx_hit_small"), requiredColor);
        }


    }

    public void ClearStats()
    {
        while(transform.childCount > 1)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(1).SetParent(transform.parent);
        }
    }

    Coroutine FollowMouseRoutine;
    private IEnumerator FollowMouse()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(GameCamera.MousePosition.x, GameCamera.MousePosition.y, transform.position.z), Time.deltaTime * 5f);
            yield return 0;
        }
    }
}
