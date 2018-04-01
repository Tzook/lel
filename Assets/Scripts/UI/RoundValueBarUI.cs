using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RoundValueBarUI : MonoBehaviour {

    [SerializeField]
    Image ChangeBarPanel;

    [SerializeField]
    Image BarPanel;

    [SerializeField]
    Text ValueText;

    [SerializeField]
    bool ShowAsPrecent = false;

    Color icChangeBar;
    Color icBar;
    Color icText;

    [SerializeField]
    float LowValue = 0f;

    [SerializeField]
    ParticleSystem AddParticles;

    [SerializeField]
    ParticleSystem LoseParticles;

    float CurrentValue;

    public Coroutine ChangeValueInstance { get; private set; }
    public Coroutine LowValueInstance { get; private set; }

    void OnEnable()
    {
        icChangeBar = ChangeBarPanel.color;

        icBar = BarPanel.color;
        icText = ValueText.color;
    }

    void OnDisable()
    {
        LowValueInstance = null;
    }

    public void SetValue(float valueMin, float valueMax)
    {
        HaltCurrentRoutine();
        ChangeValueInstance = StartCoroutine(ChangeValueRoutine(CurrentValue, valueMin, valueMax));

        CurrentValue = valueMin / valueMax;

        if(valueMin < LowValue)
        {
            if (LowValueInstance == null)
            {
                LowValueInstance = StartCoroutine(LowValueRoutine());
            }
        }
        else
        {
            if (LowValueInstance != null)
            {
                StopCoroutine(LowValueInstance);
                LowValueInstance = null;
            }
        }
    }

    private IEnumerator LowValueRoutine()
    {
        float t;
        while(true)
        {
            t = 0f;
            while(t<1f)
            {
                t += 3f * Time.deltaTime;

                BarPanel.color = Color.Lerp(new Color(icBar.r, icBar.g, icBar.b, 1f), new Color(icBar.r, icBar.g, icBar.b, 0.55f), t);
                ValueText.color = Color.Lerp(new Color(icText.r, icText.g, icText.b, 1f), new Color(icText.r, icText.g, icText.b, 0.75f), t);

                yield return 0;
            }

            t = 0f;
            while (t < 1f)
            {
                t += 3f * Time.deltaTime;

                BarPanel.color = Color.Lerp(new Color(icBar.r, icBar.g, icBar.b, 0.55f), new Color(icBar.r, icBar.g, icBar.b, 1f), t);
                ValueText.color = Color.Lerp(new Color(icText.r, icText.g, icText.b, 0.75f), new Color(icText.r, icText.g, icText.b, 1f), t);

                yield return 0;
            }
        }
    }

    private IEnumerator ChangeValueRoutine(float currentValue, float minValue, float maxValue)
    {
        if(currentValue < minValue)
        {
            if (AddParticles != null)
            {
                AddParticles.Play();
            }
        }
        else
        {
            if (LoseParticles != null)
            {
                LoseParticles.Play();
            }
        }

        float t = 0f;
        while(t<1f)
        {
            t += 4f * Time.deltaTime;

            ChangeBarPanel.color = Color.Lerp(new Color(icChangeBar.r, icChangeBar.g, icChangeBar.b, 0f), new Color(icChangeBar.r, icChangeBar.g, icChangeBar.b, 0.75f),t);

            yield return 0;
        }

        if (ShowAsPrecent)
        {
            t = 0f;
            while (t < 1f)
            {
                t += 2f * Time.deltaTime;

                BarPanel.fillAmount = Mathf.Lerp(currentValue, (minValue / maxValue), t);
                ChangeBarPanel.fillAmount = BarPanel.fillAmount;

                ValueText.text = Mathf.FloorToInt(BarPanel.fillAmount * 100f) + "%";



                yield return 0;
            }
        }
        else
        {
            t = 0f;
            while (t < 1f)
            {
                t += 2f * Time.deltaTime;

                BarPanel.fillAmount = Mathf.Lerp(currentValue, (minValue / maxValue), t);
                ChangeBarPanel.fillAmount = BarPanel.fillAmount;

                ValueText.text = Mathf.FloorToInt(Mathf.Lerp(currentValue, minValue, t)).ToString();



                yield return 0;
            }
        }

        t = 0f;
        while (t < 1f)
        {
            t += 4f * Time.deltaTime;

            ChangeBarPanel.color = Color.Lerp(new Color(icChangeBar.r, icChangeBar.g, icChangeBar.b, 0.75f), new Color(icChangeBar.r, icChangeBar.g, icChangeBar.b, 0f), t);

            yield return 0;
        }

        if (LoseParticles != null)
        {
            LoseParticles.Stop();
        }

        if (AddParticles != null)
        {
            AddParticles.Stop();
        }
    }

    public void SetValueInstant(float valuePrecent)
    {
        HaltCurrentRoutine();

        CurrentValue = valuePrecent;

        BarPanel.fillAmount = valuePrecent;

        ValueText.text = Mathf.FloorToInt(valuePrecent * 100f) + "%";

        ChangeBarPanel.color = Color.clear;
    }

    private void HaltCurrentRoutine()
    {
        if (LoseParticles != null)
        {
            LoseParticles.Stop();
        }

        if (AddParticles != null)
        {
            AddParticles.Stop();
        }

        if (ChangeValueInstance != null)
        {
            StopCoroutine(ChangeValueInstance);
            ChangeValueInstance = null;
        }
    }
}
