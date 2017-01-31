using UnityEngine;
using System.Collections;

public class AnimationAssist : MonoBehaviour {

    public void PlaySound(string soundKey)
    {
        AudioControl.Instance.Play(soundKey);
    }
}
