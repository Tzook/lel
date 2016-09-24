using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using SimpleJSON;


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
        CurrentSocket.On("message", OnChatMessage);

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
            instance.UpdatePosition(new Vector3(data["x"].AsFloat, data["y"].AsFloat, data["z"].AsFloat));
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

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"]);
        if (actorInfo != null && actorInfo.Instance)
        {
            actorInfo.Instance.ChatBubble(data["message"]);
        }
        else
        {
            Debug.LogError(data["id"] + " is no longer in the room for this event to occur.");
        }
    }

    #endregion

    #region Emittions

    public void EmitLoadedScene()
    {
        BroadcastEvent("Emitted : LoadedScene");
        LoadingWindowUI.Instance.Leave(this);
        JSONNode node = new JSONClass();

        CurrentSocket.Emit("entered_room", node);

        Game.Instance.LoadPlayerCharacter();

        CurrentSocket.On("movement", OnMovement);
    }

    public void EmitMovement(Vector3 pos)
    {
        JSONNode node = new JSONClass();
        node["x"] = pos.x.ToString();
        node["y"] = pos.y.ToString();
        node["z"] = pos.z.ToString();
        CurrentSocket.Emit("movement", node);
    }

    public void SendChatMessage(string Message)
    {
        JSONNode node = new JSONClass();
        node["message"] = Message;
        CurrentSocket.Emit("message", node);
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
