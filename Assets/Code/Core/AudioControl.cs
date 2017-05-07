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

    void Awake()
    {
        m_res = GetComponent<ResourcesLoader>();
        Instance = this;
    }

    #endregion

    #region Methods

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

        currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().pitch = fPitch;
        currentInstance.GetComponent<AudioSource>().Play();

        if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
        }
    }


    #endregion
}
