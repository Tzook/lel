using SimpleJSON;
using System;

public class BanHandler : HttpProvider 
{
	private const string BAN_URL = "/user/ban";
	private const string UNBAN_URL = "/user/unban";

	public BanHandler() : base(delegate {}){}
	
	public void Ban(string charName, string reason)
	{
		JSONNode parameters = new JSONClass();
		parameters["char_name"] = charName;
		parameters["reason"] = reason;
		
		performRequest(BAN_URL, parameters, true);
	}
	
	public void Unban(string charName)
	{
		JSONNode parameters = new JSONClass();
		parameters["char_name"] = charName;
		
		performRequest(UNBAN_URL, parameters, true);
	}
}
