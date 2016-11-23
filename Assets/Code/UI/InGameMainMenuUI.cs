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

    public static InGameMainMenuUI Instance;

	void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!menuPanel.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                menuPanel.SetActive(true);
            }
            else
            {
                menuPanel.SetActive(false);
            }

            if (optionsPanel.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                optionsPanel.SetActive(false);
            }

            if (inventoryPanel.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                inventoryPanel.SetActive(false);
            }
        }

        if(Input.GetKeyDown(InputMap.Map["Inventory"]))
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
}
