using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;

public class SocketClient : MonoBehaviour {

    #region Config
    public bool DebugMode = false;

    #endregion

    #region Essential

    protected WebSocketConnector webSocketConnector;

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
        Socket socket = webSocketConnector.connect(LocalUserInfo.Me.SelectedCharacter.ID);
        socket.On("connect", OnConnect);
        socket.On("disconnect", OnDisconnect);
        socket.On("error", OnError);
        socket.On("load_scene", OnLoadScene);
    }

    public void Subscribe(string id, IUpdatePositionListener instance)
    {
        if(!SubscribedMovables.ContainsKey(id))
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

    #region Internal Methods

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

    protected void OnLoadScene(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("On Load Scene");
    }

    protected void BroadcastEvent(string info)
    {
        if(DebugMode)
        {
            Debug.Log(this + " | " + info);
        }
    }

    #endregion
}
