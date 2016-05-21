using System;
using System.Collections.Generic;
using BestHTTP.SocketIO;

public class WebSocketConnector 
{
	private const string URL = Config.SocketUrl;



	public Socket connect(string charName)
	{
		SocketOptions options = new SocketOptions();
		options.AdditionalQueryParams = new Dictionary<string, string>();
		options.AdditionalQueryParams.Add("ch", charName);
		var manager = new SocketManager(new Uri(URL), options);
		manager.Open();
		return manager.Socket;
	}
}
