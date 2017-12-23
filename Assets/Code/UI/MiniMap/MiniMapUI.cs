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

    private float? originalPanelHeight;

    public void Awake()
    {
        // listen to scene change and update accordingly
        SceneManager.activeSceneChanged += OnSceneChanged;
        // initialize with the first scene
        UpdateToNewScene(SceneManager.GetActiveScene());
    }

    public void LateUpdate()
    {
        // painting the minimap in late update so the content will be first calculated in update
        if (IsPanelOpen()) 
        {
            // TODO draw the minimap based on map points
            // PolygonCollider2D[] coliders = Object.FindObjectsOfType<PolygonCollider2D>();
            // Vector2[] points = coliders[0].points;

            // TODO draw the actor based on his position compare to all the points
            // Transform actorTransform = Game.Instance.CurrentScene.ClientCharacter.Instance.transform;
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
        float toggleHeight = toggle.GetComponent<RectTransform>().sizeDelta.y;
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
