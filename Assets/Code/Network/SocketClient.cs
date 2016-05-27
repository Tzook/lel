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

    void Awake()
    {
        SM.SocketClient = this;
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
        CurrentSocket.On("movement", OnMovement);
        SM.LoadingWindow.Register(this);
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
        SM.Game.LoadScene(LocalUserInfo.Me.SelectedCharacter.CurrentRoom);
    }

    protected void OnActorJoinRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Actor has joined the room");

        JSONNode data = (JSONNode)args[0];
        for (int i = 0; i < data.Count; i++)
        {
            SM.Game.SpawnPlayer(new ActorInfo(data[i]));
        }
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

    #endregion

    #region Emittions

    public void EmitLoadedScene()
    {
        BroadcastEvent("Emitted : LoadedScene");
        SM.LoadingWindow.Leave(this);
        JSONNode node = new JSONClass();
        CurrentSocket.Emit("entered_room", node);
    }

    public void EmitMovement(Vector3 pos)
    {
        JSONNode node = new JSONClass();
        node["x"] = pos.x.ToString();
        node["y"] = pos.y.ToString();
        node["z"] = pos.z.ToString();
        CurrentSocket.Emit("movement", node);
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
