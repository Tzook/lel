using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

//VERSION 1.3

public class ResourcesLoader : MonoBehaviour {

    #region Essential
    protected Dictionary<string, Sprite>     m_dicLoadedSprites = new Dictionary<string, Sprite>();
    protected Dictionary<string, GameObject> m_dicLoadedObjects = new Dictionary<string, GameObject>();
    protected Dictionary<string, AudioClip>  m_dicLoadedClips   = new Dictionary<string, AudioClip>();

    public static ResourcesLoader Instance;

    public Material LitSprite;
    public Material UnlitSprite;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (loadOnAwake)
        {
            LoadResources();
        }
    }

    #endregion

    #region Configurable Parameters

    public bool debugMode;
	public bool loadOnAwake;
    public bool m_bLoading;

    [Tooltip("Add only objects which are hidden on initialization but meant to be preloaded...")]
    public List<GameObject> m_listPreloadObjects = new List<GameObject>();

    #endregion

    #region External Methods

    public Sprite     GetSprite(string gKey)
	{
		if(m_dicLoadedSprites.ContainsKey(gKey))
		{
			return m_dicLoadedSprites[gKey];
		}
		else
		{
            if (debugMode)
            {
                Debug.LogError("Resource Loader - " + gKey + " could not be provided by the dictionary. (Doesn't exists in the Resources/UI ?)");
            }
		}

		return new Sprite();
	}

    public GameObject GetObject(string gKey)
    {
        if (m_dicLoadedObjects.ContainsKey(gKey))
        {
            return m_dicLoadedObjects[gKey];
        }
        else
        {
            Debug.LogError("Resource Loader - " + gKey + " could not be provided by the dictionary. (Doesn't exists in the Resources/UI ?)");
        }

        return null;
    }

    public GameObject GetRecycledObject(string gKey)
    {
        if (m_dicLoadedObjects.ContainsKey(gKey))
        {
            return recycleObject(gKey);
        }
        else
        {
            Debug.LogError("Resource Loader - " + gKey + " could not be provided by the dictionary. (Doesn't exists in the Resources/Objects ?)");
        }

        return null;
    }

    public AudioClip  GetClip(string gKey)
    {
        if (m_dicLoadedClips.ContainsKey(gKey))
        {
            return m_dicLoadedClips[gKey];
        }
        else
        {
            Debug.LogError("Resource Loader - " + gKey + " could not be provided by the dictionary. (Doesn't exists in the Resources/Audio ?)");
        }

        return null;
    }

    public void ReloadResources()
	{
		if(debugMode)
		{
			print ("Resource Loader - Reloading...");
		}

		m_dicLoadedSprites.Clear();
        m_dicLoadedObjects.Clear();
        m_dicLoadedClips.Clear();
        LoadResources();
	}

	public void LoadResources()
	{
        StartCoroutine(LoadResourcesRoutine());
    }

	public IEnumerator GetImageFromURL(Image gImg,string gUrl)
	{
		WWW wwwRequest = new WWW(gUrl);
		
		yield return wwwRequest;

		//Server check
		if (wwwRequest.error != null || wwwRequest.texture.width == 8)
		{
			Debug.LogError(gUrl+" - had a request problem...");
			Debug.LogError(wwwRequest.error);

			if(debugMode)
			{
				print ("Resource Loader - "+gUrl+" fell back to a local file...");
			}

			string croppedName = "";
			int iFrom=gUrl.Length-1;
			int iTo = 0;

			//FALLBACK
			while(iFrom > 0)
			{
				if(gUrl[iFrom]=='.')
				{
					iTo = iFrom;
				}

				if(gUrl[iFrom]=='/')
				{
					iFrom++;

					if(iTo!=0)
					{
						croppedName = gUrl.Substring(iFrom,(iTo-iFrom));
					}
					else
					{
						croppedName = gUrl.Substring(iFrom,(gUrl.Length-4)-iFrom);
					}

					if(debugMode)
					{
						print ("Resource Loader - had gathered the file name "+croppedName+" from "+gUrl);
					}

					break;
				}

				iFrom--;
			}

			if(croppedName!="")
			{
				if(GetSprite(croppedName)!=null)
				{
					gImg.sprite = GetSprite(croppedName);
				}
				else
				{
					if(debugMode)
					{
						print ("Resource Loader - "+croppedName+" does not exist as a local file...");
						gImg.sprite = GetSprite("error");
					}
				}
			}
			else
			{
				if(debugMode)
				{
					print ("Resource Loader - "+gUrl+" could not be broken apart correctly.");
					gImg.sprite = GetSprite("error");
				}
			}


		}
		else
		{
			Rect tempRect = new Rect(0,0,wwwRequest.texture.width,wwwRequest.texture.height);
			gImg.sprite = Sprite.Create(wwwRequest.texture ,tempRect , new Vector2(wwwRequest.texture.width/2,wwwRequest.texture.height/2));
		}
	}

    public void ClearObjectPool()
    {
        m_listObjectPool.Clear();
    }

    #endregion

    #region Internal Methods

    protected IEnumerator LoadResourcesRoutine()
    {
        LoadingWindowUI.Instance.Register(this);
        
        float lastMiliSec = 0;

        m_bLoading = true;


        yield return 0;

        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "***");
            lastMiliSec = Time.time;
            Debug.Log("Resource Loader - Initializing Resources...");
        }

        Sprite[] loadedSprite = Resources.LoadAll<Sprite>("UI");

        yield return 0;

        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "*** UI");
            lastMiliSec = Time.time;
            Debug.Log("Resource Loader - Loaded " + (loadedSprite.Length + 1) + " resources.");
        }

        for(int i=0; i<loadedSprite.Length; i++)
        {
            m_dicLoadedSprites.Add(loadedSprite[i].name, loadedSprite[i]);

            if (debugMode)
            {
                Debug.Log("Resource Loader - " + loadedSprite[i].name + " - Has been loaded.");
            }

            if(i % 10 == 0)
            {
                yield return 0;
            }
        }

        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "***IMP");
            lastMiliSec = Time.time;
        }

        GameObject[] loadedObject = Resources.LoadAll<GameObject>("Objects");

        yield return 0;

        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "***OBJ");
            lastMiliSec = Time.time;
            Debug.Log("Resource Loader - Loaded " + (loadedObject.Length + 1) + " resources.");
        }

        for (int i = 0; i < loadedObject.Length; i++)
        {

            m_dicLoadedObjects.Add(loadedObject[i].name, loadedObject[i]);

            if (debugMode)
            {
                Debug.Log("Resource Loader - " + loadedObject[i].name + " - Has been loaded.");
            }

            if (i % 10 == 0)
            {
                yield return 0;
            }
        }


        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "***IMP");
            lastMiliSec = Time.time;
        }



        AudioClip[] loadedClip = Resources.LoadAll<AudioClip>("Audio");

        yield return 0;

        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "***AUDIO");
            lastMiliSec = Time.time;
        }


        if (debugMode)
        {
            Debug.Log("Resource Loader - Loaded " + (loadedClip.Length + 1) + " resources.");
        }

        for(int i=0; i<loadedClip.Length; i++)
        {

            m_dicLoadedClips.Add(loadedClip[i].name, loadedClip[i]);

            if (debugMode)
            {
                Debug.Log("Resource Loader - " + loadedClip[i].name + " - Has been loaded.");
            }

            if (i % 10 == 0)
            {
                yield return 0;
            }
        }


        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "***IMP");
            lastMiliSec = Time.time;
            Debug.Log(this + " - Has finished loading resources...");
        }

        yield return StartCoroutine(LoadSceneObjectsRoutine());

        yield return 0;

        if (debugMode)
        {
            Debug.Log((Time.time - lastMiliSec) + "***SCENE OBJS PRELOAD");
            lastMiliSec = Time.time;
        }

        m_bLoading = false;

        if (debugMode)
        {
            Debug.Log((Time.time) + "***WHOLE PROCCESS");
        }

        LoadingWindowUI.Instance.Leave(this);

        RunResourcesLoadedCallbacks();
    }

    protected List<Action> m_resourcesLoadedCallbacks = new List<Action>();
    public void RunWhenResourcesLoaded(Action callback)
    {
        if (m_bLoading)
        {
            m_resourcesLoadedCallbacks.Add(callback);
        }
        else 
        {
            callback();
        }
    }

    protected void RunResourcesLoadedCallbacks()
    {  
        foreach (Action callback in m_resourcesLoadedCallbacks)
        {
            callback();
        }
        m_resourcesLoadedCallbacks.Clear();
    }

    protected IEnumerator LoadSceneObjectsRoutine()
    {

        if (debugMode)
        {
            print("Resource Loader - Initializing Scene Objects...");
        }

        foreach(GameObject obj in m_listPreloadObjects)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return 0;

        foreach (GameObject obj in m_listPreloadObjects)
        {
            obj.SetActive(false);

            if (debugMode)
            {
                print(obj + " - Was preloaded...");
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (debugMode)
        {
            print(this + " - Has finished loading scene objects...");
        }

    }

    protected List<GameObject> m_listObjectPool = new List<GameObject>();

    protected GameObject recycleObject(string gKey)
    {
        GameObject tempObj = null;

        List<int> objectIndexesToRemove = new List<int>();
        for(int i=0;i<m_listObjectPool.Count;i++)
        {
            if (m_listObjectPool[i] == null) 
            {
                // if an object was destroyed, it will equal to null
                if (debugMode)
                {
                    Debug.Log("Object destroyed " + gKey);
                }
                objectIndexesToRemove.Add(i);
            } 
            else if (!m_listObjectPool[i].activeInHierarchy && m_listObjectPool[i].name == gKey)
            {
                tempObj = m_listObjectPool[i];

                if (debugMode)
                {
                    Debug.Log("Recycled " + gKey);
                }

                break;
            }
        }

        for (int i = objectIndexesToRemove.Count - 1; i >= 0; i--)
        {
            int indexToRemove = objectIndexesToRemove[i];
            m_listObjectPool.RemoveAt(indexToRemove);
        }

        if(tempObj == null)
        {
            tempObj = (GameObject)Instantiate(m_dicLoadedObjects[gKey]);

            if(debugMode)
            {
                Debug.Log("Created a new " + gKey);
            }
        }

        tempObj.name = gKey;
        tempObj.SetActive(true);
        m_listObjectPool.Add(tempObj);

        return tempObj;
    }

    
    #endregion

}
