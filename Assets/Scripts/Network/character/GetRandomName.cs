using SimpleJSON;
using System;

public class GetRandomName : HttpProvider
{
	private const string GET_RANDOM_NAME_URL = "/character/random-name";

	public GetRandomName(Action<JSONNode> callback) : base(callback){}

	public void Get(Gender gender)
	{
		JSONNode parameters = new JSONClass();
		string urlSuffix = GET_RANDOM_NAME_URL + "?g=" + (gender == Gender.Male ? "1" : "0");

        performRequest(urlSuffix, new JSONClass(), false);
	}
}
