using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniMapUI : MonoBehaviour 
{
    [SerializeField]
    RectTransform contentPanel;

    [SerializeField]
    Text sceneName;
    
    [SerializeField]
    Image actor;

    private float panelHeight;
    private float panelWidth;

    public void Awake()
    {
        // listen to scene change and update accordingly
        SceneManager.activeSceneChanged += OnSceneChanged;

        Vector3[] corners = new Vector3[4];
        contentPanel.GetLocalCorners(corners);
        panelWidth = corners[2].x - corners[1].x - 10;
        panelHeight = corners[1].y - corners[0].y - 10;

        // initialize with the first scene
        UpdateToNewScene(SceneManager.GetActiveScene());
    }

    public void LateUpdate()
    {
        // painting the minimap in late update so the content will be first calculated in update
        if (!Game.Instance.isLoadingScene && SceneInfo.Instance.miniMapInfo != null)
        {
            Vector2 actorPosition = Game.Instance.CurrentScene.ClientCharacter.Instance.transform.position;
            Vector2 newActorPosition = GetRelativePosition(actorPosition);
            actor.transform.localPosition = newActorPosition;
        }
    }

    protected Vector2 GetRelativePosition(Vector2 point)
    {
        Vector2 percentPosition = getPercentLocation(point);
        Vector2 newPoint = new Vector2(percentPosition.x * panelWidth - panelWidth / 2, -percentPosition.y * panelHeight);
        return newPoint;
    }

    protected Vector2 getPercentLocation(Vector2 position)
    {
        Vector2 percentPosition = new Vector2();
        
        percentPosition.x = (position.x - SceneInfo.Instance.miniMapInfo.left) / (SceneInfo.Instance.miniMapInfo.right - SceneInfo.Instance.miniMapInfo.left);
        percentPosition.y = (position.y - SceneInfo.Instance.miniMapInfo.top) / (SceneInfo.Instance.miniMapInfo.bottom - SceneInfo.Instance.miniMapInfo.top);

        return percentPosition;
    }

    public void Destroy()
    {
        // cleanup
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    public void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        UpdateToNewScene(newScene);
    }

    protected void UpdateToNewScene(Scene scene) 
    {
        sceneName.text = scene.name;

        foreach (var colliderGroup in SceneInfo.Instance.miniMapInfo.coliders)
        {
            LineRenderer line = ResourcesLoader.Instance.GetRecycledObject("MiniMapTerrainLine").GetComponent<LineRenderer>();
            line.positionCount = colliderGroup.coliders.Count;
            line.SetPositions(GetColliders(colliderGroup));
            line.transform.SetParent(contentPanel, false);
        }
    }

    protected Vector3[] GetColliders(ColiderGroup colliderGroup)
    {
        Vector3[] colliders = new Vector3[colliderGroup.coliders.Count];
        int i = 0;
        foreach (var collider in colliderGroup.coliders)
        {
            colliders[i++] = GetRelativePosition(collider);
        }
        return colliders;
    }
}
