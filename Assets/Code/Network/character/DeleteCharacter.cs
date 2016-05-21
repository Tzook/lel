using SimpleJSON;
using System;

public class DeleteCharacter : HttpProvider
{
	private const string DELETE_CHARACTER_URL = "/character/delete";

	public DeleteCharacter(Action<JSONNode> callback) : base(callback){}

	public void Delete(string id)
	{
		JSONNode parameters = new JSONClass();
		parameters["id"] = id;

		performRequest(DELETE_CHARACTER_URL, parameters, true);
	}
}
