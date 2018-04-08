using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeAttackBarUI : MonoBehaviour {

    [SerializeField]
    Image Center;

    [SerializeField]
    Image Fill;

    [SerializeField]
    Color CenterColorA;

    [SerializeField]
    Color CenterColorB;

    Transform CurrentTarget;

    void LateUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, (CurrentTarget.position + transform.TransformDirection(0f, 1f, 0f)),Time.deltaTime * 2f);
    }

    public void StartCharging(Transform target, Sprite icon = null)
    {
        if(icon == null)
        {
            Center.sprite = ResourcesLoader.Instance.GetSprite("ui_sheet01_4");
        }
        else
        {
            Center.sprite = icon;
        }

        this.gameObject.SetActive(true);
        CurrentTarget = target;
    }

    public void SetValue(float val)
    {
        Center.color = Color.Lerp(CenterColorA, CenterColorB, val);
        Fill.fillAmount = val;
    }
    
    public void StopCharging()
    {
        this.gameObject.SetActive(false);
    }
}
