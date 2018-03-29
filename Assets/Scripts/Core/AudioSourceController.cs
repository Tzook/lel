using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceController : MonoBehaviour
{
    AudioSource Source;

    [SerializeField]
    string VolumeTag = "Untagged";

    private void OnEnable()
    {
        Source = GetComponent<AudioSource>();

        Source.volume = AudioControl.Instance.GetVolumeByTag(VolumeTag);
    }
}
