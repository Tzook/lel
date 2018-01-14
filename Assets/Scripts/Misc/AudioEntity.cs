using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEntity : MonoBehaviour {

    public void Play(string Key)
    {
        AudioControl.Instance.Play(Key);
    }
}
