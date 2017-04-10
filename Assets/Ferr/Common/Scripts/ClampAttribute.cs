using System;
using UnityEngine;

public class ClampAttribute : PropertyAttribute {
	public float mMin, mMax;
	public ClampAttribute(float aMin, float aMax) {
		mMin = aMin;
		mMax = aMax;
	}
}
