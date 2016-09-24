using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

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
    protected Transform CharactersContainer;

    [SerializeField]
    protected CreateCharacterUI m_CreateCharacterUI;

    [SerializeField]
    protected CharacterInfoPageUI m_CharacterInfoUI;

    [SerializeField]
    protected ui_pageMenu m_CharacterSelectionPageMenu;

    protected LoginHandler m_LoginHandler;
    protected RegisterHandler m_RegisterHandler;
    protected LogoutHandler m_LogoutHandler;
    protected SessionHandler m_SessionHandler;
    protected ui_pageMenu m_PageMenu;

    #endregion

    #region Mono

    void Awake()
    {
        m_PageMenu = GetComponent<ui_pageMenu>();
    }

    void Start()
    {
        m_RegisterationPanel.SetActive(false);
        m_LoginHandler = new LoginHandler(LoginResponse);
        m_RegisterHandler = new RegisterHandler(RegisterationResponse);
        m_SessionHandler = new SessionHandler(SessionResponse);
        m_LogoutHandler = new LogoutHandler(LogoutResponse);

        LoadingWindowUI.Instance.Register(this);
        m_SessionHandler.Session();
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
        m_CreateCharacterUI.Init();
        MoveToMenu(2);
    }

    public void MoveToMenu(int index)
    {
        m_PageMenu.SwitchTo(index);
    }

    #endregion

    #region Internal Methods

    public void LoadPlayerCharacters(User user)
    {
        StartCoroutine(LoadCharactersCoroutine(user));
    }

    protected IEnumerator LoadCharactersCoroutine(User user)
    {
        yield return 0;
        ClearPlayerCharacters();

        for (int i = 0; i < user.Characters.Count; i++)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("CharSpot");
            tempObj.transform.SetParent(CharactersContainer, false);
            tempObj.GetComponent<CharspotUI>().Load(user.Characters[i]);
            AddListenerToCharspot(tempObj.GetComponent<Button>(), user.Characters[i]);
        }
    }

    protected void AddListenerToCharspot(Button charspotButton, ActorInfo info)
    {
        charspotButton.onClick.AddListener(delegate
        {
            m_CharacterSelectionPageMenu.SwitchTo(1);
            m_CharacterInfoUI.SetInfo(info);
        });
    }

    public void ClearPlayerCharacters()
    {
        for (int i = 0; i < CharactersContainer.childCount; i++)
        {
            CharactersContainer.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            CharactersContainer.GetChild(i).gameObject.SetActive(false);
        }
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
            WarningMessageUI.Instance.ShowMessage(response["error"].ToString());
        }
        else
        {
            LocalUserInfo.Me.UpdateData(response["data"]);
            WarningMessageUI.Instance.ShowMessage(response["data"].ToString());
            MoveToMenu(1);
            LoadPlayerCharacters(LocalUserInfo.Me);
        }
    }

    public void RegisterationResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);

        if (response["error"] != null)
        {
            WarningMessageUI.Instance.ShowMessage(response["error"].ToString());
        }
        else
        {
            LocalUserInfo.Me.UpdateData(response["data"]);
            WarningMessageUI.Instance.ShowMessage("Welcome, new member!", 1f);
            MoveToMenu(1);
            LoadPlayerCharacters(LocalUserInfo.Me);
        }
    }

    public void SessionResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);

        if (response["error"] != null)
        {
            //SM.WarningMessage.ShowMessage(response["error"].ToString());
            Debug.Log("Session could not be done: " + response["error"].Value);
        }
        else
        {
            Debug.Log(response["data"]);
            LocalUserInfo.Me.UpdateData(response["data"]);
            MoveToMenu(1);
            LoadPlayerCharacters(LocalUserInfo.Me);
        }
    }

    public void LogoutResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);
        LocalUserInfo.DisposeCurrentUser();

        if (response["error"] != null)
        {
            //SM.WarningMessage.ShowMessage(response["error"].ToString());
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
