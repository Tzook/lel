using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneState : MonoBehaviour {
    private Dictionary<string, string> sceneState = new Dictionary<string, string>();

    public static SceneState Instance;

    void Awake()
    {
        Instance = this;
        // listen to scene change and update accordingly
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    public string GetState(string key)
    {
        return sceneState[key];
    }

    public void SetState(string key, string value)
    {
        SocketClient.Instance.SendUpdateRoomState(key, value);
        OnStateChanged(key, value);
    }

    public void OnStateChanged(string key, string value)
    {
        sceneState[key] = value;
    }
    
    public void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        if (SceneInfo.Instance) 
        {
            UpdateToNewScene(newScene);
        }
    }

    protected void UpdateToNewScene(Scene scene) 
    {
        sceneState.Clear();
        for (int i = 0; i < SceneInfo.Instance.SceneState.Count; i++)
        {
            sceneState[SceneInfo.Instance.SceneState[i].Key] = SceneInfo.Instance.SceneState[i].Value;
        }
    }

    public void Destroy()
    {
        // cleanup
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}