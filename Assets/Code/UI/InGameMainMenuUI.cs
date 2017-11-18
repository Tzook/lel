﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class InGameMainMenuUI : MonoBehaviour {

    [SerializeField]
    protected GameObject menuPanel;

    [SerializeField]
    protected ChatboxUI chatPanel;

    [SerializeField]
    protected GameObject optionsPanel;

    [SerializeField]
    protected InventoryUI inventoryPanel;

    [SerializeField]
    protected ItemInfoUI itemInfoPanel;

    [SerializeField]
    protected CharInfoUI m_CharInfoUI;

    [SerializeField]
    RoundValueBarUI HPBar;

    [SerializeField]
    RoundValueBarUI MPBar;

    [SerializeField]
    RoundValueBarUI XPBar;

    [SerializeField]
    BlinkNumberUI LevelUI;

    [SerializeField]
    protected GameObject m_GameUI;

    [SerializeField]
    protected List<Transform> WindowsContainers = new List<Transform>();

    [SerializeField]
    protected EquipmentWindowUI equipmentPanel;

    [SerializeField]
    protected StatsWindowUI statsPanel;

    [SerializeField]
    protected ProgressQuestsWindowUI questsPanel;

    [SerializeField]
    protected CompletedQuestsWindowUI completedQuestsPanel;

    [SerializeField]
    protected AbandonQuestWindowUI AbandonQuestWindow;

    [SerializeField]
    protected CanvasGroup m_DimmerCanvasGroup;

    [SerializeField]
    protected Canvas SubCanvas;

    [SerializeField]
    protected Canvas VendorCanvas;

    [SerializeField]
    Text LastChatMessageText;

    [SerializeField]
    Image PrimaryAbilityImage;

    [SerializeField]
    ChargeAttackBarUI ChargeAttackBar;

    [SerializeField]
    MinilogUI m_Minilog;

    [SerializeField]
    DeathWindowUI DeathWindow;

    [SerializeField]
    GoldDropWindowUI GoldDropWindow;

    [SerializeField]
    QuestRewardsWindowUI QuestRewardWindow;

    [SerializeField]
    PrimaryAbilitiesGridUI PrimaryAbilitiesGrid;

    [SerializeField]
    Image PrimaryAbilityIcon;

    [SerializeField]
    AcceptDeclineWindowUI AcceptDeclineWindow;

    [SerializeField]
    PartyWindowUI PartyWindow;

    [SerializeField]
    public ShockMessageUI ShockMessageCenter;

    [SerializeField]
    public ShockMessageUI ShockMessageTop;

    [SerializeField]
    public PAWindowUI PrimaryAbilityWindow;

    [SerializeField]
    public UpgradeCounterUI UpgradeCounterPanel;

    [SerializeField]
    public MasteryUpgradeWindowUI MasteryUpgradePanel;

    [SerializeField]
    public Image PAExpBar;

    public static InGameMainMenuUI Instance;

    public StatsInfoUI StatsInfo;

    public bool isWindowOpen
    {
        get
        {
            for (int wc = 0; wc < WindowsContainers.Count; wc++)
            {
                for (int i = 0; i < WindowsContainers[wc].childCount; i++)
                {
                    if (WindowsContainers[wc].GetChild(i).gameObject.activeInHierarchy)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public bool isDraggingItem;

    public bool isFullScreenWindowOpen
    {
        get
        {
            return (GoldDropWindow.gameObject.activeInHierarchy || questsPanel.gameObject.activeInHierarchy || completedQuestsPanel.gameObject.activeInHierarchy);
        }
    }

    public ItemUI DraggedSlot;

    public ItemUI HoveredSlot;

    public GameObject CurrentDragged;

    public Coroutine FadeCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isWindowOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                for (int wc = 0; wc < WindowsContainers.Count; wc++)
                {
                    for (int i = 0; i < WindowsContainers[wc].childCount; i++)
                    {
                        WindowsContainers[wc].GetChild(i).gameObject.SetActive(false);
                    }
                }

                chatPanel.onDeselectChat();
                StatsInfo.Hide();
                itemInfoPanel.Hide();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !DialogManager.Instance.inDialog)
            {
                menuPanel.SetActive(true);
            }
        }

        if (Game.Instance.InGame)
        {
            if (Input.GetKeyDown(InputMap.Map["Chat"]))
            {
                chatPanel.ChatClicked();
            }
        }

        if(Game.Instance.InGame && !Game.Instance.InChat)
        {
            
            if (Input.GetKeyDown(InputMap.Map["Inventory"]))
            {
                if (!inventoryPanel.gameObject.activeInHierarchy)
                {
                    inventoryPanel.GetComponent<InventoryUI>().ShowInventory(Game.Instance.ClientCharacter.GetComponent<ActorInstance>().Info);
                }
                else
                {
                    inventoryPanel.GetComponent<InventoryUI>().Hide();
                    itemInfoPanel.Hide();
                }
            }

            if (Input.GetKeyDown(InputMap.Map["Equipment"]))
            {
                if (!equipmentPanel.gameObject.activeInHierarchy)
                {
                    equipmentPanel.Open(Game.Instance.ClientCharacter.GetComponent<ActorInstance>().Info);
                }
                else
                {
                    equipmentPanel.Hide();
                    itemInfoPanel.Hide();
                }
            }

            if (Input.GetKeyDown(InputMap.Map["Stats"]))
            {
                if (!statsPanel.gameObject.activeInHierarchy)
                {
                    statsPanel.Show(Game.Instance.ClientCharacter.GetComponent<ActorInstance>().Info);
                }
                else
                {
                    statsPanel.Hide();
                    StatsInfo.Hide();

                }
            }

            if (Input.GetKeyDown(InputMap.Map["Quests"]))
            {
                if (!questsPanel.gameObject.activeInHierarchy)
                {
                    questsPanel.Show();
                }
                else
                {
                    questsPanel.Hide();
                }
            }

            if (Input.GetKeyDown(InputMap.Map["CompletedQuests"]))
            {
                if (!completedQuestsPanel.gameObject.activeInHierarchy)
                {
                    completedQuestsPanel.Show();
                }
                else
                {
                    completedQuestsPanel.Hide();
                }
            }

            if (Input.GetKeyDown(InputMap.Map["Talents"]))
            {
                if (!PrimaryAbilityWindow.gameObject.activeInHierarchy)
                {
                    ShowPrimaryAbilitiesWindow();
                }
                else
                {
                    HidePrimaryAbilitiesWindow();
                }
            }
        }

    }

    public void Resume()
    {
        menuPanel.SetActive(false);
    }

    public void Logout()
    {
        menuPanel.SetActive(false);
        Game.Instance.LeaveToMainMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowGameUI()
    {
        m_GameUI.SetActive(true);
        ActorInfo info = LocalUserInfo.Me.ClientCharacter;

        RefreshXP(info);
        RefreshHP(info);
        RefreshMP(info);
        RefreshLevel(info);

        PrimaryAbilityImage.sprite = Content.Instance.GetPrimaryAbility(info.CurrentPrimaryAbility.Key).Icon;

        RefreshPAExpBar();

        UpdateUpgradeCounter(info.UnspentPerkPoints);
    }

    public void HideGameUI()
    {
        m_GameUI.SetActive(false);
    }

    public void ShowCharacterInfo(ActorInfo Info)
    {
        if (Info != LocalUserInfo.Me.ClientCharacter)
        {
            m_CharInfoUI.Open(Info);
        }
    }

    public void SetLastChatMessage(string text, Color color)
    {
        LastChatMessageText.text = text;
        LastChatMessageText.color = color;
    }


    public void RefreshInventory()
    {
        if (inventoryPanel.gameObject.activeInHierarchy)
        {
            inventoryPanel.GetComponent<InventoryUI>().RefreshInventory();
        }
    }

    internal void DisableInventoryInput()
    {
        inventoryPanel.DisableInput();
    }

    public void EnableInventoryInput()
    {
        inventoryPanel.EnableInput();
    }


    public void RefreshEquipment()
    {
        equipmentPanel.RefreshEquipment();
    }

    internal void DisableEquipmentInput()
    {
        equipmentPanel.DisableInput();
    }

    public void EnableEquipmentInput()
    {
        equipmentPanel.EnableInput();
    }


    public void RefreshStats()
    {
        statsPanel.Refresh(LocalUserInfo.Me.ClientCharacter);
        RefreshHP();
        RefreshMP();
    }

    
    public void SetHoverSlot(ItemUI item)
    {
        HoveredSlot = item;
        itemInfoPanel.Show(item.CurrentItem);
    }

    public void UnsetHoverSlot()
    {
        HoveredSlot = null;
        itemInfoPanel.Hide();
    }

    public void ForceHideItemInfo()
    {
        itemInfoPanel.Hide();
    }

    public IEnumerator FadeInRoutine()
    {
        m_DimmerCanvasGroup.alpha = 0;
        while(m_DimmerCanvasGroup.alpha < 1f)
        {
            m_DimmerCanvasGroup.alpha += 2f * Time.deltaTime;
            yield return 0;
        }
    }

    public IEnumerator FadeOutRoutine()
    {
        m_DimmerCanvasGroup.alpha = 1;
        while (m_DimmerCanvasGroup.alpha > 0f)
        {
            m_DimmerCanvasGroup.alpha -= 2f * Time.deltaTime;
            yield return 0;
        }
    }

    public Coroutine StartFadeCoroutine(IEnumerator Fade)
    {
        if (FadeCoroutine != null) 
        {
            StopCoroutine(FadeCoroutine);
        }
        FadeCoroutine = StartCoroutine(Fade);
        return FadeCoroutine;
    }

    public void BeginDrag(ItemUI slot)
    {
        DraggedSlot = slot;

        isDraggingItem = true;

        CurrentDragged = ResourcesLoader.Instance.GetRecycledObject("DraggedItem");
        CurrentDragged.GetComponent<Image>().sprite = ResourcesLoader.Instance.GetSprite(slot.CurrentItem.IconKey);
        CurrentDragged.transform.SetParent(transform, false);
        CurrentDragged.transform.SetAsLastSibling();

        Game.Instance.CurrentScene.StartSellMode(slot.CurrentItem);

        if (DragRoutineInstance != null)
        {
            StopCoroutine(DragRoutineInstance);
        }
        DragRoutineInstance = StartCoroutine(DragSlotRoutine(slot));
    }

    protected Coroutine DragRoutineInstance;
    protected virtual IEnumerator DragSlotRoutine(ItemUI slot)
    {
        while (Input.GetMouseButton(0))
        {
            CurrentDragged.transform.position = GameCamera.MousePosition;
            yield return 0;
        }

        DragRoutineInstance = null;

        ReleaseDraggedItem(slot);
    }

    protected void ReleaseDraggedItem(ItemUI slot)
    {
        Game.Instance.CurrentScene.StopSellMode();

        DraggedSlot.UnDrag();
        CurrentDragged.gameObject.SetActive(false);
        CurrentDragged = null;

        isDraggingItem = false;

        int draggedIndex = DraggedSlot.transform.GetSiblingIndex();

        if (Game.Instance.CanUseUI)
        {
            if (DraggedSlot.ParentContainer == inventoryPanel)// -FROM INVENTORY
            {
                if (HoveredSlot != null)
                {
                    if (HoveredSlot.ParentContainer == inventoryPanel) // --TO INVENTORY
                    {
                        if (HoveredSlot != DraggedSlot)
                        {
                            int releasedIndex = HoveredSlot.transform.GetSiblingIndex();
                            SocketClient.Instance.SendMovedItem(draggedIndex, releasedIndex);
                            AudioControl.Instance.Play("sound_clickFast");
                        }

                        HoveredSlot.UnDrag();
                    }
                    else if (HoveredSlot.ParentContainer == equipmentPanel) // --TO EQUIPMENT
                    {
                        if (equipmentPanel.CanEquip(DraggedSlot.CurrentItem, HoveredSlot))
                        {
                            SocketClient.Instance.SendEquippedItem(draggedIndex, HoveredSlot.slotKey);
                            AudioControl.Instance.Play("sound_equip");
                        }
                    }
                }
                else // --TO SPACE
                {
                    if (GameCamera.Instance.CurrentHovered != null 
                        && GameCamera.Instance.CurrentHovered.tag == "NPC" 
                        && GameCamera.Instance.CurrentHovered.GetComponent<NPC>().SellingItems.Count > 0)
                    {
                        NPC tempNPC = GameCamera.Instance.CurrentHovered.GetComponent<NPC>();
                        
                        SocketClient.Instance.SendSellVendorItem(tempNPC.Key, draggedIndex, DraggedSlot.CurrentItem.Stack);
                        AudioControl.Instance.Play("sound_coins");

                        ResourcesLoader.Instance.GetRecycledObject("CoinSplash").transform.position = GameCamera.MousePosition;
                    }
                    else
                    {
                        SocketClient.Instance.SendDroppedItem(draggedIndex);
                        AudioControl.Instance.Play("sound_throw");
                    }
                }
            }
            if (DraggedSlot.ParentContainer == equipmentPanel)// -FROM EQUIPMENT
            {
                if (HoveredSlot != null)
                {
                    if (HoveredSlot.ParentContainer == equipmentPanel) // --TO EQUIPMENT
                    {
                        AudioControl.Instance.Play("sound_equip");
                        //TODO MOVE SLOT
                    }
                    else if (HoveredSlot.ParentContainer == inventoryPanel) // --TO INVENTORY
                    {
                        int releasedIndex = HoveredSlot.transform.GetSiblingIndex();
                        AudioControl.Instance.Play("sound_equip");
                        SocketClient.Instance.SendUnequippedItem(DraggedSlot.slotKey, releasedIndex);
                    }
                }
                else // --TO SPACE
                {
                    SocketClient.Instance.SendDroppedEquip(DraggedSlot.slotKey);
                    AudioControl.Instance.Play("sound_throw");
                }
            }
        }

    }

    public void RefreshXP(ActorInfo info = null)
    {
        if (info == null)
        {
            info = LocalUserInfo.Me.ClientCharacter;
        }

        XPBar.SetValue(info.EXP / (info.NextLevelXP * 1f));
    }

    public void RefreshHP(ActorInfo info = null)
    {
        if (info == null)
        {
            info = LocalUserInfo.Me.ClientCharacter;
        }

        HPBar.SetValue(info.CurrentHealth / (info.MaxHealth * 1f));
    }

    public void RefreshMP(ActorInfo info = null)
    {
        if (info == null)
        {
            info = LocalUserInfo.Me.ClientCharacter;
        }

        MPBar.SetValue(info.CurrentMana / (info.MaxMana * 1f));
    }

    public void RefreshLevel(ActorInfo info = null)
    {
        if (info == null)
        {
            info = LocalUserInfo.Me.ClientCharacter;
        }

        LevelUI.SetValue(info.LVL.ToString());
    }

    public void StartChargingAttack()
    {
        ChargeAttackBar.StartCharging(Game.Instance.ClientCharacter.transform); 
    }

    public void SetChargeAttackValue(float val)
    {
        ChargeAttackBar.SetValue(val);
    }

    public void StopChargingAttack()
    {
        ChargeAttackBar.StopCharging();
    }

    public void SetCurrentCamera(Camera cam)
    {
        GetComponent<Canvas>().worldCamera = cam;
        SubCanvas.worldCamera = cam;
    }

    public void MinilogMessage(string message)
    {
        m_Minilog.AddMessage(message);
    }

    public void ShowDeathWindow()
    {
        DeathWindow.gameObject.SetActive(true);
    }

    public void RefreshQuestProgress()
    {
        questsPanel.Refresh();
    }

    public void RefreshCompletedQuestProgress()
    {
        completedQuestsPanel.Refresh();
    }

    public void RecieveQuestReward(Quest quest, string npcKey)
    {
        QuestRewardWindow.Show(quest, npcKey);
    }

    public void AbandonQuest(Quest quest)
    {
        AbandonQuestWindow.Show(quest);
    }

    public void ShowPrimaryAbilities()
    {
        PrimaryAbilitiesGrid.Show();
    }
    
    public void HidePrimaryAbilities()
    {
        PrimaryAbilitiesGrid.Hide();
    }

    public void TogglePrimaryAbilities()
    {
        if(PrimaryAbilitiesGrid.gameObject.activeInHierarchy)
        {
            HidePrimaryAbilities();
        }
        else
        {
            ShowPrimaryAbilities();
        }
        
    }

    public void RefreshCurrentPrimaryAbility()
    {
        PrimaryAbilityIcon.sprite = Content.Instance.GetPrimaryAbility(LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.Key).Icon;
        RefreshPAExpBar();
    }

    public void AddAcceptDeclineMessage(string content,string key, Action<string> acceptCallback)
    {
        AcceptDeclineWindow.AddMessage(content,key, acceptCallback);
    }

    public void ShowParty()
    {
        PartyWindow.gameObject.SetActive(true);
    }

    public void HideParty()
    {
        PartyWindow.gameObject.SetActive(false);
    }

    public void RefreshParty()
    {
        if (PartyWindow.gameObject.activeInHierarchy)
        {
            PartyWindow.Refresh();
        }
    }

    public void ShowVendorPanel(string itemKey, NPC Vendor)
    {
        VendorCanvas.GetComponent<VendorPanelUI>().Show(itemKey, Vendor);
    }

    public void HideVendorPanel()
    {
        VendorCanvas.GetComponent<VendorPanelUI>().Hide();
    }

    public void ShowPrimaryAbilitiesWindow()
    {
        PrimaryAbilityWindow.Show();
    }

    public void HidePrimaryAbilitiesWindow()
    {
        PrimaryAbilityWindow.Hide();
    }

    public void RefreshPrimaryAbilitiesWindow()
    {
        if (PrimaryAbilityWindow.gameObject.activeInHierarchy)
        {
            PrimaryAbilityWindow.RefreshWindow();
        }

        RefreshPAExpBar();
    }

    public void UpdateUpgradeCounter(int Value)
    {
        UpgradeCounterPanel.SetValue(Value);
    }

    public void EnableUpgradeCounter()
    {
        UpgradeCounterPanel.Interactable = true;
    }

    public void DisableUpgradeCounter()
    {
        UpgradeCounterPanel.Interactable = false;
    }


    public void ShowMasteryUpgradeWindow()
    {
        MasteryUpgradePanel.ShowLatest();
    }

    public void HideMasteryUpgradeWindow()
    {
        MasteryUpgradePanel.Hide();
    }

    public void ChooseMasteryUpgradePerk(string perkKey)
    {
        MasteryUpgradePanel.DisableButtons();
        SocketClient.Instance.SendChoosePerk(MasteryUpgradePanel.CurrentAbility.Key, perkKey);
    }

    public void RefreshPAExpBar()
    {
        PAExpBar.fillAmount = (LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.Exp*1f) / LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.NextLevelXP;
    }
}
