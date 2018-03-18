using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;
using System;

[RequireComponent(typeof(ui_pageMenu))]
public class MainMenuUI : MonoBehaviour
{

    #region References

    [SerializeField]
    protected InputField m_UsernameField;
    [SerializeField]
    protected InputField m_PasswordField;
    [SerializeField]
    protected InputField m_RUsernameField;
    [SerializeField]
    protected InputField m_RPasswordField;
    [SerializeField]
    protected InputField m_RPasswordConfirmField;
    [SerializeField]
    protected GameObject m_RegisterationPanel;
    [SerializeField]
    protected GameObject m_LoginPanel;

    [SerializeField]
    protected CreateCharacterUI m_CreateCharacterUI;

    [SerializeField]
    protected CharacterSelectionPageUI m_CharacterSelectionUI;

    protected LoginHandler m_LoginHandler;
    protected RegisterHandler m_RegisterHandler;
    protected LogoutHandler m_LogoutHandler;
    protected SessionHandler m_SessionHandler;
    protected ui_pageMenu m_PageMenu;

    #endregion

    public const string LAST_LOGGED_IN_CHAR_ID_PREF_KEY = "LastLoggedInCharId";
    protected string LastLoggedInCharId;

    #region Mono

    void Awake()
    {
        m_PageMenu = GetComponent<ui_pageMenu>();
        LastLoggedInCharId = PlayerPrefs.GetString(LAST_LOGGED_IN_CHAR_ID_PREF_KEY);
    }

    void Start()
    {
        m_RegisterationPanel.SetActive(false);
        m_LoginHandler = new LoginHandler(LoginResponse);
        m_RegisterHandler = new RegisterHandler(RegisterationResponse);
        m_SessionHandler = new SessionHandler(SessionResponse);
        m_LogoutHandler = new LogoutHandler(LogoutResponse);

        if (CookiesManager.Instance.HasCookie(CookiesManager.UNICORN))
        {
            LoadingWindowUI.Instance.Register(this);
            m_SessionHandler.Session();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            switch (m_PageMenu.m_iIndex)
            {
                case 0:
                    if (m_RegisterationPanel.activeInHierarchy)
                    {
                        AttemptRegister();
                    }
                    else
                    {
                        AttemptLogin();
                    }
                    break;
                case 1:
                    // TODO
                    break;
                case 2:
                    m_CreateCharacterUI.AttemptCreateCharacter();
                    break;
            }
        }
    }

    #endregion

    #region Public Methods

    public void AttemptLogin()
    {
        string username = m_UsernameField.text;
        string password = m_PasswordField.text;

        if (string.IsNullOrEmpty(username))
        {
            WarningMessageUI.Instance.ShowMessage("You must type your username to log in!", 2f);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            WarningMessageUI.Instance.ShowMessage("You must type your password to log in!", 2f);
            return;
        }

        LoadingWindowUI.Instance.Register(this);
        m_LoginHandler.Login(username, password);
    }

    public void AttemptRegister()
    {
        string username = m_RUsernameField.text;
        string password = m_RPasswordField.text;
        string passwordConfirm = m_RPasswordConfirmField.text;

        if (string.IsNullOrEmpty(username))
        {
            WarningMessageUI.Instance.ShowMessage("You must type your username to register!", 2f);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            WarningMessageUI.Instance.ShowMessage("You must type your password to register!", 2f);
            return;
        }

        if (string.IsNullOrEmpty(passwordConfirm))
        {
            WarningMessageUI.Instance.ShowMessage("You must type your password again to register!", 2f);
            return;
        }

        if (passwordConfirm != password)
        {
            WarningMessageUI.Instance.ShowMessage("The password and its confirmation do not match!", 2f);
            return;
        }

        LoadingWindowUI.Instance.Register(this);
        m_RegisterationPanel.SetActive(false);
        m_LoginPanel.SetActive(true);
        m_RegisterHandler.Register(username, password);
    }

    public void AttemptQuit()
    {
        //TODO Close socket from here.
        Application.Quit();
    }

    public void AttemptLogout()
    {
        LoadingWindowUI.Instance.Register(this);
        m_LogoutHandler.Logout();
    }

    public void CreateCharacterMenu()
    {
        MoveToMenu(2);
        m_CreateCharacterUI.Init();
    }

    public void MoveToMenu(int index)
    {
        m_PageMenu.SwitchTo(index);
    }

    #endregion

    #region Internal Methods

    public void LoadPlayerCharacters(User user)
    {
        StartCoroutine(m_CharacterSelectionUI.LoadCharactersCoroutine(user));
    }

    public void SelectLastCharacter(User user)
    {
        StartCoroutine(ShowCharacterInfoRoutine(user.Characters[user.Characters.Count - 1]));
    }

    protected IEnumerator ShowCharacterInfoRoutine(ActorInfo info)
    {
        // wait for the page to first get back to the main menu
        yield return new WaitForSeconds(m_PageMenu.m_fDelay + m_PageMenu.m_fSpeed);
        m_CharacterSelectionUI.ShowCharacterInfo(info);
    }

    public void ResetLoginFields()
    {
        m_UsernameField.text = "";
        m_PasswordField.text = "";
    }

    public void ResetRegisterFields()
    {
        m_RPasswordConfirmField.text = "";
        m_RUsernameField.text = "";
        m_RPasswordField.text = "";
    }

    #endregion

    #region Callbacks

    public void LoginResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);

        if (response["error"] != null)
        {
            WarningMessageUI.Instance.ShowMessage(response["error"].Value);
            AudioControl.Instance.Play("sound_negative");
        }
        else
        {
            LocalUserInfo.Me.UpdateData(response["data"]);
            MoveToMenu(1);
            LoadPlayerCharacters(LocalUserInfo.Me);
            AudioControl.Instance.Play("sound_positive");
        }
    }

    public void RegisterationResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);

        if (response["error"] != null)
        {
            WarningMessageUI.Instance.ShowMessage(response["error"].Value);
            AudioControl.Instance.Play("sound_negative");
        }
        else
        {
            LocalUserInfo.Me.UpdateData(response["data"]);
            WarningMessageUI.Instance.ShowMessage("Welcome, new member!", 1f);
            MoveToMenu(1);
            LoadPlayerCharacters(LocalUserInfo.Me);
            AudioControl.Instance.Play("sound_positiveprogress");
        }
    }

    public void SessionResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);

        if (response["error"] != null)
        {
            Debug.Log("Session could not be done: " + response["error"].Value);
            AudioControl.Instance.Play("sound_negative");
        }
        else
        {
            Debug.Log(response["data"]);
            LocalUserInfo.Me.UpdateData(response["data"]);
            MoveToMenu(1);
            LoadPlayerCharacters(LocalUserInfo.Me);
            for (int i = 0; i < LocalUserInfo.Me.Characters.Count; i++)
            {
                if (LocalUserInfo.Me.Characters[i].ID == LastLoggedInCharId)
                {
                    StartCoroutine(ShowCharacterInfoRoutine(LocalUserInfo.Me.Characters[i]));
                }
            }
            LastLoggedInCharId = null;
            AudioControl.Instance.Play("sound_positive");
        }
    }

    public void LogoutResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);
        LocalUserInfo.DisposeCurrentUser();

        if (response["error"] != null)
        {
            Debug.Log("Session could not be done: " + response["error"].Value);
        }
        else
        {
            Debug.Log(response);
        }

        ResetLoginFields();
        MoveToMenu(0);
    }

    #endregion
}
