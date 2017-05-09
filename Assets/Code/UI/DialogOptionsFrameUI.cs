using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogOptionsFrameUI : MonoBehaviour {

    [SerializeField]
    Text m_InstructionsText;

    public void StartWavingInstruction(string instruction)
    {
        m_InstructionsText.text = instruction;
        m_InstructionsText.gameObject.SetActive(true);
        StartCoroutine(WaveInstruction());
    }

    private IEnumerator WaveInstruction()
    {
        float t;
        while (true)
        {
            t = 0f;
            while (t < 1f)
            {
                t += 1f * Time.deltaTime;
                m_InstructionsText.color = new Color(m_InstructionsText.color.r, m_InstructionsText.color.g, m_InstructionsText.color.b, t);

                yield return 0;
            }

            t = 1f;
            while (t > 0f)
            {
                t -= 1f * Time.deltaTime;
                m_InstructionsText.color = new Color(m_InstructionsText.color.r, m_InstructionsText.color.g, m_InstructionsText.color.b, t);

                yield return 0;
            }

            yield return 0;
        }
    }

    public void StopWavingInstruction()
    {
        StopAllCoroutines();
        m_InstructionsText.text = "";
        m_InstructionsText.gameObject.SetActive(false);
    }
}
