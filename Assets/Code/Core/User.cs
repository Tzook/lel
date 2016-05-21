using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

public class User {

    public string ID;
    public string Name;

    public void UpdateData(JSONNode data)
    {
        if (data != null)
        {
            this.ID = data["_id"].Value;
            this.Name = data["username"].Value;
            SetCharacters(data["characters"]);
        }
    }

    public void SetCharacters(JSONNode data)
    {
        ClearCharacters();
        for(int i = 0; i < data.Count; i++)
        {
            AddCharacter(data[i]);
        }
    }

    public void ClearCharacters()
    {
        Characters.Clear();
    }

    public void AddCharacter(JSONNode node)
    {
        ActorInfo tempCharacter = new ActorInfo(node);
        Characters.Add(tempCharacter);
    }

    public ActorInfo SelectedCharacter;

    public List<ActorInfo> Characters = new List<ActorInfo>();



}
