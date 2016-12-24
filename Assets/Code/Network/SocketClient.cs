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
        CurrentSocket.On("actor_join_room", OnActorJoinRoom);
        CurrentSocket.On("actor_leave_room", OnActorLeaveRoom);
        CurrentSocket.On("actor_move_room", OnMoveRoom);
        CurrentSocket.On("message", OnChatMessage);
        CurrentSocket.On("movement", OnMovement);
        CurrentSocket.On("actor_pick_item", OnActorPickItem);
        CurrentSocket.On("drop_item", OnActorDropItem);
        CurrentSocket.On("item_disappear", OnItemDisappear);

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
        Game.Instance.RemoveNpcCharacter(new ActorInfo(data["character"]));
    }

    protected void OnMovement(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Movement occured");

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
        Game.Instance.ReceiveChatMessage(data["id"], data["message"], data["type"]);
    }

    protected void OnMoveRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Moved Room");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.LoadScene(data["room"], data["oldRoom"]);
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

        CurrentSocket.Emit("move_room", node);
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

    public void SendChatMessage(string Message)
    {
        JSONNode node = new JSONClass();
        node["message"] = Message;
        node["type"] = "room"; // can also be "whisper" or "global"
        //node["target_id"] = id; use this when whispering
        CurrentSocket.Emit("message", node);
    }

    public void SendPickedItem(string ItemID)
    {
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

        Debug.Log("MovedItem from " + node["from"].AsInt + " to " + node["to"].AsInt);

        CurrentSocket.Emit("moved_item", node);
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
