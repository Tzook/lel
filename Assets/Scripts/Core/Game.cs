using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
//using SimpleJSON;

public class Game : MonoBehaviour {

    public SceneControl CurrentScene;
    public bool InGame { protected set; get; }
    public bool InChat = false;
    public bool IsAlive = true;
    public bool IsChattingWithNpc = false;
    
    public bool CanUseUI
    {
        get 
        {
            return IsAlive && !IsChattingWithNpc;
        }
    }
    public bool isDraggingWindow = false;
    public bool isClickingOnUI = false;
    public bool isInteractingWithUI
    {
        get
        {
            return isDraggingWindow
                    || isClickingOnUI
                    || InGameMainMenuUI.Instance.isDraggingItem
                    || InGameMainMenuUI.Instance.isFullScreenWindowOpen;
        }
    }
    public bool MovingTroughPortal = false;
    public bool isBitch = false;
    public bool isLoadingScene = false;
    bool isEnteringWorld = false;

    public static Game Instance;

    public GameObject ClientCharacter;

    public List<GameObject> DontDestroyMeOnLoadList = new List<GameObject>();

    //Custom quests routines
    List<OkRoutineInstance> OkRoutineInstances = new List<OkRoutineInstance>();

    void Awake()
    {
        Instance = this;
        Application.runInBackground = true;
        #if UNITY_WEBGL
        #else
        Application.targetFrameRate = 60;
        #endif
    }

    public void EnterGameWorld()
    {
        SocketClient.Instance.ConnectToGame();
        isEnteringWorld = true;
    }

    public void LoadScene(string scene, ActorInfo actorInfo)
    {
        StartCoroutine(LoadSceneRoutine(scene, actorInfo));
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

    public void Focus()
    {
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(GetComponent<GameObject>(), null);
        }
    }

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

        if (actor == LocalUserInfo.Me.ClientCharacter)
        {
            if (previouslyEquipped != null)
            {
                ClientCharacter.GetComponent<ActorInstance>().Info.Inventory.ContentArray[inventoryIndex] = previouslyEquipped;
            }
            else
            {
                ClientCharacter.GetComponent<ActorInstance>().Info.Inventory.RemoveItem(inventoryIndex);
            }

            actor.ValidatePrimaryAbility();

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
            InGameMainMenuUI.Instance.RefreshStats();
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
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_male"));
        }
        else
        {
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_female"));
        }


