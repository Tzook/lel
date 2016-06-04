using UnityEngine;
using System.Collections;

public class InGameMainMenuUI : MonoBehaviour {

    [SerializeField]
    protected GameObject menuPanel;

	void Awake()
    {
        SM.InGameMainMenu = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!menuPanel.activeInHierarchy && SM.Game.InGame)
            {
                menuPanel.SetActive(true);
            }
            else
            {
                menuPanel.SetActive(false);
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
        SM.Game.LeaveToMainMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
