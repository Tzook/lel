using UnityEngine;

public class ColorIndicator : MonoBehaviour {

	HSBColor color;
	public Color resultColor;

	void Start() {
		transform.parent.BroadcastMessage("SetColor", color);
	}

	void ApplyColor ()
	{
		resultColor = color.ToColor();
		transform.parent.BroadcastMessage("OnColorChange", color, SendMessageOptions.DontRequireReceiver);
	}

	void SetHue(float hue)
	{
		color.h = hue;
		ApplyColor();
    }	

	void SetSaturationBrightness(Vector2 sb) {
		color.s = sb.x;
		color.b = sb.y;
		ApplyColor();
	}
}
