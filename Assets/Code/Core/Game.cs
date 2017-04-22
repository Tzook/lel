using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
//using SimpleJSON;

public class Game : MonoBehaviour {

    public SceneControl CurrentScene;
    public bool InGame { protected set; get; }
    public bool InChat = false;
    public bool DraggingWindow = false;
    public bool MovingTroughPortal = false;
    public bool isBitch = false;
    public static Game Instance;

    public GameObject ClientCharacter;

    public List<GameObject> DontDestroyMeOnLoadList = new List<GameObject>();

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

            InGameMainMenuUI.Instance.HideGameUI();

            ResourcesLoader.Instance.ClearObjectPool();
            SocketClient.Instance.Disconnect();

            ResetUndestroyables();

            SceneManager.LoadScene("MainMenu");
        }
    }

    private void ResetUndestroyables()
    {
        while(DontDestroyMeOnLoadList.Count > 0)
        {
            Destroy(DontDestroyMeOnLoadList[0]);
            DontDestroyMeOnLoadList.RemoveAt(0);
        }

        Destroy(this.gameObject);
    }

    #region Chat

    public void ReceiveChatMessage(string actorID, string message)
    {
        ActorInfo actorInfo = CurrentScene.GetActor(actorID);

        if (actorInfo != null && actorInfo.Instance)
        {
            actorInfo.Instance.ChatBubble(message);
            ChatlogUI.Instance.AddMessage(actorInfo, message);
            InGameMainMenuUI.Instance.SetLastChatMessage(message);
            InGameMainMenuUI.Instance.SetLastChatMessage(actorInfo.Name + " : \"" + message + "\"");
        }
        else
        {
            Debug.LogError(actorID + " is no longer in the room for this event to occur.");
        }
    }

    public void ReceiveWhisper(string name, string message)
    {
        ChatlogUI.Instance.AddWhisperFrom(name, message);
    }

    public void ReceiveWhisperFail(string name)
    {
        ChatlogUI.Instance.AddWhisperFail(name);
    }

    public void SendChatMessage(string givenText)
    {
        SocketClient.Instance.SendChatMessage(givenText);
        ActorInstance actor = ClientCharacter.GetComponent<ActorInstance>();
        actor.ChatBubble(givenText);
        ChatlogUI.Instance.AddMessage(actor.Info, givenText);
        InGameMainMenuUI.Instance.SetLastChatMessage(actor.Info.Name + " : \""+givenText+"\"");
    }

    public void SendWhisper(string givenText, string targetName)
    {
        SocketClient.Instance.SendWhisper(givenText, targetName);
        ChatlogUI.Instance.AddWhisperTo(targetName, givenText);
    }

    #endregion

    #region Items

    public void MoveInventoryItem(int from, int to)
    {
        ClientCharacter.GetComponent<ActorInstance>().Info.Inventory.SwapSlots(from, to);
        InGameMainMenuUI.Instance.RefreshInventory();
    }

    public void DeleteInventoryItem(int slot)
    {
        ClientCharacter.GetComponent<ActorInstance>().Info.Inventory.RemoveItem(slot);
        InGameMainMenuUI.Instance.RefreshInventory();
    }

    public void ActorEquippedItem(string actorID, int inventoryIndex, string equipSlot, ItemInfo item)
    {
        ActorInfo actor = CurrentScene.GetActor(actorID);
        ItemInfo previouslyEquipped = actor.Equipment.GetItem(equipSlot);

        actor.Equipment.SetItem(equipSlot, item);

        if (actor == CurrentScene.ClientCharacter)
        {
            if (previouslyEquipped != null)
            {
                ClientCharacter.GetComponent<ActorInstance>().Info.Inventory.Content[inventoryIndex] = previouslyEquipped;
            }
            else
            {
                ClientCharacter.GetComponent<ActorInstance>().Info.Inventory.RemoveItem(inventoryIndex);
            }


            InGameMainMenuUI.Instance.RefreshEquipment();
            InGameMainMenuUI.Instance.RefreshInventory();
        }

        actor.Instance.UpdateVisual();
    }

    public void ActorUnequippedItem(string actorID, string equipSlot, int inventoryIndex, ItemInfo item)
    {
        ActorInfo actor = CurrentScene.GetActor(actorID);
        ItemInfo itemToAdd = actor.Equipment.GetItem(equipSlot);

        actor.Equipment.SetItem(equipSlot, item);

        if (actor == CurrentScene.ClientCharacter)
        {
            ClientCharacter.GetComponent<ActorInstance>().Info.Inventory.AddItemAt(inventoryIndex, itemToAdd);


            InGameMainMenuUI.Instance.RefreshEquipment();
            InGameMainMenuUI.Instance.RefreshInventory();
        }

        actor.Instance.UpdateVisual();
    }

    public void DeleteEquip(string actorID, string equipSlot)
    {
        ActorInfo actor = CurrentScene.GetActor(actorID);

        if (actor == CurrentScene.ClientCharacter)
        {
            ClientCharacter.GetComponent<ActorInstance>().Info.Equipment.SetItem(equipSlot, null);

            InGameMainMenuUI.Instance.RefreshEquipment();
        }

        actor.Instance.UpdateVisual();
    }

    public void ActorEmoted(string id, string type, string emote)
    {
        if (type == "eyes")
        {
            CurrentScene.GetActor(id).Instance.PlayEyesEmote(emote);
        }
        else if (type == "mouth")
        {
            CurrentScene.GetActor(id).Instance.PlayMouthEmote(emote);
        }

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

    public ItemInstance SpawnItem(ItemInfo info, string instanceID, float x, float y)
    {
        AudioControl.Instance.Play("impact");

        ItemInstance itemInstance = ResourcesLoader.Instance.GetRecycledObject("ItemInstance").GetComponent<ItemInstance>();
        itemInstance.transform.position = new Vector3(x, y, 0f);
        itemInstance.SetInfo(info, instanceID);

        CurrentScene.Items.Add(instanceID, itemInstance);

        return itemInstance;
    }

    public void SpawnItems(List<ItemInfo> infos, List<string> ids, float x, float y)
    {
        StartCoroutine(SpawnItemsRoutine(infos, ids, x, y));
    }

    public void SpawnMonster(string instanceID, float xPos, float yPos, string mobKey, int currentHP)
    {
        GameObject tempMob = ResourcesLoader.Instance.GetRecycledObject(mobKey);
        tempMob.transform.position = new Vector3(xPos, yPos, 0f);

        tempMob.GetComponent<Enemy>().Initialize(instanceID, currentHP);

        ResourcesLoader.Instance.GetRecycledObject("SpawnParticles").transform.position = tempMob.transform.position;

        if(isBitch)
        {
            tempMob.GetComponent<Enemy>().SetAION();
        }
        else
        {
            tempMob.GetComponent<Enemy>().SetAIOFF();
        }
    }

    public void LoadNpcCharacter(ActorInfo info)
    {
        GameObject tempObj = SpawnPlayer(info);

        tempObj.GetComponent<ActorMovement>().enabled = true;
        tempObj.GetComponent<ActorController>().enabled = false;
        tempObj.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void RemoveNpcCharacter(string id)
    {
        GameObject leavingPlayer = CurrentScene.GetActor(id).Instance.gameObject;

        leavingPlayer.SetActive(false);

        CurrentScene.Leave(id);
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

        InGameMainMenuUI.Instance.ShowGameUI();

        GameCamera.Instance.InstantFocusCamera();
        yield return StartCoroutine(InGameMainMenuUI.Instance.FadeOutRoutine());
    }

    public void SetBitch(bool isbitch)
    {
        if(this.isBitch && !isbitch) // WAS SET ON
        {
            foreach (Enemy enemy in CurrentScene.Enemies)
            {
                enemy.SetAIOFF();
            }
        }
        else if(!this.isBitch && isbitch) // WAS SET OFF
        {
            foreach (Enemy enemy in CurrentScene.Enemies)
            {
                enemy.SetAION();
            }
        }

        this.isBitch = isbitch;

    }

    private IEnumerator SpawnItemsRoutine(List<ItemInfo> infos, List<string> ids, float x, float y)
    {
        SpawnItem(infos[0], ids[0], x, y);

        yield return 0;

        Rigidbody2D tempRigid;
        bool ThrowRight = true;

        List<ItemInstance> ItemInstances = new List<ItemInstance>();

        for(int i=1;i<infos.Count;i++)
        {
            ThrowRight = !ThrowRight;

            tempRigid = SpawnItem(infos[i], ids[i], x, y).GetComponent<Rigidbody2D>();
            ItemInstances.Add(tempRigid.GetComponent<ItemInstance>());

            if(ThrowRight)
            {
                tempRigid.AddForce(new Vector2(1f*i,4f), ForceMode2D.Impulse);
            }
            else
            {
                tempRigid.AddForce(new Vector2(-1f * i, 4f), ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (isBitch)
        {
            yield return new WaitForSeconds(2f);

            SocketClient.Instance.SendItemPositions(ItemInstances);
        }
    }

    public static Vector3 SplineLerp(Vector3 source, Vector3 target, float Height, float t)
    {
        Vector3 ST = new Vector3(source.x , source.y + Height, source.z);
        Vector3 TT = new Vector3(target.x , target.y + Height, target.z);

        Vector3 STTTM = Vector3.Lerp(ST, TT, t);

        Vector3 STM = Vector3.Lerp(source, ST, t);
        Vector3 TTM = Vector3.Lerp(TT, target, t);

        Vector3 SplineST = Vector3.Lerp(STM, STTTM, t);
        Vector3 SplineTM = Vector3.Lerp(STTTM, TTM, t);

        return Vector3.Lerp(SplineST, SplineTM, t);
    }


    #region Debug

    //[SerializeField]
    //bool Test;
    //void Update()
    //{
    //    if(Test)
    //    {
    //        Test = false;

    //        List<ItemInfo> infos = new List<ItemInfo>();
    //        List<string> ids = new List<string>();

    //        for(int i=0;i<6;i++)
    //        {
    //            infos.Add(new ItemInfo(JSON.Parse("{\"sprites\":{\"hip\":\"testingclothing_greenPants_male_hip\", \"knee\":\"testingclothing_greenPants_male_knee\"}, \"name\":\"Green Pants\", \"icon\":\"testingclothing_greenPants_male_hip\", \"type\":\"legs\"}")));
    //            ids.Add(i.ToString());
    //        }

    //        SpawnItems(infos, ids, ClientCharacter.transform.position.x, ClientCharacter.transform.position.y);

    //    }
    //}

    #endregion

}

