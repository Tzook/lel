using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using SimpleJSON;
using System;

public class SocketClient : MonoBehaviour
{
    private const int BITCH_WAIT_FRAMES = 5;

    #region Config
    public bool DebugMode = false;

    #endregion

    #region Essential

    protected WebSocketConnector webSocketConnector;

    protected Socket CurrentSocket;

    public static SocketClient Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        webSocketConnector = new WebSocketConnector();
    }

    #endregion

    #region Public Methods

    public void ConnectToGame()
    {
        BroadcastEvent("Connecting to server..");
        CurrentSocket = webSocketConnector.connect(LocalUserInfo.Me.ClientCharacter.ID);

        CurrentSocket.On("connect", OnConnect);
        CurrentSocket.On("disconnect", OnDisconnect);
        CurrentSocket.On("error", OnError);

        CurrentSocket.On("event_error", OnEventError);

        CurrentSocket.On("actor_join_room", OnActorJoinRoom);
        CurrentSocket.On("actor_leave_room", OnActorLeaveRoom);
        CurrentSocket.On("actor_move_room", OnMoveRoom);
        CurrentSocket.On("bitch_please", OnBitchPlease);
        CurrentSocket.On("actor_bitch", OnActorBitch);
        CurrentSocket.On("room_state", OnRoomState);

        CurrentSocket.On("chat", OnChatMessage);
        CurrentSocket.On("party_chat", OnPartyChat);
        CurrentSocket.On("whisper", OnWhisper);

        CurrentSocket.On("movement", OnMovement);

        CurrentSocket.On("actor_start_climbing", OnActorStartClimbing);
        CurrentSocket.On("actor_stop_climbing", OnActorStopClimbing);

        CurrentSocket.On("actor_pick_item", OnActorPickItem);
        CurrentSocket.On("drop_items", OnDropItems);
        CurrentSocket.On("item_disappear", OnItemDisappear);
        CurrentSocket.On("item_owner_gone", OnItemOwnerGone);
        CurrentSocket.On("actor_move_item", OnActorMoveItem);
        CurrentSocket.On("actor_add_item", OnActorAddItem);
        CurrentSocket.On("actor_delete_item", OnActorDeleteItem);
        CurrentSocket.On("change_item_stack", OnChangeItemStack);
        CurrentSocket.On("change_gold_amount", OnChangeGoldAmount);

        CurrentSocket.On("actor_equip_item", OnActorEquipItem);
        CurrentSocket.On("actor_unequip_item", OnActorUnequipItem);
        CurrentSocket.On("actor_delete_equip", OnActorDeleteEquip);

        CurrentSocket.On("actor_emote", OnActorEmoted);

        CurrentSocket.On("actor_gain_hp", OnActorGainHP);
        CurrentSocket.On("actor_gain_mp", OnActorGainMP);
        CurrentSocket.On("actor_gain_exp", OnActorGainXP);
        CurrentSocket.On("actor_gain_stats", OnActorGainStats);
        CurrentSocket.On("update_client_perks", OnUpdateClientPerks);
        CurrentSocket.On("actor_lvl_up", OnActorLevelUp);
        CurrentSocket.On("actor_gain_ability", OnActorGainAbility);

        CurrentSocket.On("take_damage", OnTakeDamage);
        CurrentSocket.On("actor_blocked", OnActorBlocked);
        CurrentSocket.On("actor_ded", OnActorDed);
        CurrentSocket.On("actor_resurrect", OnActorResurrect);

        CurrentSocket.On("actor_load_attack", OnActorLoadAttack);
        CurrentSocket.On("actor_perform_attack", OnActorPreformAttack);
        CurrentSocket.On("actor_change_ability", OnActorChangeAbility);

        CurrentSocket.On("mob_spawn", OnMobSpawn);
        CurrentSocket.On("mob_die", OnMobDeath);
        CurrentSocket.On("mob_take_miss", OnMobTakeMiss);
        CurrentSocket.On("mob_move", OnMobMovement);
        CurrentSocket.On("mob_blocked", OnMobBlocked);
        CurrentSocket.On("aggro", OnAggro);

        CurrentSocket.On("quest_start", OnQuestStart);
        CurrentSocket.On("quest_complete", OnQuestComplete);
        CurrentSocket.On("quest_abort", OnQuestAbort);
        CurrentSocket.On("quest_hunt_progress", OnQuestHuntProgress);
        CurrentSocket.On("quest_ok_progress", OnQuestOkProgress);

        CurrentSocket.On("create_party", OnCreateParty);
        CurrentSocket.On("party_invitation", OnPartyInvitation);
        CurrentSocket.On("actor_join_party", OnActorJoinParty);
        CurrentSocket.On("actor_leave_party", OnActorLeaveParty);
        CurrentSocket.On("actor_kicked_from_party", OnActorKickedFromParty);
        CurrentSocket.On("actor_lead_party", OnActorLeadParty);
        CurrentSocket.On("party_members", OnPartyMembersUpdate);

        CurrentSocket.On("known_info", OnKnownInfo);
        CurrentSocket.On("known_move_room", OnKnownMoveRoom);
        CurrentSocket.On("known_logout", OnKnownLogOut);
        CurrentSocket.On("known_login", OnKnownLogIn);

        CurrentSocket.On("actor_transaction", OnTransaction);

        CurrentSocket.On("ability_gain_exp", OnAbilityGainEXP);
        CurrentSocket.On("ability_lvl_up", OnAbilityLevelUp);
        CurrentSocket.On("ability_choose_perk", OnAbilityChoosePerk);
        CurrentSocket.On("ability_gain_perk", OnAbilityGainPerk);

        CurrentSocket.On("buff_activated", OnBuffActivated);
        CurrentSocket.On("buff_resisted", OnBuffResisted);
        CurrentSocket.On("spell_activated", OnSpellActivated);
        CurrentSocket.On("spell_cooldown", OnSpellCooldown);
        CurrentSocket.On("mob_spell_activated", OnMobSpellActivated);

        LoadingWindowUI.Instance.Register(this);
    }

    public void Disconnect()
    {
        if (CurrentSocket != null)
        {
            CurrentSocket.Disconnect();
            CurrentSocket.Off();
        }
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    #endregion

    #region Callbacks

    private void OnError(Socket socket, Packet packet, object[] args)
    {
        Error error = args[0] as Error;
        BroadcastEvent("On error: " + error);
    }
    
    private void OnEventError(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("On event error: " + data.AsObject.ToString());

        if (data["display"].AsBool)
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(data["error"].Value);
        }
    }

    private void OnDisconnect(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("On disconnect");

        LocalUserInfo.Me.DisposeCurrentCharacter();
        Game.Instance.Online = false;
    }

    protected void OnConnect(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("On connect");
        Game.Instance.Online = true;
    }

    protected void OnActorJoinRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Actor has joined the room");

        JSONNode data = (JSONNode)args[0];


        Game.Instance.LoadOtherPlayerCharacter(new ActorInfo(data["character"]));
    }

    protected void OnActorLeaveRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Actor has left the room");

        JSONNode data = (JSONNode)args[0];
        Game.Instance.RemoveOtherPlayerCharacter(data["id"]);
    }

    protected void OnMovement(Socket socket, Packet packet, params object[] args)
    {
        //BroadcastEvent("Movement occured");

        JSONNode data = (JSONNode)args[0];

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"]);
        if (actorInfo != null)
        {
            actorInfo.Instance.GetComponent<ActorMovement>().UpdateMovement(new Vector3(data["x"].AsFloat, data["y"].AsFloat, data["z"].AsFloat), data["angle"].AsFloat, data["velocity"].AsFloat);
        }
    }

    protected void OnActorStartClimbing(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        //BroadcastEvent(data["id"].Value + " started climbing");

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"]);
        if (actorInfo != null)
        {
            actorInfo.Instance.GetComponent<ActorMovement>().StartClimbing();
        }

    }

    protected void OnActorStopClimbing(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        //BroadcastEvent(data["id"].Value + " started climbing");

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"]);
        if (actorInfo != null)
        {
            actorInfo.Instance.GetComponent<ActorMovement>().StopClimbing();
        }

    }

    protected void OnChatMessage(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Chat Message!");

        JSONNode data = (JSONNode)args[0];
        ChatHandler.Instance.ReceiveChatMessage(data["id"], data["msg"]);
    }

    protected void OnPartyChat(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Chat Message!");

        JSONNode data = (JSONNode)args[0];
        ChatHandler.Instance.ReceivePartyMessage(data["id"], data["name"], data["msg"]);
    }


    protected void OnWhisper(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Whisper Message!");

        JSONNode data = (JSONNode)args[0];
        ChatHandler.Instance.ReceiveWhisper(data["id"], data["name"], data["msg"]);
    }

    protected void OnMoveRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Moved Room");

        JSONNode data = (JSONNode)args[0];

        Game.Instance.LoadScene(data["room"], new ActorInfo(data["character"]));

    }

    protected void OnBitchPlease(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Bitch Please");

        JSONNode data = (JSONNode)args[0];
        StartCoroutine(BitchPleaseCoroutine(data["key"].Value));
    }

    protected void OnActorBitch(Socket socket, Packet packet, params object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Bitch "+ data["is_bitch"].AsBool);

        Game.Instance.SetBitch(data["is_bitch"].AsBool);
    }

    protected void OnRoomState(Socket socket, Packet packet, params object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        string key = data["key"].Value;
        string value = data["value"].Value;
        BroadcastEvent("Room State "+ key + ": " + value);

        SceneInfo.Instance.UpdateStateChange(key, value);
    }

    protected void OnItemDisappear(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Disappeared");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.CurrentScene.DestroySceneItem(data["item_id"].Value);
    }

    protected void OnItemOwnerGone(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item's owner is gone");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.CurrentScene.RemoveItemOwner(data["item_id"].Value);
    }

    protected void OnDropItems(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("An item was dropped");
        JSONNode data = (JSONNode)args[0];

        List<ItemInfo> infoList = new List<ItemInfo>();
        List<string> idsList = new List<string>();
        List<string> ownersList = new List<string>();

        //Debug.Log(data.ToString());

        for(int i=0;i<data.Count;i++)
        {
            JSONNode item = data[i]["item"];
           // Debug.Log("ITEM - " + item["key"].Value);
            infoList.Add(new ItemInfo(Content.Instance.GetItem(item["key"].Value), PerkListGetter.Instance.Get(item), item["stack"].AsInt));
            idsList.Add(data[i]["item_id"].Value);
            ownersList.Add(data[i]["owner"].Value);
        }

        Game.Instance.SpawnItems(infoList, idsList, ownersList, data[0]["x"].AsFloat, data[0]["y"].AsFloat);
    }

    protected void OnActorPickItem(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent(data["id"].Value + " picked the item "+ data["item_id"].Value);

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"]);
        if (actorInfo != null)
        {
            actorInfo.Instance.PickUpItem(data["item_id"]);
        }
    }

    protected void OnActorMoveItem(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Moved");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.MoveInventoryItem(data["from"].AsInt, data["to"].AsInt);
    }

    protected void OnActorAddItem(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Added");
        JSONNode data = (JSONNode)args[0];

        ItemInfo tempItem = new ItemInfo(Content.Instance.GetItem(data["item"]["key"].Value), PerkListGetter.Instance.Get(data["item"]), data["item"]["stack"].AsInt);

        LocalUserInfo.Me.ClientCharacter.Instance.AddItem(data["slot"].AsInt, tempItem);
    }

    protected void OnChangeItemStack(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Stack Changed");
        JSONNode data = (JSONNode)args[0];

        LocalUserInfo.Me.ClientCharacter.Instance.ChangeItemStack(data["slot"].AsInt, data["amount"].AsInt);
    }

    protected void OnChangeGoldAmount(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Changed Gold");
        JSONNode data = (JSONNode)args[0];

        LocalUserInfo.Me.ClientCharacter.ChangeGold(data["amount"].AsInt);
    }

    protected void OnActorDeleteItem(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Item Deleted");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.DeleteInventoryItem(data["slot"].AsInt);
    }

    protected void OnActorEquipItem(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        ItemInfo swappedItem = null;

        //Debug.Log(data.ToString());

        if (!string.IsNullOrEmpty(data["equipped_item"]["key"]))
        {
            swappedItem = new ItemInfo(Content.Instance.GetItem(data["equipped_item"]["key"]), PerkListGetter.Instance.Get(data["equipped_item"]));
        }

        BroadcastEvent(data["id"].Value+" Equipped Item ");

        Game.Instance.ActorEquippedItem(data["id"].Value, data["from"].AsInt, data["to"].Value, swappedItem);
    }

    protected void OnActorUnequipItem(Socket socket, Packet packet, object[] args)
    {

        JSONNode data = (JSONNode)args[0];

        ItemInfo swappedItem = null;

        if (!string.IsNullOrEmpty(data["equipped_item"]["key"]))
        {
            swappedItem = new ItemInfo(Content.Instance.GetItem(data["equipped_item"]["key"].Value), PerkListGetter.Instance.Get(data["equipped_item"]));
        }

        BroadcastEvent(data["id"].Value + " Unequipped Item ");

        Game.Instance.ActorUnequippedItem(data["id"].Value, data["from"].Value, data["to"].AsInt, swappedItem);
    }

    protected void OnActorDeleteEquip(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Equip Deleted");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.DeleteEquip(data["id"].Value, data["slot"].Value);
    }

    protected void OnActorEmoted(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Actor Emoted");
        JSONNode data = (JSONNode)args[0];

        Game.Instance.ActorEmoted(data["id"].Value, data["type"].Value, data["emote"].Value);
    }

    protected void OnActorGainHP(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Gained " + data["hp"].AsInt + " HP");

        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);
        int hp = data["now"].AsInt;
        Debug.Log(data.ToString());
        if (actor != null)
        {
            actor.CurrentHealth = hp;
            if (actor == LocalUserInfo.Me.ClientCharacter) 
            {
                string text = String.Format("{0:n0}", data["hp"].AsInt);
                if (data["crit"].AsBool)
                {
                    // TODO make this beautiful, lel
                    text += " (CRIT)";
                }
                actor.Instance.PopHint(text, Color.white, "hitIcon_heal");
                InGameMainMenuUI.Instance.RefreshHP();
            }
            else 
            {

                if (data["cause"].Value == "heal")
                {
                    string text = String.Format("{0:n0}", data["hp"].AsInt);
                    if (data["crit"].AsBool)
                    {
                        // TODO make this beautiful, lel
                        text += " (CRIT)";
                    }
                    actor.Instance.PopHint(text, new Color(176f / 255f, 196f / 255f, 255f / 255f, 1f), "hitIcon_heal");
                }
                actor.Instance.MovementController.RefreshHealth();
            }
        }
        string name = data["name"].Value;
        UpdateKnownHealth(name, hp);
    }

    protected void OnActorGainMP(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Gained " + data["mp"].AsInt + " MP");

        LocalUserInfo.Me.ClientCharacter.CurrentMana = data["now"].AsInt;

        InGameMainMenuUI.Instance.RefreshMP();
    }

    protected void OnActorGainXP(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Gained " + data["exp"].AsInt + " XP");

        LocalUserInfo.Me.ClientCharacter.EXP = data["now"].AsInt;

        InGameMainMenuUI.Instance.MinilogMessage("+" + data["exp"].AsInt.ToString("N0") + " EXP");

        InGameMainMenuUI.Instance.RefreshXP();
    }

    protected void OnActorGainStats(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Actor Gained Stats | "+data.ToString());

        LocalUserInfo.Me.ClientCharacter.SetStats(data["stats"]);
        InGameMainMenuUI.Instance.RefreshStats();
        InGameMainMenuUI.Instance.RefreshXP();
        InGameMainMenuUI.Instance.RefreshLevel();

        Game.Instance.CurrentScene.UpdateAllQuestProgress();
    }

    protected void OnUpdateClientPerks(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor update client perks | " + data.ToString());

        ActorInfo actor = null;
        string id = data["id"].Value;
        if (Game.Instance.CurrentScene == null)
        {
            if (LocalUserInfo.Me.ClientCharacter.ID == id)
            {
                actor = LocalUserInfo.Me.ClientCharacter;
            }
        }
        else 
        {
            actor = Game.Instance.CurrentScene.GetActor(id);
        }

        if (actor != null)
        {
            bool refreshStats = false;
            
            if (data["hpBonus"] != null) 
            {
                actor.ClientPerks.MaxHealth = data["hpBonus"].AsInt;
                refreshStats = true;
            }
            
            if (data["knockbackModifier"] != null) 
            {
                actor.ClientPerks.KnockbackModifier = data["knockbackModifier"].AsFloat;
            }

            if (actor == LocalUserInfo.Me.ClientCharacter) 
            {
                if (data["mpBonus"] != null) 
                {
                    actor.ClientPerks.MaxMana = data["mpBonus"].AsInt;
                    refreshStats = true;
                }

                if (data["attackSpeedModifier"] != null) 
                {
                    actor.ClientPerks.AttackSpeed = data["attackSpeedModifier"].AsFloat;
                }

                if (data["mpCost"] != null) 
                {
                    actor.ClientPerks.ManaCost = data["mpCost"].AsFloat;
                    refreshStats = true;
                }

                if (data["questExpBonus"] != null) 
                {
                    actor.ClientPerks.QuestExpBonus = data["questExpBonus"].AsFloat;
                }

                if (data["questGoldBonus"] != null) 
                {
                    actor.ClientPerks.QuestGoldBonus = data["questGoldBonus"].AsFloat;
                }

                if (data["shopsDiscount"] != null) 
                {
                    actor.ClientPerks.ShopsDiscount = data["shopsDiscount"].AsFloat;
                }

                if (data["saleModifier"] != null) 
                {
                    actor.ClientPerks.SaleModifier = data["saleModifier"].AsFloat;
                }

                if (data["cooldownModifier"] != null) 
                {
                    actor.ClientPerks.CooldownModifier = data["cooldownModifier"].AsFloat;
                }
            }
            
            if (refreshStats && !Game.Instance.isLoadingScene && actor == LocalUserInfo.Me.ClientCharacter)
            {
                InGameMainMenuUI.Instance.RefreshStats();
            }
        }
    }

    protected void OnActorLevelUp(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor Leveled Up | "+data.ToString());

        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);
        ActorInfo info = actor;

        if (actor == LocalUserInfo.Me.ClientCharacter)
        {
            AudioControl.Instance.Play("sound_positive2");
            InGameMainMenuUI.Instance.MinilogMessage("Leveled Up!");
        }
        else
        {
            KnownCharacter known = LocalUserInfo.Me.GetKnownCharacter(data["name"].Value);
            if (known != null) 
            {
                info = known.Info;
            }
        }

        if (actor != null)
        {
            actor.Instance.LevelUp();
        }
        if (info != null)
        {
            info.LVL++;
        }
    }

    protected void OnActorGainAbility(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor gain ability | " + data.ToString());

        if (data["isCharAbility"].AsBool) {
            LocalUserInfo.Me.ClientCharacter.AddCharAbility(data["key"].Value, data["ability"]);
        } else {
            LocalUserInfo.Me.ClientCharacter.AddPrimaryAbility(data["key"].Value, data["ability"]);
        }
    }

    protected void OnTakeDamage(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Damage taken | " + data.ToString());

        int hp = data["hp"].AsInt;

        ActorInfo targetActor = Game.Instance.CurrentScene.GetActor(data["target_id"].Value);

        // TODO use same hurt method for actor and mob

        if (targetActor != null) 
        {
            // actor takes the damage
            targetActor.CurrentHealth = hp;
            targetActor.Instance.Hurt();
            if (targetActor == LocalUserInfo.Me.ClientCharacter)
            {
                string text = String.Format("{0:n0}", data["dmg"].AsInt);
                if (data["crit"].AsBool) {
                    // TODO make this beautiful, lel
                    text += " (CRIT)";
                }
                targetActor.Instance.PopHint(text, new Color(231f/255f, 103f/255f, 103f/255f ,1f));
                InGameMainMenuUI.Instance.RefreshHP();
            }
            else
            {
                targetActor.Instance.MovementController.RefreshHealth();
            }
        } else {
            // mob takes damage    
            Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["target_id"].Value);
            ActorInfo attackingActor = Game.Instance.CurrentScene.GetActor(data["attacker_id"].Value);
            // mob attacking mob still not supported ;P
            if (attackingActor != null && monster != null) 
            {
                monster.Hurt(attackingActor.Instance, data["dmg"].AsInt, hp, data["cause"].Value, data["crit"].AsBool);
            }
        }

        // updated knowns' health
        string name = data["name"].Value;
        UpdateKnownHealth(name, hp);
    }
    
    protected void OnActorBlocked(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor Blocked Attack");

        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);

        if (actor != null) 
        {
            // TODO style 'block' better
            actor.Instance.PopHint(String.Format("{0:n0}", "BLOCKED") , new Color(231f/255f, 103f/255f, 103f/255f ,1f), "");
        }
    }

    protected void UpdateKnownHealth(string name, int hp) 
    {
        KnownCharacter knownCharacter = LocalUserInfo.Me.GetKnownCharacter(name);
        if (knownCharacter != null)
        {
            knownCharacter.Info.CurrentHealth = hp;
            InGameMainMenuUI.Instance.RefreshParty();
        }
    }

    protected void OnActorDed(Socket socket, Packet packet, object[] args)
    {

        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor Has Died");

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"].Value);
        if (actorInfo != null)
        {
            if (actorInfo == LocalUserInfo.Me.ClientCharacter)
            {
                actorInfo.Instance.GetComponent<ActorController>().Death();
                InGameMainMenuUI.Instance.ShowDeathWindow();
            }
            else
            {
                actorInfo.Instance.Death();
            }
        }
    }

    protected void OnActorResurrect(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Actor Has Been Resurrected");

        Game.Instance.IsAlive = true;
    }

    protected void OnActorLoadAttack(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"].Value);

        if (actorInfo != null)
        {
            BroadcastEvent(actorInfo.Name + " Loads Attack");
            actorInfo.Instance.LoadAttack();
        }
    }

    protected void OnActorPreformAttack(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"].Value);

        if (actorInfo != null)
        {
            BroadcastEvent(actorInfo.Name + " Preforms Attack");

            float load = (1f * data["load"].AsInt) / 100f;
            actorInfo.Instance.PreformAttack(load);
            actorInfo.Instance.MovementController.ActivatePrimaryAbility(load);
        }
    }

    protected void OnActorChangeAbility(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"].Value);

        if (actorInfo != null)
        {
            BroadcastEvent(actorInfo.Name + " Changed ability Attack");
            actorInfo.SetPrimaryAbility(data["ability"].Value);
        }
    }

    private void OnMobMovement(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        //BroadcastEvent("Mob Moved " + data["mob_id"].Value);
        
        //TODO Just update each mob in the array instead...
        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);

        if (monster != null)
        {
            monster.UpdateMovement(data["x"].AsFloat, data["y"].AsFloat, data["velocity"].AsFloat);
        }

        // Debug.Log(data.ToString());

    }
    
    protected void OnMobBlocked(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["mob_id"].Value + " Blocked DMG from " + data["id"].Value);

        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);
        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"].Value);
        
        if (actorInfo != null && monster != null)
        {
            monster.Hurt(actorInfo.Instance, 0, data["hp"].AsInt);
        }
    }

    private void OnAggro(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Aggro on mob " + data["mob_id"].Value);
        
        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);

        if (monster != null)
        {
            if (String.IsNullOrEmpty(data["id"].Value))
            {
                monster.SetTarget(null);
            }
            else
            {
                ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"].Value);
                if (actorInfo != null)
                {
                    monster.SetTarget(actorInfo.Instance);
                }
            }
        }
    }

    private void OnMobTakeMiss(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["mob_id"].Value + " Took Miss from " + data["id"].Value);

        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);
        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(data["id"].Value);
        if (actorInfo != null && monster != null) 
        {
            monster.Miss(actorInfo.Instance, data["cause"].Value);
        }
    }

    private void OnMobDeath(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["mob_id"].Value + " Died...");

        Enemy monster = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);

        if (monster != null)
        {
            monster.gameObject.GetComponent<Enemy>().Death();
        }
    }

    private void OnMobSpawn(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];


        BroadcastEvent(data["key"].Value + " Spawned");

        Game.Instance.SpawnMonster(data["mob_id"].Value, data["x"].AsFloat, data["y"].AsFloat, data["key"].Value, data["hp"].AsInt);
    }



    private void OnQuestAbort(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["id"].Value + " Aborted");

        Game.Instance.CurrentScene.UpdateAllQuestProgress();
    }

    private void OnQuestStart(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["id"].Value + " Has Started");

        AudioControl.Instance.Play("sound_quest_taken");

        LocalUserInfo.Me.ClientCharacter.AddQuest(data["id"].Value);
        InGameMainMenuUI.Instance.RefreshQuestProgress();
        Game.Instance.CurrentScene.UpdateQuestProgress(data["id"].Value);

        Game.Instance.CurrentScene.UpdateAllQuestProgress();
    }

    private void OnQuestComplete(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["id"].Value + " Was Completed");

        AudioControl.Instance.Play("sound_quest_complete");

        LocalUserInfo.Me.ClientCharacter.CompleteQuest(data["id"].Value);
        InGameMainMenuUI.Instance.RefreshQuestProgress();
        InGameMainMenuUI.Instance.RefreshCompletedQuestProgress();

        Game.Instance.CurrentScene.UpdateAllQuestProgress();
    }

    private void OnQuestHuntProgress(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["id"].Value + " Had Progress");

        LocalUserInfo.Me.ClientCharacter.UpdateQuestProgress(data["id"].Value, data["mob_id"].Value, data["value"].AsInt);
        InGameMainMenuUI.Instance.RefreshQuestProgress();
        Game.Instance.CurrentScene.UpdateQuestProgress(data["id"].Value);

        AudioControl.Instance.Play("sound_quest_progress");
    }

    private void OnQuestOkProgress(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["id"].Value + " - OK " + data["ok"].Value + " is " + data["value"].Value);

        LocalUserInfo.Me.ClientCharacter.UpdateQuestProgress(data["id"].Value, data["ok"].Value, data["value"].AsInt);
        InGameMainMenuUI.Instance.RefreshQuestProgress();
        Game.Instance.CurrentScene.UpdateQuestProgress(data["id"].Value);

        AudioControl.Instance.Play("sound_quest_progress");
    }

    private void OnPartyMembersUpdate(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Register party members: " + data["chars_names"].Count);

        List<string> members = new List<string>();

        for (int i=0;i<data["chars_names"].Count;i++)
        {
            members.Add(data["chars_names"][i].Value);
        }

        Party party = new Party(data["leader_name"].Value, members);

        LocalUserInfo.Me.CurrentParty = party;

        InGameMainMenuUI.Instance.ShowParty();
    }

    private void OnCreateParty(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Create Party");

        List<string> members = new List<string>();

        members.Add(LocalUserInfo.Me.ClientCharacter.Name);

        LocalUserInfo.Me.CurrentParty = new Party(LocalUserInfo.Me.ClientCharacter.Name, members);

        InGameMainMenuUI.Instance.ShowParty();

        InGameMainMenuUI.Instance.ShockMessageTop.CallMessage("Created a new party.", Color.black, false);
    }

    private void OnActorLeadParty(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("Create Party");

        JSONNode data = (JSONNode)args[0];

        LocalUserInfo.Me.CurrentParty.Leader = data["char_name"].Value;

        InGameMainMenuUI.Instance.RefreshParty();

        if (LocalUserInfo.Me.ClientCharacter.Name == data["char_name"].Value)
        {
            InGameMainMenuUI.Instance.ShockMessageTop.CallMessage("You are the new party leader.", Color.black, false);
        }
        else 
        {
            InGameMainMenuUI.Instance.ShockMessageTop.CallMessage(data["char_name"].Value + " is the new party leader.", Color.black, false);
        }
    }

    private void OnActorKickedFromParty(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["char_name"].Value +  " was kicked from party");

        if (LocalUserInfo.Me.ClientCharacter.Name == data["char_name"].Value)
        {
            LocalUserInfo.Me.CurrentParty = null;
            InGameMainMenuUI.Instance.HideParty();

            InGameMainMenuUI.Instance.ShockMessageTop.CallMessage("You were kicked from the party.", Color.red, true);
        }
        else
        {
            LocalUserInfo.Me.CurrentParty.Members.Remove(data["char_name"].Value);

            InGameMainMenuUI.Instance.RefreshParty();

            InGameMainMenuUI.Instance.ShockMessageTop.CallMessage(data["char_name"].Value + " was kicked from the party.", Color.red, true);
        }
    }

    private void OnActorLeaveParty(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["char_name"].Value + " has left the party");

        LocalUserInfo.Me.CurrentParty.Members.Remove(data["char_name"].Value);

        ActorInfo actor = Game.Instance.CurrentScene.GetActorByName(data["char_name"].Value);

        if (actor != null)
        {
            if (LocalUserInfo.Me.ClientCharacter != actor)
            {
                actor.Instance.MovementController.HideHealth();
            }
            else
            {
                LocalUserInfo.Me.CurrentParty = null;
            }
        }

        
        InGameMainMenuUI.Instance.RefreshParty();

        if (LocalUserInfo.Me.ClientCharacter.Name == data["char_name"].Value)
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage("You have left the party.", Color.red, true);
        }
        else
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(data["char_name"].Value + " has left the party.", Color.red, true);
        }
    }
    
    private void OnActorJoinParty(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["char_name"].Value + " has joined the party");

        if (!LocalUserInfo.Me.CurrentParty.Members.Contains(data["char_name"].Value))
        {
            LocalUserInfo.Me.CurrentParty.Members.Add(data["char_name"].Value);
        }

        ActorInfo actor = Game.Instance.CurrentScene.GetActorByName(data["char_name"].Value);

        if (actor != null)
        {
            if (LocalUserInfo.Me.ClientCharacter == actor)
            {
                ActorInfo otherActor;
                for(int i=0;i<LocalUserInfo.Me.CurrentParty.Members.Count;i++)
                {
                    otherActor = Game.Instance.CurrentScene.GetActorByName(LocalUserInfo.Me.CurrentParty.Members[i]);
                    if (otherActor != null && otherActor.ID != LocalUserInfo.Me.ClientCharacter.ID)
                    {
                        otherActor.Instance.MovementController.ShowHealth();
                    }
                    
                }
            }
            else
            {
                actor.Instance.MovementController.ShowHealth();
            }
        }

        InGameMainMenuUI.Instance.RefreshParty();

        
        if (LocalUserInfo.Me.ClientCharacter.Name == data["char_name"].Value)
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage("You have joined the party!", Color.green, true);
        }
        else
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(data["char_name"].Value + " has joined the party!", Color.green, true);
        }
    }

    private void OnPartyInvitation(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["leader_name"].Value + " has sent an invitation to party");

        InGameMainMenuUI.Instance.AddAcceptDeclineMessage(data["leader_name"].Value + " has invited you to party!", data["leader_name"].Value, SendJoinParty);

        InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage("New party invite!", Color.black, false);
    }



    private void OnKnownInfo(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Recieved info on "+data.ToString());

        LocalUserInfo.Me.AddKnownCharacter(new ActorInfo(data["character"]));

        if (LocalUserInfo.Me.CurrentParty != null)
        {
            if (LocalUserInfo.Me.CurrentParty.Members.Contains(data["character"]["name"].Value))
            {
                InGameMainMenuUI.Instance.RefreshParty();
            }
        }
    }

    private void OnKnownMoveRoom(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent( data["name"].Value + " Moved room to "+ data["room"].Value);

        KnownCharacter knownChar = LocalUserInfo.Me.GetKnownCharacter(data["name"].Value);

        if(knownChar != null)
        {
            knownChar.Info.CurrentRoom = data["room"].Value;
        }

        if (LocalUserInfo.Me.CurrentParty != null)
        {
            if (LocalUserInfo.Me.CurrentParty.Members.Contains(data["name"].Value))
            {
                InGameMainMenuUI.Instance.RefreshParty();
            }
        }
    }

    private void OnKnownLogOut(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["name"].Value + " Logged Out");

        KnownCharacter knownChar = LocalUserInfo.Me.GetKnownCharacter(data["name"].Value);

        if (knownChar != null)
        {
            knownChar.isLoggedIn = false;
        }

        if (LocalUserInfo.Me.CurrentParty != null)
        {
            if (LocalUserInfo.Me.CurrentParty.Members.Contains(data["name"].Value))
            {
                InGameMainMenuUI.Instance.RefreshParty();
            }
        }

        InGameMainMenuUI.Instance.ShockMessageTop.CallMessage(data["name"].Value + " is now OFFLINE.", Color.red, true);
    }

    private void OnKnownLogIn(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["name"].Value + " Logged In");

        KnownCharacter knownChar = LocalUserInfo.Me.GetKnownCharacter(data["name"].Value);

        if (knownChar != null)
        {
            knownChar.isLoggedIn = true;
        }

        if (LocalUserInfo.Me.CurrentParty != null)
        {
            if (LocalUserInfo.Me.CurrentParty.Members.Contains(data["name"].Value))
            {
                InGameMainMenuUI.Instance.RefreshParty();
            }
        }

        InGameMainMenuUI.Instance.ShockMessageTop.CallMessage(data["name"].Value + " is now ONLINE!", Color.green, true);
    }

    private void OnTransaction(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());
    }

    private void OnAbilityGainEXP(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        Ability tempPA = LocalUserInfo.Me.ClientCharacter.GetPrimaryAbility(data["ability"].Value);
        if (tempPA == null)
        {
            tempPA = LocalUserInfo.Me.ClientCharacter.GetCharAbility(data["ability"].Value);
        }
        tempPA.Exp = data["now"].AsInt;

        InGameMainMenuUI.Instance.RefreshPrimaryAbilitiesWindow();
    }

    private void OnAbilityLevelUp(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        bool isCharTalent = data["ability"].Value == "charTalent";
        
        ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["id"].Value);
        if (actor != null)
        {
            if (actor == LocalUserInfo.Me.ClientCharacter) 
            {
                bool isCharAbility = false;
                Ability tempPA = actor.GetPrimaryAbility(data["ability"].Value);
                if (tempPA == null)
                {
                    isCharAbility = true;
                    tempPA = actor.GetCharAbility(data["ability"].Value);
                }

                tempPA.LVL = data["lvl"].AsInt;
                tempPA.Points = data["points"].AsInt;

                if(Content.Instance.GetSpellAtLevel(tempPA.LVL) != null)
                {
                    InGameMainMenuUI.Instance.RefreshSpellArea();
                }

                if (isCharAbility)
                {
                    InGameMainMenuUI.Instance.UpdateCharUpgradeCounter(actor.UnspentCharPerkPoints);
                } 
                else 
                {
                    InGameMainMenuUI.Instance.UpdateUpgradeCounter(actor.UnspentPerkPoints);
                }

                InGameMainMenuUI.Instance.RefreshPrimaryAbilitiesWindow();
                InGameMainMenuUI.Instance.RefreshPALevel(false);
            }
            else
            {
                AudioControl.Instance.Play("sound_reward2");
            }

            if (!isCharTalent) {
                actor.Instance.MasteryUp();
            }
        }
    }

    private void OnAbilityChoosePerk(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        Ability tempAbility = LocalUserInfo.Me.ClientCharacter.GetPrimaryAbility(data["ability"].Value);
        if (tempAbility == null)
        {
            tempAbility = LocalUserInfo.Me.ClientCharacter.GetCharAbility(data["ability"].Value);
        }

        tempAbility.PerkPool.Clear();

        for(int i=0;i<data["pool"].Count;i++)
        {
            tempAbility.PerkPool.Add(data["pool"][i].Value);
        }
    }

    private void OnAbilityGainPerk(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        bool isCharAbility = false;
        Ability tempAbility = LocalUserInfo.Me.ClientCharacter.GetPrimaryAbility(data["ability"].Value);
        if (tempAbility == null)
        {
            isCharAbility = true;
            tempAbility = LocalUserInfo.Me.ClientCharacter.GetCharAbility(data["ability"].Value);
        }

        tempAbility.GainPerk(data["perk"].Value);
        tempAbility.Points--;

        InGameMainMenuUI.Instance.EnableUpgradeCounter();
        if (isCharAbility)
        {
            InGameMainMenuUI.Instance.UpdateCharUpgradeCounter(LocalUserInfo.Me.ClientCharacter.UnspentCharPerkPoints);
        } 
        else 
        {
            InGameMainMenuUI.Instance.UpdateUpgradeCounter(LocalUserInfo.Me.ClientCharacter.UnspentPerkPoints);
        }

        InGameMainMenuUI.Instance.RefreshPrimaryAbilitiesWindow();
    }


    private void OnBuffActivated(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        Enemy tempEnemy = Game.Instance.CurrentScene.GetEnemy(data["target_id"].Value);
        if (tempEnemy != null) //IS MOB
        {
            tempEnemy.AddBuff(data["key"].Value, data["duration"].AsFloat);
        }
        else
        {
            ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["target_id"].Value);
            if (actor != null)
            {
                actor.Instance.AddBuff(data["key"].Value, data["duration"].AsFloat);
            }
        }
    }

    private void OnBuffResisted(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        Enemy tempEnemy = Game.Instance.CurrentScene.GetEnemy(data["target_id"].Value);
        // TODO modify the resist based on data["key"].Value and make this pop more beautiful
        string text = "Resist!";
        Color clr = Color.red;
        if (tempEnemy != null) //IS MOB
        {
            tempEnemy.PopHint(text, clr, new Color(), "");
        }
        else
        {
            ActorInfo actor = Game.Instance.CurrentScene.GetActor(data["target_id"].Value);
            if (actor != null)
            {
                actor.Instance.PopHint(text, clr, "");
            }
        }
    }

    private void OnSpellActivated(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        DevSpell spell = Content.Instance.GetPlayerSpell(data["spell_key"].Value);
        string id = data["char_id"].Value;
        ActorInfo actorInfo = Game.Instance.CurrentScene.GetActor(id);
        if (actorInfo != null)
        {
            actorInfo.Instance.CastSpell(spell);
        }
    }

    private void OnSpellCooldown(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        LocalUserInfo.Me.ClientCharacter.SpellsCooldowns.SetSpellInCooldown(data["spell_key"].Value, data["cooldown"].AsFloat);
    }

    private void OnMobSpellActivated(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data.ToString());

        Enemy tempEnemy = Game.Instance.CurrentScene.GetEnemy(data["mob_id"].Value);

        DevMobSpellBase spell = Content.Instance.GetMobSpell(data["spell_key"].Value);

        if (tempEnemy != null)
        {
            tempEnemy.ActivateSpell(spell);

        }
    }


    #endregion

    #region Emittions

    public void EmitLoadedScene(ActorInfo actorInfo)
    {
        BroadcastEvent("Emitted : LoadedScene");
        LoadingWindowUI.Instance.Leave(this);
        JSONNode node = new JSONClass();

        CurrentSocket.Emit("entered_room", node);

        Game.Instance.LoadPlayerCharacter(actorInfo);
    }

    public void EmitEnteredPortal(string portal)
    {
        BroadcastEvent("Emitted : Entering Portal");
        JSONNode node = new JSONClass();

        node["portal"] = portal;

        CurrentSocket.Emit("entered_portal", node);
    }

    public void EmitMovement(Vector3 pos, float rotDegrees, float velocity)
    {
        JSONNode node = new JSONClass();

        node["x"] = pos.x.ToString();
        node["y"] = pos.y.ToString();
        node["z"] = pos.z.ToString();
        node["angle"].AsFloat = rotDegrees;
        node["velocity"].AsFloat = velocity;

        CurrentSocket.Emit("movement", node);
    }

    public void SendStartedClimbing()
    {
        JSONNode node = new JSONClass();
        CurrentSocket.Emit("started_climbing", node);
    }

    public void SendStoppedClimbing()
    {
        JSONNode node = new JSONClass();
        CurrentSocket.Emit("stopped_climbing", node);
    }

    protected IEnumerator BitchPleaseCoroutine(string requestKey)
    {
        // wait for X frames, to make sure the client doesn't have FPS issues
        for (int i = 0; i < BITCH_WAIT_FRAMES; i++) 
        {
            yield return 0;
        }
        SendBitchPlease(requestKey);
    }

    public void SendBitchPlease(string requestKey)
    {
        JSONNode node = new JSONClass();
        node["key"] = requestKey;
        CurrentSocket.Emit("bitch_please", node);
    }

    public void SendUpdateRoomState(string key, string value)
    {
        JSONNode node = new JSONClass();
        
        node["key"] = key;
        node["value"] = value;
        
        CurrentSocket.Emit("update_room_state", node);
    }

    public void SendChatMessage(string message)
    {
        JSONNode node = new JSONClass();
        node["msg"] = message;
        CurrentSocket.Emit("chatted", node);
    }

    public void SendPartyMessage(string message)
    {
        JSONNode node = new JSONClass();
        node["msg"] = message;
        CurrentSocket.Emit("party_chatted", node);
    }

    public void SendWhisper(string message, string targetName)
    {
        JSONNode node = new JSONClass();
        node["msg"] = message;
        node["to"] = targetName;
        CurrentSocket.Emit("whispered", node);
    }

    public void SendPickedItem(string ItemID)
    {
        //Debug.Log("picking up " + ItemID);
        JSONNode node = new JSONClass();

        node["item_id"] = ItemID;

        CurrentSocket.Emit("picked_item", node);
    }

    public void SendDroppedItem(int slotIndex)
    {
        JSONNode node = new JSONClass();

        node["slot"].AsInt = slotIndex;

        CurrentSocket.Emit("dropped_item", node);
    }

    public void SendDroppedGold(int GoldAmount)
    {
        JSONNode node = new JSONClass();

        node["amount"].AsInt = GoldAmount;

        CurrentSocket.Emit("dropped_gold", node);
    }


    public void SendMovedItem(int fromIndex, int toIndex)
    {
        JSONNode node = new JSONClass();

        node["from"].AsInt = fromIndex;
        node["to"].AsInt = toIndex;

        CurrentSocket.Emit("moved_item", node);
    }

    public void SendEquippedItem(int fromIndex, string Slot)
    {
        JSONNode node = new JSONClass();

        node["from"].AsInt = fromIndex;
        node["to"] = Slot;

        CurrentSocket.Emit("equipped_item", node);
    }

    public void SendUnequippedItem(string fromSlot, int toIndex)
    {
        JSONNode node = new JSONClass();

        node["from"] = fromSlot;
        node["to"].AsInt = toIndex;

        //Debug.Log(node);

        CurrentSocket.Emit("unequipped_item", node);
    }

    public void SendUsedEquip(string fromSlot)
    {
        ItemInfo itemInfo = LocalUserInfo.Me.ClientCharacter.Equipment.GetItem(fromSlot);
        if (itemInfo == null)
        {
            return;
        }
        JSONNode node = new JSONClass();

        node["slot"] = fromSlot;

        if (!string.IsNullOrEmpty(itemInfo.UseSound))
        {
            AudioControl.Instance.Play(itemInfo.UseSound);
        }

        CurrentSocket.Emit("used_equip", node);
    }

    public void SendUsedItem(int inventoryIndex)
    {
        if(LocalUserInfo.Me.ClientCharacter.Inventory.ContentArray[inventoryIndex] == null)
        {
            return;
        }

        JSONNode node = new JSONClass();

        node["slot"] = inventoryIndex.ToString();

        string useSound = LocalUserInfo.Me.ClientCharacter.Inventory.ContentArray[inventoryIndex].UseSound;
        if (!string.IsNullOrEmpty(useSound))
        {
            AudioControl.Instance.Play(useSound);
        }

        CurrentSocket.Emit("used_item", node);
    }

    public void SendDroppedEquip(string fromSlot)
    {
        JSONNode node = new JSONClass();

        node["slot"] = fromSlot;

        CurrentSocket.Emit("dropped_equip", node);
    }

    public void SendMovedEquip(string fromSlot, string toSlot)
    {
        JSONNode node = new JSONClass();

        node["from"] = fromSlot;
        node["to"] = toSlot;

        CurrentSocket.Emit("moved_equip", node);
    }

    public void SendEmote(string type, string emote)
    {
        JSONNode node = new JSONClass();

        node["type"] = type;
        node["emote"] = emote;

        CurrentSocket.Emit("emoted", node);
    }

    public void SendTookDMG(EnemyInfo Info)
    {
        JSONNode node = new JSONClass();

        node["mob_id"] = Info.ID;

        CurrentSocket.Emit("took_dmg", node);
    }

    public void SendWorldDMG(int damage, List<DevPerkMap> perks = null)
    {
        JSONNode node = new JSONClass();

        node["dmg"] = damage.ToString();

        if (perks != null)
        {
            for (int i = 0; i < perks.Count; i++)
            {
                node["perks"][i]["key"] = perks[i].Key;
                node["perks"][i]["value"] = perks[i].Value.ToString();
            }
        }

        CurrentSocket.Emit("take_world_damage", node);
    }

    public void SendLoadedAttack()
    {
        JSONNode node = new JSONClass();
        CurrentSocket.Emit("loaded_attack", node);
    }

    public void SendPreformedAttack(float attackValue, uint attackId)
    {
        JSONNode node = new JSONClass();

        // value might be more than 1 sometimes since we are dealing with floats
        attackValue = Mathf.Min(1f, attackValue); 

        node["load"] = Mathf.FloorToInt(attackValue * 100f).ToString();
        node["attack_id"] = attackId.ToString();

        CurrentSocket.Emit("performed_attack", node);
    }

    public void SendChangedAbility(string ability)
    {
        JSONNode node = new JSONClass();

        node["ability"] = ability;

        CurrentSocket.Emit("changed_ability", node);
    }



    public void SendMobsMove(Dictionary<string, MobMovementData> mobsToUpdate)
    {
        JSONNode node = new JSONClass();
        node["mobs"] = new JSONArray();

        int i = 0;
        foreach (KeyValuePair<string, MobMovementData> pair in mobsToUpdate) 
        {
            node["mobs"][i]["mob_id"] = pair.Key;
            node["mobs"][i]["x"] = pair.Value.position.x.ToString();
            node["mobs"][i]["y"] = pair.Value.position.y.ToString();
            node["mobs"][i]["velocity"] = pair.Value.velocity.ToString();
            i++;
        }

        CurrentSocket.Emit("mobs_moved", node);
    }

    public void SendUsedPrimaryAbility(List<string> targetIDs, uint attackId)
    {
        JSONNode node = new JSONClass();

        for(int i = 0; i < targetIDs.Count; i++)
        {
            node["target_ids"][i] = targetIDs[i];
        }

        node["attack_id"] = attackId.ToString();

        CurrentSocket.Emit("used_ability", node);
    }

    public void SendItemPositions(List<ItemInstance> ItemInstances)
    {
        JSONNode node = new JSONClass();
        node["items"] = new JSONArray();

        for (int i = 0; i < ItemInstances.Count; i++)
        {
            node["items"][i]["x"] = ItemInstances[i].transform.position.x.ToString();
            node["items"][i]["y"] = ItemInstances[i].transform.position.y.ToString();

            node["items"][i]["item_id"] = ItemInstances[i].ID;
        }

        CurrentSocket.Emit("items_locations", node);

    }

    public void SendReleaseDeath()
    {
        JSONNode node = new JSONClass();

        Game.Instance.RemovePlayerCharacter();
        Game.Instance.RemoveAllSceneEntityInstances();

        CurrentSocket.Emit("release_death", node);
    }

    public void SendStuck()
    {
        JSONNode node = new JSONClass();

        Game.Instance.MovingTroughPortal = true;
        CurrentSocket.Emit("stuck", node);
    }

    public void SendQuestStarted(string questID, string npcKey)
    {
        JSONNode node = new JSONClass();

        node["id"] = questID;
        node["npc"] = npcKey;

        CurrentSocket.Emit("quest_started", node);
    }

    public void SendQuestCompleted(string questID, string npcKey)
    {
        JSONNode node = new JSONClass();

        node["id"] = questID;
        node["npc"] = npcKey;

        CurrentSocket.Emit("quest_completed", node);
    }

    public void SendQuestAborted(string questID)
    {
        JSONNode node = new JSONClass();

        node["id"] = questID;

        CurrentSocket.Emit("quest_aborted", node);
    }

    public void SendQuestOK(string okKey, int okValue = 1)
    {
        JSONNode node = new JSONClass();

        node["ok"] = okKey;
        node["value"] = okValue.ToString();

        CurrentSocket.Emit("quest_ok_progress", node);
        Debug.Log(node);
    }


    public void SendJoinParty(string leaderKey)
    {
        JSONNode node = new JSONClass();

        node["leader_name"] = leaderKey;

        CurrentSocket.Emit("join_party", node);
    }

    public void SendCreateParty()
    {
        JSONNode node = new JSONClass();

        CurrentSocket.Emit("create_party", node);
    }

    public void SendLeaveParty()
    {
        JSONNode node = new JSONClass();

        CurrentSocket.Emit("leave_party", node);
    }

    public void SendInviteToParty(string characterName)
    {
        JSONNode node = new JSONClass();

        node["char_name"] = characterName;

        CurrentSocket.Emit("invite_to_party", node);
    }

    public void SendKickFromParty(string characterName)
    {
        JSONNode node = new JSONClass();

        node["char_name"] = characterName;

        CurrentSocket.Emit("kick_from_party", node);
    }

    public void SendChangePartyLeader(string characterName)
    {
        JSONNode node = new JSONClass();

        node["char_name"] = characterName;

        CurrentSocket.Emit("change_party_leader", node);
    }


    public void SendSellVendorItem(string npcKey, int slot, int stack = 1)
    {
        JSONNode node = new JSONClass();

        node["npcKey"] = npcKey;
        node["slot"] = slot.ToString();
        node["stack"] = stack.ToString();

        CurrentSocket.Emit("sell_item", node);
    }

    public void SendBuyVendorItem(string npcKey, int itemIndex, int stack = 1)
    {
        JSONNode node = new JSONClass();

        node["npcKey"] = npcKey;
        node["index"] = itemIndex.ToString();
        node["stack"] = stack.ToString();

        CurrentSocket.Emit("buy_item", node);
    }

    public void SendChoosePerk(string ability, string perk)
    {
        JSONNode node = new JSONClass();

        node["ability"] = ability;
        node["perk"] = perk;

        CurrentSocket.Emit("choose_perk", node);

        InGameMainMenuUI.Instance.DisableUpgradeCounter();
    }

    public void SendUsedSpell(string spellKey, uint attackId)
    {
        JSONNode node = new JSONClass();

        node["spell_key"] = spellKey;
        node["attack_id"] = attackId.ToString();

        CurrentSocket.Emit("used_spell", node);
    }

    public void SendHitSpell(List<string> targetIDs, uint attackId)
    {
        JSONNode node = new JSONClass();

        node["attack_id"] = attackId.ToString();

        for (int i = 0; i < targetIDs.Count; i++)
        {
            node["target_ids"][i] = targetIDs[i];
        }

        Debug.Log(node);

        CurrentSocket.Emit("hit_spell", node);
    }

    public void SendNpcTeleport(string npcKey, string roomKey, bool instance = false)
    {
        JSONNode node = new JSONClass();

        node["npcKey"] = npcKey;
        node["room"] = roomKey;
        if (instance) {
            node["instance"] = "true";
        }

        CurrentSocket.Emit("npc_teleport", node);
    }

    
    public void SendTookSpellDamage(string spellKey, string mobID)
    {
        JSONNode node = new JSONClass();

        node["spell_key"] = spellKey;
        node["mob_id"] = mobID;

        CurrentSocket.Emit("took_spell_dmg", node);
    }

    #endregion

    #region Internal


    protected void BroadcastEvent(string info)
    {
        if (DebugMode)
        {
            Debug.Log(this + " | " + info);
        }
    }


    #endregion
}
