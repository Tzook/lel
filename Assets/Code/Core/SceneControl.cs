using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SceneControl
{
    protected Dictionary<string, ActorInfo> actors = new Dictionary<string, ActorInfo>();
    protected List<GatePortal>              Portals = new List<GatePortal>();
    public List<Enemy>                      Enemies = new List<Enemy>();
    public Dictionary<string, ItemInstance> Items = new Dictionary<string, ItemInstance>();

    public int SceneItemsCount
    {
        get { return Items.Count; } set{ } }

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

    public void Leave(string id)
    {
        if(actors.ContainsKey(id))
        {
            actors.Remove(id);
        }
    }

    public void AddScenePortal(GatePortal portal)
    {
        Portals.Add(portal);
    }

    public void AddSceneEnemy(Enemy enemy)
    {
        if (!Enemies.Contains(enemy))
        {
            Enemies.Add(enemy);
        }
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

    public Enemy GetMob(string instanceID)
    {
        for(int i=0;i<Enemies.Count;i++)
        {
            if(Enemies[i].Info.ID == instanceID)
            {
                return Enemies[i];
            }
        }

        return null;
    }

    public void DestroySceneItem(string instanceID)
    {
        Items[instanceID].gameObject.SetActive(false);
        Items.Remove(instanceID);
    }
}
