using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

class ResolutionEnforcer : UIBehaviour
{
    float EnforcedAspect = 16.0f / 9.0f;

    Coroutine ThrottledEnforceCoroutine;

    override protected void Awake()
    {
        // listen to scene change and update accordingly
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    override protected void OnDestroy()
    {
        // cleanup
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    // listen to screen resize and update resolution accordingly
    override protected void OnRectTransformDimensionsChange()
    {
        // limit the resize event since it gets called many times
        if (ThrottledEnforceCoroutine == null && gameObject.activeInHierarchy)
        {
            ThrottledEnforceCoroutine = StartCoroutine(ThrottledEnforce());
        }
    }
    
    public void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        Enforce();
    }

    protected IEnumerator ThrottledEnforce()
    {
        yield return new WaitForSeconds(0.05f);
        Enforce();
        // we have to wait a frame since we just resized manually so it will get called again
        yield return null;
        ThrottledEnforceCoroutine = null;
    }

    protected void Enforce()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;

        float scaleHeight = currentAspect / EnforcedAspect;
        Camera[] cameras = Camera.allCameras;

        foreach (var camera in cameras)
        {
            // if scaled height is less than current height, add letterbox
            if (scaleHeight < 1.0f)
            {
                Rect rect = camera.rect;

                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;

                camera.rect = rect;
            }
            // add pillarbox
            else
            {
                float scalewidth = 1.0f / scaleHeight;

                Rect rect = camera.rect;

                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;

                camera.rect = rect;
            }
        }
    }
}