using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningMessageUI : MonoBehaviour {

    #region References

    [SerializeField]
    protected Animator m_Animator;

    [SerializeField]
    protected Text m_MessageText;

    public static WarningMessageUI Instance;
    #endregion

    #region Mono

    void Awake()
    {
        Instance = this;
        Hidden = true;
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Public Methods

    public bool Hidden { protected set; get; }

    public void ShowMessage(string message)
    {
        if (Hidden)
        {
            Show();
        }

        m_MessageText.text = message.ToString();
    }

    public void ShowMessage(string message, float timeDuration)
    {
        this.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(TimedHide(timeDuration));
        ShowMessage(message);
    }


    public void Hide()
    {
        m_Animator.SetTrigger("Hide");
        Hidden = true;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        m_Animator.SetTrigger("Show");
        Hidden = false;
    }

    #endregion

    #region Internal

    public void CanShut()
    {
        this.gameObject.SetActive(false);
    }

    protected IEnumerator TimedHide(float time)
    {
        yield return new WaitForSeconds(time);

        if (!Hidden)
        {
            Hide();
        }
    }

    #endregion

}
