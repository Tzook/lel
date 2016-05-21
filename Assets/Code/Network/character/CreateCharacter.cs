using SimpleJSON;
using System;

public class CreateCharacter : HttpProvider 
{
	private const string CREATE_CHARACTER_URL = "/character/create";

	public CreateCharacter(Action<JSONNode> callback) : base(callback){}
	
	public void Create(string name, Gender gender)
	{
		JSONNode parameters = new JSONClass();
		parameters["name"] = name;
		parameters["g"] = ((gender == Gender.Male) ? "1" : "0");
		
		performRequest(CREATE_CHARACTER_URL, parameters, true);
	}
}
