using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

// credit to https://gist.github.com/benblo/10732800
public class SceneSaver
{
    private static SceneSaver _instance; 
    public static SceneSaver Instance
    { get { return _instance == null ? _instance = new SceneSaver() : _instance; } }

    public delegate void ProcessAllScenesDelegate( string _scenePath, UnityEngine.Object _sceneObject );

    public void processAllScenes(
        ProcessAllScenesDelegate _callback )
    {
        processAllScenes(
            "Processing {0} Scenes",
            "Processing scene {1}/{0} : {2}",
            _callback);
    }
    /// <summary>
    /// Format {0} : scene count
    /// Format {1} : scene index
    /// Format {2} : scene path
    /// </summary>
    public void processAllScenes(
        string _titleFormat,
        string _messageFormat,
        ProcessAllScenesDelegate _callback )
    {
        if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
        {
            return;
        }

        EditorCoroutine.start(
            processAllScenesCoroutine(_titleFormat, _messageFormat, _callback));
    }
    IEnumerator processAllScenesCoroutine(
        string _titleFormat,
        string _messageFormat,
        ProcessAllScenesDelegate _callback )
    {
        var scenePaths = WriteConstantLists.Instance.GetAllScenesPaths();
        var sceneCount = scenePaths.Count;
        Debug.Log(string.Format("Processing {0} scenes", sceneCount));

        for (int i = 0; i < sceneCount; i++)
        {
            var scenePath = scenePaths[i];

            EditorUtility.DisplayProgressBar(
                string.Format(_titleFormat, sceneCount, i, scenePath),
                string.Format(_messageFormat, sceneCount, i, scenePath),
                (float)i / sceneCount);

            UnityEngine.Object sceneObject = AssetDatabase.LoadMainAssetAtPath(scenePath);

            if (EditorApplication.OpenScene(scenePath))
            {
                // delay one frame to give a chance for all Awake/Start/OnEnable callbacks to trigger
                yield return null;

                try
                {
                    _callback(scenePath, sceneObject);
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Error while processing scene  '{0}'", scenePath), sceneObject);
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogError(string.Format("Failed to open scene '{0}'", scenePath), sceneObject);
            }
        }

        EditorUtility.ClearProgressBar();
        EditorApplication.NewScene();
    }

    public void resaveAllScenes()
    {
        processAllScenes(
            "Resaving {0} Scenes",
            "Resaving scene {1}/{0} : {2}",
            ( scenePath, sceneObject ) =>
            {
                if (EditorApplication.SaveScene())
                {
                    Debug.Log(string.Format("Scene '{0}' saved successfully", scenePath), sceneObject);
                }
                else
                {
                    Debug.LogError(string.Format("Scene '{0}' save failed", scenePath), sceneObject);
                }
            });
    }

    void openAllScenes()
    {
        processAllScenes(
            "Opening {0} Scenes",
            "Opening scene {1}/{0}\n{2}",
            ( scenePath, sceneObject ) =>
            {
                Debug.Log(string.Format("Scene '{0}' open successfully", scenePath), sceneObject);
            });
    }
}