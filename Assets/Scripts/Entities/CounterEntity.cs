using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CounterEntity : MonoBehaviour {

    [SerializeField]
    Text m_Label;

    [SerializeField]
    UnityEvent HitNumber;

    [SerializeField]
    UnityEvent AboveNumber;

    [SerializeField]
    UnityEvent BelowNumber;

    [SerializeField]
    int TargetNumber;

    int Count;
    public void AddNumber(int num)
    {
        Count += num;

        if(TargetNumber < Count)
        {
            AboveNumber.Invoke();
        }
        else if (TargetNumber > Count)
        {
            BelowNumber.Invoke();
        }
        else
        {
            HitNumber.Invoke();
        }
              
        if (m_Label != null)
        {
            m_Label.text = Count.ToString();
        }
    }

    public void SetNumber(int num)
    {
        Count = num;

        if (m_Label != null)
        {
            m_Label.text = Count.ToString();
        }
    }


}
