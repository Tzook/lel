using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour {

    [SerializeField]
    Slider MasterVolume;

    [SerializeField]
    Slider SFXVolume;

    [SerializeField]
    Slider MusicVolume;

    public void Show()
    {
        this.gameObject.SetActive(true);
        SFXVolume.value = AudioControl.Instance.GetVolumeByTag("Untagged");
        MusicVolume.value = AudioControl.Instance.GetVolumeByTag("Music");
        MasterVolume.value = AudioListener.volume;
    }

    public void RefreshVolumeSFX(string Tag)
    {
        AudioControl.Instance.SetVolume(Tag, SFXVolume.value);
    }

    public void RefreshVolumeMusic(string Tag)
    {
        AudioControl.Instance.SetVolume(Tag, MusicVolume.value);
    }

    public void RefreshVolumeMaster()
    {
        AudioControl.Instance.SetMasterVolume(MasterVolume.value);
    }
}
