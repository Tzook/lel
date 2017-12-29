using SimpleJSON;
using System;

public class SessionHandler : HttpProvider 
{
	private const string SESSION_URL = "/user/session";

	public SessionHandler(Action<JSONNode> callback) : base(callback){}
	
	public void Session()
	{
		performRequest(SESSION_URL, new JSONClass(), false);
	}
}
