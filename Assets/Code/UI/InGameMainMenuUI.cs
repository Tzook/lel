using UnityEngine;
using System.Collections;

public class InGameMainMenuUI : MonoBehaviour {

    [SerializeField]
    protected GameObject menuPanel;

    public static InGameMainMenuUI Instance;

	void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!menuPanel.activeInHierarchy && Game.Instance.InGame)
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
        Game.Instance.LeaveToMainMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
