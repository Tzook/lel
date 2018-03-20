using UnityEngine;
using UnityEngine.UI;

public class BugReportWindowUI: MonoBehaviour
{
    [SerializeField]
    InputField inputTitle;

    [SerializeField]
    InputField inputBody;

    [SerializeField]
    Button btnSubmit;

    [SerializeField]
    GameObject ThanksWindow;

    private bool enabled = false;

    void Awake()
    {
        inputTitle.onValueChanged.AddListener(OnInputValueChanged);
        inputBody.onValueChanged.AddListener(OnInputValueChanged);
        btnSubmit.onClick.AddListener(OnSubmit);
    }

    void OnEnable()
    {
        Game.Instance.InChat = true;
        inputTitle.text = "";
        inputBody.text = "";
        OnInputValueChanged();
    }

    void OnDisable()
    {
        Game.Instance.InChat = false;
    }

    public void OnInputValueChanged(string newValue = "")
    {
        enabled = inputTitle.text.Length > 2 && inputBody.text.Length > 2;
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
        BugsReporter.Instance.ReportBug(inputTitle.text, inputBody.text);
    }
}