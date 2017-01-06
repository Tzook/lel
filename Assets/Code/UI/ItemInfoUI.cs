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

    public void Show(ItemInfo info)
    {
        if(info==null)
        {
            return;
        }

        this.gameObject.SetActive(true);
        txtTitle.text = info.Name;
        txtDecription.text = info.Description;
        txtType.text = info.Type;

        imgIcon.sprite = ResourcesLoader.Instance.GetSprite(info.IconKey);

        if(FollowMouseRoutine!=null)
        {
            StopCoroutine(FollowMouseRoutine);
        }

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

    Coroutine FollowMouseRoutine;
    private IEnumerator FollowMouse()
    {
        while (true)
        {
            transform.position = new Vector3(GameCamera.MousePosition.x, GameCamera.MousePosition.y, transform.position.z);
            yield return 0;
        }
    }
}
