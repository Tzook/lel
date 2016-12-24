using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SceneControl
{
    protected Dictionary<string, ActorInfo> actors = new Dictionary<string, ActorInfo>();

    protected List<GatePortal> Portals = new List<GatePortal>();

    protected Dictionary<string, ItemInstance> items = new Dictionary<string, ItemInstance>();

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

    public void AddScenePortal(GatePortal portal)
    {
        Portals.Add(portal);
    }

    public ActorInfo GetActorByName(string Name)
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

    public ActorInfo GetActor(string ID)
    {
        if(actors.ContainsKey(ID))
        {
            return actors[ID];
        }

        return null;
    }

    public GatePortal GetPortal(string targetScene)
    {
        for(int i=0;i<Portals.Count;i++)
        {
            if(Portals[i].TargetLevel == targetScene)
            {
                return Portals[i];
            }
        }

        return null;
    }

    internal void AddSceneItem(ItemInstance itemInstance, string instanceID)
    {
        if (!items.ContainsKey(instanceID))
        {
            items.Add(instanceID, itemInstance);
        }
    }

    public ItemInstance GetSceneItem(string instanceID)
    {
        return items[instanceID];
    }

    public void DestroySceneItem(string instanceID)
    {
        items[instanceID].gameObject.SetActive(false);
        items.Remove(instanceID);
    }
}
