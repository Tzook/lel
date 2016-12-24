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

    [SerializeField]
    protected CanvasGroup m_DimmerCanvasGroup;

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
                inventoryPanel.GetComponent<InventoryUI>().ShowInventory(Game.Instance.ClientCharacter.GetComponent<ActorInstance>().Info);
            }
            else
            {
                inventoryPanel.GetComponent<InventoryUI>().Hide();
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

    public IEnumerator FadeInRoutine()
    {
        m_DimmerCanvasGroup.alpha = 0;
        while(m_DimmerCanvasGroup.alpha < 1f)
        {
            m_DimmerCanvasGroup.alpha += 2f * Time.deltaTime;
            yield return 0;
        }
    }

    public IEnumerator FadeOutRoutine()
    {
        m_DimmerCanvasGroup.alpha = 1;
        while (m_DimmerCanvasGroup.alpha > 0f)
        {
            m_DimmerCanvasGroup.alpha -= 2f * Time.deltaTime;
            yield return 0;
        }
    }
}
