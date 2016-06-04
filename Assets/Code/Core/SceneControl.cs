using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SceneControl
{
    protected Dictionary<string, ActorInfo> actors = new Dictionary<string, ActorInfo>();

    public ActorInfo ClientCharacter { protected set; get; }

    public int ActorCount { get { return actors.Count; } }

    public void Join(ActorInfo info)
    {
        if (!actors.ContainsKey(info.ID))
        {
            actors.Add(info.ID ,info);
        }

        if (info.ID == LocalUserInfo.Me.SelectedCharacter.ID)
        {
            ClientCharacter = info;
        }
    }

    public void Leave(ActorInfo info)
    {
        if(actors.ContainsKey(info.ID))
        {
            actors.Remove(info.ID);
        }
    }

    public ActorInfo GetPlayerByName(string Name)
    {
        for(int i=0;i<actors.Keys.Count;i++)
        {
            if(actors[actors.Keys.ElementAt(i)].Name == Name)
            {
                return actors[actors.Keys.ElementAt(i)];
            }
        }

        return null;
    }

    public ActorInfo GetPlayer(string ID)
    {
        if(actors.ContainsKey(ID))
        {
            return actors[ID];
        }

        return null;
    }

}
