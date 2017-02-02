using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopText : MonoBehaviour {

    [SerializeField]
    Text TextContent;

    [SerializeField]
    Text TextContentBG;

    [SerializeField]
    CanvasGroup CG;

    float TimeAlive = 0f;
    public void Pop(string Content, Color clr)
    {
        TextContent.text = Content;
        TextContentBG.text = Content;

        TextContent.color = clr;

        TextContentBG.color = new Color(clr.r - (clr.r / 10f), clr.g - (clr.g / 10f), clr.b - (clr.b / 10f), clr.a);

        CG.alpha = 1f;

        StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        Vector3 initPos = transform.position;
        Vector3 targetPos = initPos + new Vector3(0f, 1f, 0f);

        float t = 0f;
        while(t<1f)
        {
            t += 1f * Time.deltaTime;

            transform.position = Vector3.Lerp(initPos, targetPos, t);
            CG.alpha = 1f - t;

            yield return 0;
        }

        gameObject.SetActive(false);
    }
}
