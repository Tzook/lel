using SimpleJSON;
using System;

public class LoginHandler : HttpProvider 
{
	private const string LOGIN_URL = "/user/login";

	public LoginHandler(Action<JSONNode> callback) : base(callback){}
	
	public void Login(string user, string password)
	{
		JSONNode parameters = new JSONClass();
		parameters["username"] = user;
		parameters["password"] = Md5Encryptor.GetMd5Hash(password);
		
		performRequest(LOGIN_URL, parameters, true);
	}
}
