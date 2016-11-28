using UnityEngine;
using System.Collections;

public class InGameMainMenuUI : MonoBehaviour {

    [SerializeField]
    protected GameObject menuPanel;

    [SerializeField]
    protected ChatboxUI chatPanel;

    [SerializeField]
    protected GameObject optionsPanel;

    [SerializeField]
    protected GameObject inventoryPanel;

    [SerializeField]
    protected CharInfoUI m_CharInfoUI;

    public static InGameMainMenuUI Instance;

    public bool isWindowOpen
    {
        set
        {

        }
        get
        {
            for(int i=0;i<transform.childCount;i++)
            {
                if(transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    return true;
                }
            }

            return false;
        }
    }

	void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isWindowOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuPanel.SetActive(true);
            }
        }

        if (Input.GetKeyDown(InputMap.Map["Inventory"]))
        {
            if (!inventoryPanel.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                inventoryPanel.SetActive(true);
            }
            else
            {
                inventoryPanel.SetActive(false);
            }
        }

        if (Input.GetKeyDown(InputMap.Map["Chat"]))
        {
            if (!chatPanel.gameObject.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                chatPanel.Open();
            }
        }
    }

    public void Resume()
    {
        menuPanel.SetActive(false);
    }

    public void Logout()
    {
        menuPanel.SetActive(false);
        Game.Instance.LeaveToMainMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowCharacterInfo(ActorInfo Info)
    {
        m_CharInfoUI.Open(Info);
    }
}
