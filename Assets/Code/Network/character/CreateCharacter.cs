using SimpleJSON;
using System;

public class CreateCharacter : HttpProvider
{
	private const string CREATE_CHARACTER_URL = "/character/create";

	public CreateCharacter(Action<JSONNode> callback) : base(callback){}

	public void Create(ActorInfo info)
	{
		JSONNode parameters = new JSONClass();
		parameters["name"] = info.Name;
		parameters["g"] = ((info.Gender == Gender.Male) ? "1" : "0");
        parameters["eyes"] = info.Eyes;
        parameters["nose"] = info.Nose;
        parameters["mouth"] = info.Mouth;
        parameters["skin"].AsInt = info.SkinColor;
        parameters["hair"] = info.Hair;
        parameters["str"] = "4";
        parameters["mag"] = "2";
        parameters["dex"] = "1";


        performRequest(CREATE_CHARACTER_URL, parameters, true);
	}
}
