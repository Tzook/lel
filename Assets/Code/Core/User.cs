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

    public ActorInfo ClientCharacter;

    public List<ActorInfo> Characters = new List<ActorInfo>();

    public Dictionary<string, KnownCharacter> KnownCharacters = new Dictionary<string, KnownCharacter>();

    public void ClearKnownCharacters()
    {
        KnownCharacters.Clear();
    }

    public void AddKnownCharacter(ActorInfo info)
    {
        if (!KnownCharacters.ContainsKey(info.Name))
        {
            KnownCharacter character = new KnownCharacter(info);
            character.isLoggedIn = true;
            KnownCharacters.Add(info.Name, character);
        }
        else
        {
            KnownCharacters[info.Name].Info = info;
        }

        
    }

    public void RemoveKnownCharacter(string cName)
    {
        if (!KnownCharacters.ContainsKey(cName))
        {
            KnownCharacters.Remove(cName);
        }
    }

    public KnownCharacter GetKnownCharacter(string key)
    {
        if(KnownCharacters.ContainsKey(key))
        {
            return KnownCharacters[key];
        }

        return null;
    }

    public Party CurrentParty;

    public void DisposeCurrentCharacter()
    {
        ClientCharacter = null;
        CurrentParty = null;
        ClearKnownCharacters();
    }
}
