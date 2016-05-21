using SimpleJSON;
using System;

public class RegisterHandler : HttpProvider
{
	private const string REGISTER_URL = "/user/register";

	public RegisterHandler(Action<JSONNode> callback) : base(callback){}
	
	public void Register(string user, string password)
	{
		JSONNode parameters = new JSONClass();
		parameters["username"] = user;
		parameters["password"] = Md5Encryptor.GetMd5Hash(password);
		
		performRequest(REGISTER_URL, parameters, true);
	}
}
