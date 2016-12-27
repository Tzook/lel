using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    public SceneControl CurrentScene;
    public bool InGame { protected set; get; }
    public bool InChat;
    public bool MovingTroughPortal;
    public static Game Instance;

    public GameObject ClientCharacter;

    public List<DontDestroyMeOnLoad> UndestroyedObjects = new List<DontDestroyMeOnLoad>();

    void Awake()
    {
        Instance = this;
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
    }

    public void LoadScene(string scene, string fromScene = "")
    {
        StartCoroutine(LoadSceneRoutine(scene, fromScene));
    }

    public void LeaveToMainMenu()
    {
        if (InGame)
        {
            InGame = false;
            
            ResourcesLoader.Instance.ClearObjectPool();
            SocketClient.Instance.Diconnect();
            SceneManager.LoadScene("MainMenu");
        }
    }

    #region Chat

    public void ReceiveChatMessage(string id, string message, string type)
    {
        ActorInfo actorInfo = CurrentScene.GetActor(id);
        if (actorInfo != null && actorInfo.Instance)
        {
            actorInfo.Instance.ChatBubble(message);
            ChatlogUI.Instance.AddMessage(actorInfo, message);
        }
        else
        {
            Debug.LogError(id + " is no longer in the room for this event to occur.");
        }
    }
    public void SendChatMessage(string givenText)
    {
        SocketClient.Instance.SendChatMessage(givenText);
        ActorInstance actor = ClientCharacter.GetComponent<ActorInstance>();
        actor.ChatBubble(givenText);
        ChatlogUI.Instance.AddMessage(actor.Info, givenText);
    }

    #endregion

    public GameObject SpawnPlayer(ActorInfo info)
    {
        CurrentScene.Join(info);

        GameObject tempObj;
        if (info.Gender == Gender.Male)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("actor_male");
        }
        else
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("actor_female");
        }


        tempObj.GetComponent<ActorInstance>().UpdateVisual(info);
        tempObj.transform.position = info.LastPosition;
        return tempObj;

    }

    public void SpawnItem(ItemInfo info, string instanceID, float x, float y)
    {
        ItemInstance itemInstance = ResourcesLoader.Instance.GetRecycledObject("ItemInstance").GetComponent<ItemInstance>();
        itemInstance.transform.position = new Vector3(x, y, 0f);
        itemInstance.SetInfo(info);

        CurrentScene.Items.Add(instanceID, itemInstance);
    }

    public void LoadNpcCharacter(ActorInfo info)
    {
        GameObject tempObj = SpawnPlayer(info);

        tempObj.GetComponent<ActorMovement>().enabled = true;
        tempObj.GetComponent<ActorController>().enabled = false;
        tempObj.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void RemoveNpcCharacter(ActorInfo info)
    {
        GameObject leavingPlayer = CurrentScene.GetActor(info.ID).Instance.gameObject;

        leavingPlayer.SetActive(false);

        CurrentScene.Leave(info);
    }

    public void LoadPlayerCharacter(string fromScene = "")
    {
        ClientCharacter = SpawnPlayer(LocalUserInfo.Me.SelectedCharacter);

        ClientCharacter.GetComponent<ActorMovement>().enabled = false;
        ClientCharacter.GetComponent<ActorController>().enabled = true;
        ClientCharacter.GetComponent<Rigidbody2D>().isKinematic = false;

        if(!string.IsNullOrEmpty(fromScene))
        {
            Debug.Log(fromScene);
            ClientCharacter.transform.position = CurrentScene.GetPortal(fromScene).transform.position;
        }
    }

    protected IEnumerator LoadSceneRoutine(string scene, string fromScene = "")
    {
        yield return StartCoroutine(InGameMainMenuUI.Instance.FadeInRoutine());

        string lastScene = SceneManager.GetActiveScene().name;
        ResourcesLoader.Instance.ClearObjectPool();
        SocketClient.Instance.DisposeSubscriptions();

        CurrentScene = null;
        SceneManager.LoadScene(scene);

        while(lastScene == SceneManager.GetActiveScene().name)
        {
            yield return 0;
        }

        CurrentScene = new SceneControl();

        //TODO This yield is here because the gate portals wait for hte SceneControl to initalize, 
        // - then a frame later they set themselves at it and then a frame after that they are registered.
        // -- Without any portal gates the spawning in a new scene will recieve an exception (No gate to be placed on)
        // --- Seems unreasonable so find a better way to figure spawn location (XYZ).
        if (!string.IsNullOrEmpty(fromScene))
        {
            while (CurrentScene.GetPortal(fromScene) == null)
            {
                yield return 0;
            }
        }

        SocketClient.Instance.EmitLoadedScene(fromScene);

        while(GameCamera.Instance==null)
        {
            yield return 0;
        }

        GameCamera.Instance.Register(CurrentScene.ClientCharacter.Instance.gameObject);

        MovingTroughPortal = false;
        InGame = true;

        GameCamera.Instance.InstantFocusCamera();
        yield return StartCoroutine(InGameMainMenuUI.Instance.FadeOutRoutine());
    }

    public static Vector3 SplineLerp(Vector3 source, Vector3 target, float Height, float t)
    {
        Vector3 ST = new Vector3(source.x + Height, source.y, source.z);
        Vector3 TT = new Vector3(target.x + Height, target.y, target.z);

        Vector3 STTTM = Vector3.Lerp(ST, TT, t);

        Vector3 STM = Vector3.Lerp(source, ST, t);
        Vector3 TTM = Vector3.Lerp(TT, target, t);

        Vector3 SplineST = Vector3.Lerp(STM, STTTM, t);
        Vector3 SplineTM = Vector3.Lerp(STTTM, TTM, t);

        return Vector3.Lerp(SplineST, SplineTM, t);
    }
}