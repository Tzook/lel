using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinilogUI : MonoBehaviour {

    [SerializeField]
    int MaxLines = 5;

    [SerializeField]
    float HideDelay;

    public void AddMessage(string message)
    {
        StartCoroutine(AddMessageRoutine(message));
    }

    private IEnumerator AddMessageRoutine(string message)
    {
        if(transform.childCount >= MaxLines)
        {
            DisposeLine(transform.GetChild(0));
        }

        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("MinilogPiece");
        tempObj.transform.SetParent(transform, false);
        tempObj.transform.position = transform.position;
        tempObj.transform.localScale = Vector3.one;

        Text tempText = tempObj.GetComponent<Text>();

        tempText.text = message;

        float t = 0f;
        while (t < 1f)
        {
            t += 3f * Time.deltaTime;

            tempText.color = new Color(tempText.color.r, tempText.color.g, tempText.color.b, t);

            yield return 0;
        }

        if (ClearingRoutineInstance == null)
        {
            ClearingRoutineInstance = StartCoroutine(ClearingRoutine());
        }
    }

    Coroutine ClearingRoutineInstance;
    private IEnumerator ClearingRoutine()
    {
        while(transform.childCount > 0)
        {
            yield return new WaitForSeconds(HideDelay);

            Text tempText = transform.GetChild(0).GetComponent<Text>();

            float t = 1f;
            while(t>0f)
            {
                t -= 2f * Time.deltaTime;
                tempText.color = new Color(tempText.color.r, tempText.color.g, tempText.color.b, t);

                yield return 0;
            }

            DisposeLine(tempText.transform);
        }

        ClearingRoutineInstance = null;
    }


    private void DisposeLine(Transform lineTransform)
    {
        lineTransform.gameObject.SetActive(false);
        lineTransform.SetParent(null);
    }
}
