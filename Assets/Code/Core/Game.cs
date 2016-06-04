using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public SceneControl CurrentScene;
    public bool InGame { protected set; get; }

    void Awake()
    {
        SM.Game = this;
        Application.runInBackground = true;
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

            SM.Resources.ClearObjectPool();
            SM.SocketClient.Diconnect();
            SceneManager.LoadScene("MainMenu");
        }
    }

    public GameObject SpawnPlayer(ActorInfo info)
    {
        CurrentScene.Join(info);

        GameObject tempObj = SM.Resources.GetRecycledObject("actor");
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
        SM.Resources.ClearObjectPool();
        SceneManager.LoadScene(scene);

        while(lastScene == SceneManager.GetActiveScene().name)
        {
            yield return 0;
        }

        CurrentScene = new SceneControl();

        SM.SocketClient.EmitLoadedScene();

        while(SM.GameCamera==null)
        {
            yield return 0;
        }

        SM.GameCamera.Register(CurrentScene.ClientCharacter.Instance.gameObject);

        InGame = true;
    }


}
