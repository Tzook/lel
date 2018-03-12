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

    [SerializeField]
    bool isFollowingMouse = true;

    public void Show(ItemInfo info)
    {
        if(info==null)
        {
            return;
        }

        ClearStats();

        if(LocalUserInfo.Me.ClientCharacter.CanEquipItem(info))
        {
            txtTitle.color = Color.white;
            txtDecription.color = Color.white;
            txtType.color = Color.white;
            imgIcon.color = Color.white;
        }
        else
        {
            txtTitle.color = Color.red;
            txtDecription.color = Color.red;
            txtType.color = Color.red;
            imgIcon.color = Color.red;
        }

        this.gameObject.SetActive(true);

        if (isFollowingMouse)
        {
            transform.position = new Vector3(GameCamera.MousePosition.x, GameCamera.MousePosition.y, transform.position.z);
        }

        txtTitle.text = info.Name;
        txtDecription.text = info.Description;
        txtType.text = info.Type;

        imgIcon.sprite = ResourcesLoader.Instance.GetSprite(info.IconKey);

        if(FollowMouseRoutine!=null)
        {
            StopCoroutine(FollowMouseRoutine);
        }

        SetStats(info.Stats);

        if (isFollowingMouse)
        {
            FollowMouseRoutine = StartCoroutine(FollowMouse());
        }
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

        if (stats.JumpBonus > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("+" + stats.JumpBonus + " Jumping Bonus", ResourcesLoader.Instance.GetSprite("fx_hit01"), bonusColor);
        }

        if (stats.SpeedBonus > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.GetComponent<ItemStatUI>().SetInfo("+" + stats.SpeedBonus + " Speed Bonus", ResourcesLoader.Instance.GetSprite("fx_hit01"), bonusColor);
        }

        if (stats.RequiresLVL > 0)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
            tempObj.transform.SetParent(transform, false);
            tempObj.transform.SetAsLastSibling();
            tempObj.transform.localScale = Vector3.one;
            Color minLevelColor = stats.RequiresLVL > LocalUserInfo.Me.ClientCharacter.LVL ? requiredColor : bonusColor;
            tempObj.GetComponent<ItemStatUI>().SetInfo("Minimum Level " + stats.RequiresLVL, ResourcesLoader.Instance.GetSprite("fx_hit_small"), minLevelColor);
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
