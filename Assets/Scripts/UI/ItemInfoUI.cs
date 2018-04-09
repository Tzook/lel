using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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

    [SerializeField]
    bool alignToLeft = false;

    public string currentItemKey;

    Vector3 objectWorldWidth;

    public void Show(ItemInfo info)
    {
        if(info==null)
        {
            return;
        }

        currentItemKey = info.Key;

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
            transform.position = new Vector3(GameCamera.MousePosition.x, GameCamera.MousePosition.y, transform.position.z) - GetSizeToPushLeft();
        }

        txtTitle.text = info.Name;
        txtDecription.text = info.Description;
        txtType.text = info.Type;

        imgIcon.sprite = ResourcesLoader.Instance.GetSprite(info.IconKey);

        if(FollowMouseRoutine!=null)
        {
            StopCoroutine(FollowMouseRoutine);
        }

        SetStats(info.Stats, info.Perks);

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

    public void SetStats(ItemStats stats, List<DevPerkMap> Perks)
    {
        ItemStatUI statsInfoObject;

        if (stats.JumpBonus > 0)
        {
            statsInfoObject = GetStatsInfoObject();
            statsInfoObject.SetInfo("+" + stats.JumpBonus + " Jumping Bonus", ResourcesLoader.Instance.GetSprite("fx_hit01"), bonusColor);
        }

        if (stats.SpeedBonus > 0)
        {
            statsInfoObject = GetStatsInfoObject();
            statsInfoObject.SetInfo("+" + stats.SpeedBonus + " Speed Bonus", ResourcesLoader.Instance.GetSprite("fx_hit01"), bonusColor);
        }

        if (stats.RequiresLVL > 0)
        {
            statsInfoObject = GetStatsInfoObject();
            statsInfoObject.transform.localScale = Vector3.one;
            Color minLevelColor = stats.RequiresLVL > LocalUserInfo.Me.ClientCharacter.LVL ? requiredColor : bonusColor;
            statsInfoObject.SetInfo("Minimum Level " + stats.RequiresLVL, ResourcesLoader.Instance.GetSprite("fx_hit_small"), minLevelColor);
        }

        foreach (DevPerkMap perkMap in Perks)
        {
            statsInfoObject = GetStatsInfoObject();
            DevPAPerk perkRef = Content.Instance.GetPerk(perkMap.Key);

            string perkText = PerkValueFormatter.Instance.GetFormattedValue(perkRef, perkMap.Value, true) + " " + perkRef.Name;
            bool isGoodPerk = perkRef.PrecentPerUpgrade > 0 ? perkMap.Value > 0 : perkMap.Value < 0;
            Color color = isGoodPerk ? bonusColor : requiredColor;
            statsInfoObject.SetInfo(perkText, perkRef.Icon, color);
        }
    }

    private ItemStatUI GetStatsInfoObject()
    {
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("StatInfo");
        tempObj.transform.SetParent(transform, false);
        tempObj.transform.localScale = Vector3.one;
        tempObj.transform.SetAsLastSibling();
        return tempObj.GetComponent<ItemStatUI>();
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
            transform.position = Vector3.Lerp(transform.position, new Vector3(GameCamera.MousePosition.x, GameCamera.MousePosition.y, transform.position.z) - GetSizeToPushLeft(), Time.deltaTime * 5f);
            yield return 0;
        }
    }

    protected Vector3 GetSizeToPushLeft()
    {
        if (!alignToLeft)
        {
            return Vector3.zero;
        }
        if (objectWorldWidth == null || objectWorldWidth.Equals(Vector3.zero))
        {
            Vector3[] corners = new Vector3[4];
            ((RectTransform)transform).GetWorldCorners(corners);
            objectWorldWidth = corners[2] - corners[1];
        }
        return objectWorldWidth;
    }
}
