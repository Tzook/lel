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
    Toggle toggle;
    
    [SerializeField]
    Text sceneName;
    
    [SerializeField]
    Image actor;
     
    private float toggleHeight;
    private float? originalPanelHeight;

    public void Awake()
    {
        // listen to scene change and update accordingly
        SceneManager.activeSceneChanged += OnSceneChanged;
        // initialize with the first scene
        UpdateToNewScene(SceneManager.GetActiveScene());
        // pre-calculate heights for computations
        toggleHeight = toggle.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void LateUpdate()
    {
        // painting the minimap in late update so the content will be first calculated in update
        if (IsPanelOpen() && !Game.Instance.isLoadingScene && SceneInfo.Instance.miniMapInfo != null)
        {
            // TODO draw the minimap based on map points

            Vector2 sizeDelta = contentPanel.sizeDelta;
            float panelWidth = sizeDelta.x - 10;
            float panelHeight = sizeDelta.y - toggleHeight - 10;

            Vector2 actorPosition = Game.Instance.CurrentScene.ClientCharacter.Instance.transform.position;
            Vector2 percentPosition = SceneInfo.Instance.miniMapInfo.getPercentLocation(actorPosition);
            Vector2 newActorPosition = new Vector2(percentPosition.x * panelWidth - panelWidth / 2, -percentPosition.y * panelHeight - toggleHeight);
            actor.transform.localPosition = newActorPosition;
        }
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
        if(SceneInfo.Instance.miniMapInfo == null || SceneInfo.Instance.miniMapInfo.coliders.Count == 0)
        {
            ClosePanel();
        }
    }

    public void TogglePanel()
    {
        if (IsPanelOpen())
        {
            ClosePanel();
        }
        else 
        {
            OpenPanel();
        }
    }

    protected bool IsPanelOpen()
    {
        return originalPanelHeight == null;
    }

    protected void ClosePanel()
    {
        originalPanelHeight = contentPanel.sizeDelta.y;
        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, toggleHeight);
        TogglePanelElement(actor, false);
    }

    protected void OpenPanel()
    {
        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, (float)originalPanelHeight);
        originalPanelHeight = null;
        TogglePanelElement(actor, true);
    }
    
    protected void TogglePanelElement(Image element, bool on)
    {
        Color temp = element.color;
        temp.a = on ? 1 : 0;
        element.color = temp;
    }
}
