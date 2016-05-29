using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	void Awake()
    {
        SM.Game = this;
        Application.runInBackground = true;
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadSceneRoutine(scene));
    }

    public GameObject SpawnPlayer(ActorInfo info)
    {
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

        SM.SocketClient.EmitLoadedScene();
    }


}
