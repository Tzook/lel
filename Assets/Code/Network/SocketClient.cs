using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using SimpleJSON;
using System;

public class SocketClient : MonoBehaviour
{

    #region Config
    public bool DebugMode = false;

    #endregion

    #region Essential

    protected WebSocketConnector webSocketConnector;

    protected Socket CurrentSocket;

    public static SocketClient Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        webSocketConnector = new WebSocketConnector();
    }

    #endregion

    #region Parameters

    protected Dictionary<string, IUpdatePositionListener> SubscribedMovables = new Dictionary<string, IUpdatePositionListener>();

    #endregion

    #region Public Methods

    public void ConnectToGame()
    {
        BroadcastEvent("Connecting to server..");
        CurrentSocket = webSocketConnector.connect(LocalUserInfo.Me.SelectedCharacter.ID);

        CurrentSocket.On("connect", OnConnect);
        CurrentSocket.On("disconnect", OnDisconnect);
        CurrentSocket.On("error", OnError);

        CurrentSocket.On("event_error", OnEventError);

        CurrentSocket.On("actor_join_room", OnActorJoinRoom);
        CurrentSocket.On("actor_leave_room", OnActorLeaveRoom);
        CurrentSocket.On("actor_move_room", OnMoveRoom);
        CurrentSocket.On("bitch_please", OnBitchPlease);
        CurrentSocket.On("actor_bitch", OnActorBitch);

        CurrentSocket.On("shout", OnShout);
        CurrentSocket.On("chat", OnChatMessage);
        CurrentSocket.On("whisper", OnWhisper);
        CurrentSocket.On("whisper_fail", OnWhisperFail);

        CurrentSocket.On("movement", OnMovement);

        CurrentSocket.On("actor_pick_item", OnActorPickItem);
        CurrentSocket.On("drop_item", OnActorDropItem);
        CurrentSocket.On("item_disappear", OnItemDisappear);
        CurrentSocket.On("actor_move_item", OnActorMoveItem);
        CurrentSocket.On("actor_delete_item", OnActorDeleteItem);

        CurrentSocket.On("actor_equip_item", OnActorEquipItem);
        CurrentSocket.On("actor_unequip_item", OnActorUnequipItem);
        CurrentSocket.On("actor_delete_equip", OnActorDeleteEquip);
        CurrentSocket.On("actor_moved_equip", OnActorMovedEquip);

        CurrentSocket.On("actor_emote", OnActorEmoted);

        CurrentSocket.On("actor_gain_hp", OnActorGainHP);
        CurrentSocket.On("actor_gain_mp", OnActorGainMP);
        CurrentSocket.On("actor_gain_exp", OnActorGainXP);
        CurrentSocket.On("actor_lvl_up", OnActorLevelUp);

        CurrentSocket.On("actor_take_dmg", OnActorTakeDMG);

        CurrentSocket.On("actor_load_attack", OnActorLoadAttack);
        CurrentSocket.On("actor_perform_attack", OnActorPreformAttack);

        CurrentSocket.On("mob_spawn", OnMobSpawn);
        CurrentSocket.On("mob_die", OnMobDeath);
        CurrentSocket.On("mob_take_dmg", OnMobTakeDamage);
        CurrentSocket.On("mob_move", OnMobMovement);


        LoadingWindowUI.Instance.Register(this);
    }


    public void Diconnect()
    {
        DisposeSubscriptions();
        CurrentSocket.Disconnect();
        CurrentSocket.Off();
    }

    public void Subscribe(string id, IUpdatePositionListener instance)
    {
        if (!SubscribedMovables.ContainsKey(id))
        {
            SubscribedMovables.Add(id, instance);
        }
        else
        {
            Debug.LogError(this + " | " + id + " attempted to subscribe more than once!");
        }
    }

    public void Unsubscribe(string id)
    {
        if (SubscribedMovables.ContainsKey(id))
        {
            SubscribedMovables.Remove(id);
        }
        else
        {
            Debug.LogError(this + " | " + id + " attempted to unsubscribe but was not subscribed from the first place!");
        }
    }

    public void DisposeSubscriptions()
    {
        SubscribedMovables.Clear();
    }

    #endregion

    #region Callbacks

    private void OnError(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("On error");
        WarningMessageUI.Instance.ShowMessage("An error occurred");
    }
    
    private void OnEventError(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("On event error: " + data.AsObject.ToString());
    }

    private void OnDisconnect(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("On disconnect");
    }

    protected void OnConnect(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("On connect");
        Game.Instance.LoadScene(LocalUserInfo.Me.SelectedCharacter.CurrentRoom);
    }

    protected void OnActorJoinRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Actor has joined the room");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.LoadNpcCharacter(new ActorInfo(data["character"]));
    }

    protected void OnActorLeaveRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Actor has left the room");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.RemoveNpcCharacter(data["id"]);
    }

    protected void OnMovement(Socket socket, Packet packet, params object[] args)
    {
        //BroadcastEvent("Movement occured");

        JSONNode data = (JSONNode)args[0];


        string id = data["id"];
        if (SubscribedMovables.ContainsKey(id))
        {
            IUpdatePositionListener instance = SubscribedMovables[id];
            instance.UpdateMovement(new Vector3(data["x"].AsFloat, data["y"].AsFloat, data["z"].AsFloat), data["angle"].AsFloat);
        }
        else
        {
            Debug.LogError(this + " | " + id + " received movement, but actor is not subscribed!");
        }
    }

    protected void OnChatMessage(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Chat Message!");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.ReceiveChatMessage(data["id"], data["msg"]);
    }

    protected void OnShout(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Shout Message!");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.ReceiveWhisper(data["name"], data["msg"]);
    }

    protected void OnWhisper(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Whisper Message!");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.ReceiveWhisper(data["name"], data["msg"]);
    }

    protected void OnWhisperFail(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Whisper Fail!");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.ReceiveWhisperFail(data["name"]);
    }

    protected void OnMoveRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Moved Room");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.LoadScene(data["to"], data["from"]);
    }

    protected void OnBitchPlease(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Bitch Please");

        JSONNode data = (JSONNode)args[0];
        SendBitchPlease(data["key"].Value);
    }

    protected void OnActorBitch(Socket socket, Packet packet, params object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Bitch "+ data["is_bitch"].AsBool);

        Game.Instance.SetBitch(data["is_bitch"].AsBool);
    }

    protected void OnItemDisappear(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Disappeared");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.CurrentScene.DestroySceneItem(data["item_id"].Value);
    }

    protected void OnActorDropItem(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Actor dropped an item");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.SpawnItem(new ItemInfo(data["item"]), data["item_id"].Value, data["x"].AsFloat, data["y"].AsFloat);

    }

    protected void OnActorPickItem(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent(data["id"].Value + " picked the item "+ data["item_id"].Value);

        Game.Instance.CurrentScene.GetActor(data["id"]).Instance.PickUpItem(data["item_id"]);
    }

    protected void OnActorMoveItem(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Moved");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.MoveInventoryItem(data["from"].AsInt, data["to"].AsInt);
    }

    protected void OnActorDeleteItem(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Deleted");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.DeleteInventoryItem(data["slot"].AsInt);
    }

    protected void OnActorEquipItem(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        ItemInfo swappedItem = null;

        if (!string.IsNullOrEmpty(data["equipped_item"]["name"]))
        {
            swappedItem = new ItemInfo(data["equipped_item"]);
        }

        BroadcastEvent(data["id"].Value+" Equipped Item ");

        Game.Instance.ActorEquippedItem(data["id"].Value, data["from"].AsInt, data["to"].Value, swappedItem);
    }

    protected void OnActorUnequipItem(Socket socket, Packet packet, object[] args)
    {

        JSONNode data = (JSONNode)args[0];

        ItemInfo swappedItem = null;

        if (!string.IsNullOrEmpty(data["equipped_item"]["name"]))
        {
            swappedItem = new ItemInfo(data["equipped_item"]);
        }

        BroadcastEvent(data["id"].Value + " Unequipped Item ");

        Game.Instance.ActorUnequippedItem(data["id"].Value, data["from"].Value, data["to"].AsInt, swappedItem);
    }

    protected void OnActorDeleteEquip(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Equip Deleted");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.DeleteEquip(data["id"].Value, data["slot"].Value);
    }

    protected void OnActorMovedEquip(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Equip Moved");
        JSONNode data = (JSONNode)args[0];

    }

    protected void OnActorEmoted(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Actor Emoted");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.ActorEmoted(data["id"].Value, data["type"].Value, data["emote"].Value);
    }

    protected void OnActorGainHP(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Gained " + data["hp"].AsInt + " HP");

        Game.Instance.CurrentScene.ClientCharacter.CurrentHealth = data["now"].AsInt;

        InGameMainMenuUI.Instance.RefreshHP();
    }

    protected void OnActorGainMP(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Gained " + data["mp"].AsInt + " MP");

        Game.Instance.CurrentScene.ClientCharacter.CurrentMana = data["now"].AsInt;

        InGameMainMenuUI.Instance.RefreshMP();
    }

    protected void OnActorGainXP(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Gained " + data["exp"].AsInt + " XP");

        Game.Instance.CurrentScene.ClientCharacter.EXP = data["now"].AsInt;

        InGameMainMenuUI.Instance.RefreshXP();
    }

    protected void OnActorLevelUp(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor Got Wounded");

        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);


        if (actor == Game.Instance.CurrentScene.ClientCharacter)
        {
            actor.SetStats(data["stats"]);
            InGameMainMenuUI.Instance.RefreshHP();
            InGameMainMenuUI.Instance.RefreshMP();
            InGameMainMenuUI.Instance.RefreshXP();
            InGameMainMenuUI.Instance.RefreshLevel();
        }

        actor.Instance.LevelUp();
    }

    protected void OnActorTakeDMG(Socket socket, Packet packet, object[] args)
    {

        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor Got Wounded");

        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);


        if (actor == Game.Instance.CurrentScene.ClientCharacter)
        {
            actor.Instance.PopHint(String.Format("{0:n0}", data["dmg"].AsInt) , new Color(231f/255f, 103f/255f, 103f/255f ,1f));
            actor.CurrentHealth = data["hp"].AsInt;

            InGameMainMenuUI.Instance.RefreshHP();
        }
        else
        {
            actor.Instance.Hurt();
        }
    }

    protected void OnActorLoadAttack(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];


        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);

        BroadcastEvent(actor.Name + " Loads Attack");

        actor.Instance.LoadAttack(data["ability"].Value);
    }

    protected void OnActorPreformAttack(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];


        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);

        BroadcastEvent(actor.Name + " Preforms Attack");

        actor.Instance.PreformAttack(data["ability"].Value, (1f*data["load"].AsInt)/100f);
    }


    private void OnMobMovement(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        //BroadcastEvent("Mob Moved " + data["mob_id"].Value);
        
        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);

        monster.UpdateMovement(data["x"].AsFloat, data["y"].AsFloat);

    }

    private void OnMobTakeDamage(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["mob_id"].Value + " Took " + data["dmg"].AsInt + " DMG from " + data["id"].Value);

        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);
        ActorInstance attackingPlayer = Game.Instance.CurrentScene.GetActor(data["id"].Value).Instance;
        
        monster.Hurt(attackingPlayer, data["dmg"].AsInt);
    }

    private void OnMobDeath(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["mob_id"].Value + " Died...");

        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);

        monster.gameObject.GetComponent<Enemy>().Death();
    }

    private void OnMobSpawn(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["key"].Value + " Spawned");

        Game.Instance.SpawnMonster(data["mob_id"].Value, data["x"].AsFloat, data["y"].AsFloat, data["key"].Value, data["hp"].AsInt);
    }

    #endregion

    #region Emittions

    public void EmitLoadedScene(string fromScene = "")
    {
        BroadcastEvent("Emitted : LoadedScene");
        LoadingWindowUI.Instance.Leave(this);
        JSONNode node = new JSONClass();

        CurrentSocket.Emit("entered_room", node);

        Game.Instance.LoadPlayerCharacter(fromScene);
    }

    public void EmitMoveRoom(string targetRoom)
    {
        BroadcastEvent("Emitted : MovedRoom");
        JSONNode node = new JSONClass();

        node["room"] = targetRoom;

        CurrentSocket.Emit("moved_room", node);
    }

    public void EmitMovement(Vector3 pos, float rotDegrees)
    {
        JSONNode node = new JSONClass();

        node["x"] = pos.x.ToString();
        node["y"] = pos.y.ToString();
        node["z"] = pos.z.ToString();
        node["angle"].AsFloat = rotDegrees;

        CurrentSocket.Emit("movement", node);
    }

    public void SendBitchPlease(string requestKey)
    {
        JSONNode node = new JSONClass();
        node["key"] = requestKey;
        CurrentSocket.Emit("bitch_please", node);
    }

    public void SendChatMessage(string message)
    {
        JSONNode node = new JSONClass();
        node["msg"] = message;
        CurrentSocket.Emit("chatted", node);
    }

    public void SendWhisper(string message, string targetName)
    {
        JSONNode node = new JSONClass();
        node["msg"] = message;
        node["to"] = targetName;
        CurrentSocket.Emit("whispered", node);
    }

    public void SendPickedItem(string ItemID)
    {
        Debug.Log("picking up " + ItemID);
        JSONNode node = new JSONClass();

        node["item_id"] = ItemID;

        CurrentSocket.Emit("picked_item", node);
    }

    public void SendDroppedItem(int slotIndex)
    {
        JSONNode node = new JSONClass();

        node["slot"].AsInt = slotIndex;

        CurrentSocket.Emit("dropped_item", node);
    }


    public void SendMovedItem(int fromIndex, int toIndex)
    {
        JSONNode node = new JSONClass();

        node["from"].AsInt = fromIndex;
        node["to"].AsInt = toIndex;

        CurrentSocket.Emit("moved_item", node);
    }

    public void SendEquippedItem(int fromIndex, string Slot)
    {
        JSONNode node = new JSONClass();

        node["from"].AsInt = fromIndex;
        node["to"] = Slot;

        Debug.Log(node);

        CurrentSocket.Emit("equipped_item", node);
    }

    public void SendUnequippedItem(string fromSlot, int toIndex)
    {
        JSONNode node = new JSONClass();

        node["from"] = fromSlot;
        node["to"].AsInt = toIndex;

        Debug.Log(node);

        CurrentSocket.Emit("unequipped_item", node);
    }

    public void SendUsedEquip(string fromSlot)
    {
        JSONNode node = new JSONClass();

        node["slot"] = fromSlot;

        CurrentSocket.Emit("used_equip", node);
    }

    public void SendUsedItem(int inventoryIndex)
    {
        JSONNode node = new JSONClass();

        node["slot"] = inventoryIndex.ToString();

        CurrentSocket.Emit("used_item", node);
    }

    public void SendDroppedEquip(string fromSlot)
    {
        JSONNode node = new JSONClass();

        node["slot"] = fromSlot;

        CurrentSocket.Emit("dropped_equip", node);
    }

    public void SendMovedEquip(string fromSlot, string toSlot)
    {
        JSONNode node = new JSONClass();

        node["from"] = fromSlot;
        node["to"] = toSlot;

        CurrentSocket.Emit("moved_equip", node);
    }

    public void SendEmote(string type, string emote)
    {
        JSONNode node = new JSONClass();

        node["type"] = type;
        node["emote"] = emote;

        CurrentSocket.Emit("emoted", node);
    }

    public void SendTookDMG(EnemyInfo Info)
    {
        JSONNode node = new JSONClass();

        node["mob_id"] = Info.ID;

        CurrentSocket.Emit("took_dmg", node);
    }


    public void SendLoadedAttack()
    {
        JSONNode node = new JSONClass();
        CurrentSocket.Emit("loaded_attack", node);
    }

    public void SendPreformedAttack(float attackValue)
    {
        JSONNode node = new JSONClass();

        // value might be more than 1 sometimes since we are dealing with floats
        attackValue = Mathf.Min(1f, attackValue); 

        node["load"] = Mathf.FloorToInt(attackValue * 100f).ToString();

        CurrentSocket.Emit("performed_attack", node);
    }


    public void SendMobMove(string instanceID, float x, float y)
    {
        JSONNode node = new JSONClass();

        node["mob_id"] = instanceID;
        node["x"] = x.ToString();
        node["y"] = y.ToString();

        CurrentSocket.Emit("mob_moved", node);
    }

    public void SendMobTookDamage(ActorInstance parentActor, Enemy enemyReference)
    {
        JSONNode node = new JSONClass();

        node["mob_id"] = enemyReference.Info.ID;

        CurrentSocket.Emit("mob_took_dmg", node);
    }

    #endregion

    #region Internal


    protected void BroadcastEvent(string info)
    {
        if (DebugMode)
        {
            Debug.Log(this + " | " + info);
        }
    }


    #endregion
}
