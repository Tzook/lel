using UnityEngine;
using UnityEngine.UI;

public class BugReportWindowUI: MonoBehaviour
{
    [SerializeField]
    InputField inputBody;

    [SerializeField]
    Button btnSubmit;

    [SerializeField]
    GameObject ThanksWindow;

    private bool enabled = false;

    void Awake()
    {
        inputBody.onValueChanged.AddListener(OnInputValueChanged);
        btnSubmit.onClick.AddListener(OnSubmit);
    }

    void OnEnable()
    {
        Game.Instance.InChat = true;
        inputBody.text = "";
        OnInputValueChanged();
    }

    void OnDisable()
    {
        Game.Instance.InChat = false;
    }

    public void OnInputValueChanged(string newValue = "")
    {
        enabled = inputBody.text.Length > 5;
        btnSubmit.GetComponent<CanvasGroup>().alpha = enabled ? 1f : 0.4f;
    }

    public void OnSubmit()
    {
        if (!enabled)
        {
            return;
        }
        gameObject.SetActive(false);
        ThanksWindow.gameObject.SetActive(true);
        BugsReporter.Instance.ReportBug(inputBody.text);
    }
}