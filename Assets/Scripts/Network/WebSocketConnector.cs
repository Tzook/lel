using System;
using System.Collections.Generic;
using BestHTTP.SocketIO;

public class WebSocketConnector
{
	private const string URL = Config.SOCKET_URL;

	public Socket connect(string charId)
    {
        SocketOptions options = new SocketOptions();
        options.AdditionalQueryParams = CookiesManager.Instance.GetCookies();
        options.AdditionalQueryParams.Add("id", charId);
        var manager = new SocketManager(new Uri(URL), options);
        manager.Encoder = new SimpleJsonEncoder();
        manager.Open();
        return manager.Socket;
    }
}
