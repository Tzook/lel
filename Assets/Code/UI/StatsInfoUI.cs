using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatsInfoUI : MonoBehaviour {

    [SerializeField]
    Text txtTitle;

    [SerializeField]
    Text txtDescription;

    [SerializeField]
    Image imgIcon;

    public ContentPiece CurrentPiece;

    public void Show(ContentPiece piece)
    {
        Show(piece.Title, piece.Description, piece.Icon);
        CurrentPiece = piece;
    }

    public void Show(string title = "Title", string desc = "Description", Sprite icon = null)
    {
        this.gameObject.SetActive(true);
        txtTitle.text = title;
        txtDescription.text = desc;
        imgIcon.sprite = icon;

        if (FollowMouseRoutine != null)
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
