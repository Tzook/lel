using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    Image Fillbar;

    public void SetHealthbar(float fromValue, float toValue, float maxValue, float Speed = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(SetHealthbarRoutine(fromValue, toValue, maxValue, Speed));
    }

    private IEnumerator SetHealthbarRoutine(float fromValue, float toValue, float maxValue, float speed)
    {
        float t = 0f;
        while(t<1f)
        {
            t += speed * Time.deltaTime;

            Fillbar.fillAmount = Mathf.Lerp(fromValue / maxValue, toValue / maxValue, t);

            yield return 0;
        }
    }
}