        tempObj.GetComponent<ActorInstance>().UpdateVisual(info);
        tempObj.transform.position = info.LastPosition;
        return tempObj;

    }

    public ItemInstance SpawnItem(ItemInfo info, string instanceID, string owner, float x, float y)
    {
        ItemInstance itemInstance = ResourcesLoader.Instance.GetRecycledObject("ItemInstance").GetComponent<ItemInstance>();
        itemInstance.transform.position = new Vector3(x, y, 0f);
        itemInstance.SetInfo(info, instanceID, owner);

        AudioControl.Instance.PlayInPosition("impact", itemInstance.transform.position);

        CurrentScene.Items.Add(instanceID, itemInstance);

        return itemInstance;
    }

    public void SpawnItems(List<ItemInfo> infos, List<string> ids, List<string> owners, float x, float y)
    {
        StartCoroutine(SpawnItemsRoutine(infos, ids, owners, x, y));
    }

    public void SpawnMonster(string instanceID, float xPos, float yPos, string mobKey, int currentHP)
    {
        GameObject tempMob = ResourcesLoader.Instance.GetRecycledObject(mobKey);
        tempMob.transform.position = new Vector3(xPos, yPos, 0f);

        tempMob.GetComponent<Enemy>().Initialize(instanceID, Content.Instance.GetMonster(mobKey) , currentHP);

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

    public void LoadOtherPlayerCharacter(ActorInfo info)
    {
        GameObject tempObj = SpawnPlayer(info);
        if (info.CurrentHealth == 0) {
            tempObj.GetComponent<ActorInstance>().Death();
        }

        tempObj.GetComponent<ActorMovement>().enabled = true;
        tempObj.GetComponent<ActorController>().enabled = false;
        tempObj.GetComponent<PlayerShortcuts>().isMe = false;
        tempObj.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void RemovePlayerCharacter()
    {
        Destroy(ClientCharacter);

        CurrentScene.Leave(ClientCharacter.GetComponent<ActorInstance>().Info.ID);

        ClientCharacter = null;
    }

    public void RemoveAllSceneEntityInstances()
    {
        while(CurrentScene.ActorCount > 0)
        {
            RemoveOtherPlayerCharacter(CurrentScene.GetActorByIndex(0).ID);
        }

        while(CurrentScene.EnemyCount > 0)
        {
            RemoveEnemy(CurrentScene.GetEnemyByIndex(0).Info.ID);
        }
    }

    public void RemoveOtherPlayerCharacter(string id)
    {
        GameObject leavingPlayer = CurrentScene.GetActor(id).Instance.gameObject;

        Destroy(leavingPlayer);

        CurrentScene.Leave(id);
    }

    public void RemoveEnemy(string id)
    {
        CurrentScene.GetEnemy(id).gameObject.SetActive(false);
        CurrentScene.RemoveSceneEnemy(CurrentScene.GetEnemy(id));
    }

    public void LoadPlayerCharacter(ActorInfo actorInfo)
    {
        ClientCharacter = SpawnPlayer(LocalUserInfo.Me.ClientCharacter);
        
        ActorController actorController = ClientCharacter.GetComponent<ActorController>();
        if (actorInfo.CurrentHealth == 0) {
            actorController.Death();
            InGameMainMenuUI.Instance.ShowDeathWindow();
        }

        ClientCharacter.GetComponent<ActorMovement>().enabled = false;
        actorController.enabled = true;
        ClientCharacter.GetComponent<PlayerShortcuts>().isMe = true;
        ClientCharacter.GetComponent<Rigidbody2D>().isKinematic = false;

        if(isEnteringWorld)
        {
            InitializeOkRoutines();

            isEnteringWorld = false;
        }
    }

    protected IEnumerator LoadSceneRoutine(string scene, ActorInfo actorInfo)
    {
        isLoadingScene = true;
        LocalUserInfo.Me.ClientCharacter = actorInfo;

        yield return InGameMainMenuUI.Instance.StartFadeCoroutine(InGameMainMenuUI.Instance.FadeInRoutine());

        string lastScene = SceneManager.GetActiveScene().name;

        ResourcesLoader.Instance.ClearObjectPool();

        CurrentScene = null;

        SceneManager.LoadScene(scene);

        yield return 0;

        while (!SceneManager.GetSceneByName(scene).isLoaded)
        {
            yield return 0;
        }

        CurrentScene = new SceneControl();

        SocketClient.Instance.EmitLoadedScene(actorInfo);

        while(GameCamera.Instance==null)
        {
            yield return 0;
        }

        for (int i = 0; i < SceneInfo.Instance.RoomAbilities.Count; i++)
        {
            actorInfo.AddRoomPrimaryAbility(SceneInfo.Instance.RoomAbilities[i]);
        }

        GameCamera.Instance.Register(CurrentScene.ClientCharacter.Instance.gameObject);

        
        InGameMainMenuUI.Instance.ShowGameUI();

        CurrentScene.UpdateAllQuestProgress();

        AudioControl.Instance.SetMusic(SceneInfo.Instance.BGMusic);

        GameCamera.Instance.InstantFocusCamera();

        MovingTroughPortal = false;
        InGame = true;
        yield return InGameMainMenuUI.Instance.StartFadeCoroutine(InGameMainMenuUI.Instance.FadeOutRoutine());

        isLoadingScene = false;
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

    private IEnumerator SpawnItemsRoutine(List<ItemInfo> infos, List<string> ids, List<string> owners, float x, float y)
    {
        float side = 1f;

        for(int i = 0; i < infos.Count; i++)
        {
            side *= -1;

            Rigidbody2D tempRigid = SpawnItem(infos[i], ids[i], owners[i], x, y + 0.1f).GetComponent<Rigidbody2D>();

            float sideOffset = side * ((i + 1) / 2); // round down so both side have equal distances
            tempRigid.AddForce(new Vector2(sideOffset, 6f), ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);
        // we have an item that moves to the side - we want to update its location once it lands
        if (isBitch && infos.Count > 1) 
        {
            List<ItemInstance> itemInstances = new List<ItemInstance>();
            for(int i = 1; i < ids.Count; i++)
            {
                if (CurrentScene.Items.ContainsKey(ids[i])) 
                {
                    itemInstances.Add(CurrentScene.Items[ids[i]]);
                }
            }

            if (itemInstances.Count > 0) 
            {
                SocketClient.Instance.SendItemPositions(itemInstances);
            }

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

    public IEnumerator RemoveClickingFlagWhenDone()
    {
        while (Input.GetMouseButton(0))
        {
            yield return 0;
        }
        isClickingOnUI = false;
    }

    
    void InitializeOkRoutines()
    {
        for(int i=0; i < LocalUserInfo.Me.ClientCharacter.QuestsInProgress.Count; i++)
        {
            HandleOkRoutines(LocalUserInfo.Me.ClientCharacter.QuestsInProgress[i]);
        }
    }

    public void HandleOkRoutines(string QuestKey)
    {
        HandleOkRoutines(Content.Instance.GetQuest(QuestKey));
    }

    public void HandleOkRoutines(Quest quest)
    {
        OkRoutineInstance tempInstance;

        for (int i = 0; i < quest.Conditions.Count; i++)
        {
            if (quest.Conditions[i].Condition == "ok")
            {
                switch (quest.Conditions[i].Type)
                {
                    case "CosmoFinished":
                        {
                            tempInstance = new OkRoutineInstance(quest.Conditions[i].Type, StartCoroutine(CustomQuestWait(quest.Key, quest.Conditions[i].Type, 60f)));
                            OkRoutineInstances.Add(tempInstance);
                            break;
                        }
                }
            }
        }
    }

    public void HandleAbandonOkRoutines(string abandonedQuestKey)
    {
        Quest quest = Content.Instance.GetQuest(abandonedQuestKey);

        for(int i=0; i < quest.Conditions.Count; i++)
        {
            if (quest.Conditions[i].Condition == "ok")
            {
                for (int c = 0; c < OkRoutineInstances.Count; c++)
                {
                    if(OkRoutineInstances[c].okKey == quest.Conditions[i].Type)
                    {
                        StopCoroutine(OkRoutineInstances[c].RoutineInstnace);
                        OkRoutineInstances.RemoveAt(c);
                        break;
                    }
                }
            }
        }
    }

    IEnumerator CustomQuestWait(string questKey, string okKey, float Wait)
    {
        yield return new WaitForSeconds(Wait);

        for(int i=0;i<OkRoutineInstances.Count;i++)
        {
            if(OkRoutineInstances[i].okKey == okKey)
            {
                OkRoutineInstances.RemoveAt(i);
                break;
            }
        }

        SocketClient.Instance.SendQuestOK(okKey);
    }

}

class OkRoutineInstance
{
    public string okKey;
    public Coroutine RoutineInstnace;

    public OkRoutineInstance(string key, Coroutine routine)
    {
        this.okKey = key;
        this.RoutineInstnace = routine;
    }
}

