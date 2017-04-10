using UnityEngine;
using System.Collections;

public class InspectorName : PropertyAttribute {
    public string mName;
    public InspectorName(string aName) {
        mName = aName;
    }
}
