using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(ResourcesLoader))]
public class AudioControl : MonoBehaviour {

    #region Essential
    public  string m_sInstancePrefab;
    private ResourcesLoader m_res;
    private List<GameObject> m_listInstances   = new List<GameObject>();
    private Dictionary<string, float> m_dicVolumeGroup = new Dictionary<string, float>();

    public static AudioControl Instance;

    [SerializeField]
    protected Transform m_tInstancesContainer;

    [SerializeField]
    protected AudioSource MusicSource;

    void Awake()
    {
        m_res = GetComponent<ResourcesLoader>();
        Instance = this;

        m_dicVolumeGroup.Add("Untagged", PlayerPrefs.GetFloat("Untagged", 1f));
        m_dicVolumeGroup.Add("Music", PlayerPrefs.GetFloat("Music", 0.6f));

        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));

        MusicSource.volume = m_dicVolumeGroup["Music"];

    }

    #endregion

    #region Methods

    public float GetVolumeByTag(string tag)
    {
        return m_dicVolumeGroup[tag];
    }

    public void PlayInPosition(string gClip, Vector3 pos, float MaxDistance = 47f)
    {
        GameObject currentInstance = null;

        for (int i = 0; i < m_listInstances.Count; i++)
        {
            if (!m_listInstances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = m_listInstances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            m_listInstances.Add(currentInstance);
        }

        currentInstance.transform.position = pos;
        currentInstance.GetComponent<AudioSource>().spatialBlend = 1f;
        currentInstance.GetComponent<AudioSource>().maxDistance = MaxDistance;
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

        if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
        }
        else
        {
        }
    }

    public void Play(string gClip)
    {
        GameObject currentInstance = null;

        for (int i=0;i<m_listInstances.Count;i++)
        {
            if(!m_listInstances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = m_listInstances[i];
                break;
            }
        }

        if(currentInstance==null)
        {
            currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            m_listInstances.Add(currentInstance);
        }

        currentInstance.GetComponent<AudioSource>().spatialBlend = 0f;
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

        if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
        }
    }

    public void Play(string gClip, bool gLoop)
    {
        GameObject currentInstance = null;

        for (int i = 0; i < m_listInstances.Count; i++)
        {
            if (!m_listInstances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = m_listInstances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            m_listInstances.Add(currentInstance);
        }

        currentInstance.GetComponent<AudioSource>().spatialBlend = 0f;
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().loop = gLoop;
        currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

        if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
        }

    }

    public void Play(string gClip, bool gLoop, string gTag)
    {
        GameObject currentInstance = null;

        for (int i = 0; i < m_listInstances.Count; i++)
        {
            if (!m_listInstances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = m_listInstances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            m_listInstances.Add(currentInstance);
        }

        currentInstance.GetComponent<AudioSource>().spatialBlend = 0f;
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().loop = gLoop;
        currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

        currentInstance.tag = gTag;

        if(m_dicVolumeGroup.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
        }
    }

    public void SetVolume(string gTag, float gVolume)
    {
        PlayerPrefs.SetFloat(gTag, gVolume);
        PlayerPrefs.Save();

        if (gTag == "Music")
        {
            MusicSource.volume = gVolume;
        }
        
        if(!m_dicVolumeGroup.ContainsKey(gTag))
        {
            m_dicVolumeGroup.Add(gTag, gVolume);
        }
        else
        {
            m_dicVolumeGroup[gTag] = gVolume;
        }

        for (int i = 0; i < m_listInstances.Count; i++)
        {
            if (m_listInstances[i].tag == gTag)
            {
                m_listInstances[i].GetComponent<AudioSource>().volume = gVolume;
            }
        }
    }

    public void SetMasterVolume(float gVolume)
    {
        AudioListener.volume = gVolume;
        PlayerPrefs.SetFloat("MasterVolume", gVolume);
        PlayerPrefs.Save();
    }

    public void PlayWithPitch(string gClip,float fPitch)
    {
        GameObject currentInstance = null;

        for (int i = 0; i < m_listInstances.Count; i++)
        {
            if (!m_listInstances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = m_listInstances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            m_listInstances.Add(currentInstance);
        }

        currentInstance.GetComponent<AudioSource>().spatialBlend = 0f;
        currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().pitch = fPitch;
        currentInstance.GetComponent<AudioSource>().Play();

        if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
        }
    }

    public void SetMusic(string gClip, float fPitch = 1f)
    {
        MusicSource.volume = m_dicVolumeGroup["Music"];

        if (string.IsNullOrEmpty(gClip))
        {
            MusicSource.Stop();
            MusicSource.clip = null;
            return;
        }

        MusicSource.pitch = fPitch;

        if(MusicSource.clip == null || MusicSource.clip.name != gClip)
        {
            MusicSource.Stop();
            MusicSource.clip = ResourcesLoader.Instance.GetClip(gClip);
            MusicSource.Play();
        }
    }

    public void StopSound(string gClip)
    {
        foreach(GameObject obj in m_listInstances)
        {
            if(obj.GetComponent<AudioSource>().isPlaying)
            {
                if(obj.GetComponent<AudioSource>().clip.name == gClip)
                {
                    obj.GetComponent<AudioSource>().Stop();
                }
            }
        }
    }


    #endregion
}
