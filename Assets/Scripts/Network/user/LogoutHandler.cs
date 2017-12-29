using SimpleJSON;
using System;

public class LogoutHandler : HttpProvider 
{
	private const string LOGOUT_URL = "/user/logout";

	public LogoutHandler(Action<JSONNode> callback) : base(callback){}
	
	public void Logout()
	{
		performRequest(LOGOUT_URL, new JSONClass(), false);
	}
}
