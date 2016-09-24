using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public SceneControl CurrentScene;
    public bool InGame { protected set; get; }
    public static Game Instance;

    void Awake()
    {
        Instance = this;
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadSceneRoutine(scene));
    }

    public void LeaveToMainMenu()
    {
        if(InGame)
        {
            InGame = false;

            ResourcesLoader.Instance.ClearObjectPool();
            SocketClient.Instance.Diconnect();
            SceneManager.LoadScene("MainMenu");
        }
    }

    public GameObject SpawnPlayer(ActorInfo info)
    {
        CurrentScene.Join(info);

        GameObject tempObj;
        if (info.Gender == Gender.Male)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("actor_male");
        }
        else
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("actor_female");
        }

        
        tempObj.GetComponent<ActorInstance>().UpdateVisual(info);
        tempObj.transform.position = info.LastPosition;
        return tempObj;

    }

    public void LoadNpcCharacter(ActorInfo info)
    {
        GameObject tempObj = SpawnPlayer(info);

        tempObj.GetComponent<ActorMovement>().enabled = true;
        tempObj.GetComponent<ActorController>().enabled = false;
        tempObj.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void RemoveNpcCharacter(ActorInfo info)
    {
        GameObject leavingPlayer = CurrentScene.GetPlayer(info.ID).Instance.gameObject;

        leavingPlayer.SetActive(false);

        CurrentScene.Leave(info);
    }

    public void LoadPlayerCharacter()
    {
        GameObject tempObj = SpawnPlayer(LocalUserInfo.Me.SelectedCharacter);

        tempObj.GetComponent<ActorMovement>().enabled = false;
        tempObj.GetComponent<ActorController>().enabled = true;
        tempObj.GetComponent<Rigidbody2D>().isKinematic = false;
    }

    protected IEnumerator LoadSceneRoutine(string scene)
    {
        string lastScene = SceneManager.GetActiveScene().name;
        ResourcesLoader.Instance.ClearObjectPool();
        SceneManager.LoadScene(scene);

        while(lastScene == SceneManager.GetActiveScene().name)
        {
            yield return 0;
        }

        CurrentScene = new SceneControl();

        SocketClient.Instance.EmitLoadedScene();

        while(GameCamera.Instance==null)
        {
            yield return 0;
        }

        GameCamera.Instance.Register(CurrentScene.ClientCharacter.Instance.gameObject);

        InGame = true;
    }


}
