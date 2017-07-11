﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
//using SimpleJSON;

public class Game : MonoBehaviour {

    public SceneControl CurrentScene;
    public bool InGame { protected set; get; }
    public bool InChat = false;
    public bool CanUseUI = true;
    public bool isDraggingWindow = false;
    public bool isInteractingWithUI
    {
        get
        {
            return isDraggingWindow
                    || InGameMainMenuUI.Instance.isDraggingItem
                    || InGameMainMenuUI.Instance.isFullScreenWindowOpen;
        }
    }
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
            InGameMainMenuUI.Instance.RefreshStats();
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
            InGameMainMenuUI.Instance.RefreshStats();
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

    public void RemoveAllOtherPlayerCharacters()
    {
        for (int i = 0; i < CurrentScene.ActorCount;i++)
        {
            RemoveOtherPlayerCharacter(CurrentScene.GetActorByIndex(i).ID);
        }
    }

    public void RemoveOtherPlayerCharacter(string id)
    {
        GameObject leavingPlayer = CurrentScene.GetActor(id).Instance.gameObject;

        Destroy(leavingPlayer);

        CurrentScene.Leave(id);
    }

    public void LoadPlayerCharacter(ActorInfo actorInfo)
    {
        LocalUserInfo.Me.ClientCharacter = actorInfo;
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
    }

    protected IEnumerator LoadSceneRoutine(string scene, ActorInfo actorInfo)
    {
        yield return StartCoroutine(InGameMainMenuUI.Instance.FadeInRoutine());

        string lastScene = SceneManager.GetActiveScene().name;

        if (lastScene != scene)
        {
            ResourcesLoader.Instance.ClearObjectPool();

            CurrentScene = null;

            SceneManager.LoadScene(scene);

            while (lastScene == SceneManager.GetActiveScene().name)
            {
                yield return 0;
            }

            CurrentScene = new SceneControl();

        }

        SocketClient.Instance.EmitLoadedScene(actorInfo);

        while(GameCamera.Instance==null)
        {
            yield return 0;
        }

        GameCamera.Instance.Register(CurrentScene.ClientCharacter.Instance.gameObject);


        InGameMainMenuUI.Instance.ShowGameUI();

        CurrentScene.UpdateAllQuestProgress();

        AudioControl.Instance.SetMusic(SceneInfo.Instance.BGMusic);

        GameCamera.Instance.InstantFocusCamera();
        yield return StartCoroutine(InGameMainMenuUI.Instance.FadeOutRoutine());

        MovingTroughPortal = false;
        InGame = true;
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
        Rigidbody2D tempRigid;

        tempRigid = SpawnItem(infos[0], ids[0], owners[0], x, y+0.1f).GetComponent<Rigidbody2D>();
        tempRigid.AddForce(new Vector2(1f, 6f), ForceMode2D.Impulse);

        yield return 0;

        bool ThrowRight = true;

        List<ItemInstance> ItemInstances = new List<ItemInstance>();

        for(int i=1;i<infos.Count;i++)
        {
            ThrowRight = !ThrowRight;

            tempRigid = SpawnItem(infos[i], ids[i], owners[i], x, y + 0.1f).GetComponent<Rigidbody2D>();
            ItemInstances.Add(tempRigid.GetComponent<ItemInstance>());

            float sideOffset = (i + 1) / 2; // round down so both side have equal distances
            if(ThrowRight)
            {
                tempRigid.AddForce(new Vector2(1f * sideOffset, 6f), ForceMode2D.Impulse);
            }
            else
            {
                tempRigid.AddForce(new Vector2(-1f * sideOffset, 6f), ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (isBitch && ItemInstances.Count > 0)
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

