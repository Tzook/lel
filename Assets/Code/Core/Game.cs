using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	void Awake()
    {
        SM.Game = this;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadScene(string scene)
    {
        SM.Resources.ClearObjectPool();

        SceneManager.LoadScene(scene);
    }



}
