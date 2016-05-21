using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BestHTTP.SocketIO;
using System;

public class CharacterInfoPageUI : MonoBehaviour {

    [SerializeField]
    protected ActorInstance actorInstance;

    [SerializeField]
    protected Text txtName;

    protected WebSocketConnector webSocketConnector;

    void Start()
    {
        webSocketConnector = new WebSocketConnector();
    }

    public void SetInfo(ActorInfo info)
    {
        actorInstance.UpdateVisual(info);
        txtName.text = info.Name;
    }

    public void ConnectToGame()
    {
        Debug.Log("Connecting to server..");
        Socket socket = webSocketConnector.connect(actorInstance.Info.ID);
        socket.On("connect", OnConnect);
        socket.On("disconnect", OnDisconnect);
        socket.On("error", OnError);
    }

    private void OnError(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("On error");
    }

    private void OnDisconnect(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("On disconnect");
    }

    protected void OnConnect(Socket socket, Packet packet, params object[] args)
	{
        Debug.Log("On connect");
	}
}
