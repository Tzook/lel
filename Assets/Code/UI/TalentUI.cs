using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentUI : MonoBehaviour {

    [SerializeField]
    Text PointCount;

    [SerializeField]
    GameObject Lock;

    [SerializeField]
    string TalentKey;

    TalentContent Talent;

    public void Set()
    {
        Talent = TalentsContent.Instance.GetTalent(TalentKey);

        PointCount.text = "0/"+Talent.PointCap;

    }
}
