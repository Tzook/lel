using UnityEngine;
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
    protected CanvasGroup m_DimmerCanvasGroup;

    [SerializeField]
    protected Canvas SubCanvas;

    [SerializeField]
    Text LastChatMessageText;

    [SerializeField]
    Image PrimaryAbilityImage;

    [SerializeField]
    ChargeAttackBarUI ChargeAttackBar;

    [SerializeField]
    MinilogUI m_Minilog;

    public static InGameMainMenuUI Instance;

    public StatsInfoUI StatsInfo;

    public bool isWindowOpen
    {
        set
        {

        }
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

    public ItemUI DraggedSlot;

    public ItemUI HoveredSlot;


    public GameObject CurrentDragged;

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

                Game.Instance.InChat = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuPanel.SetActive(true);
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

            if (Input.GetKeyDown(InputMap.Map["Chat"]))
            {
                if (!chatPanel.gameObject.activeInHierarchy)
                {
                    chatPanel.Open();
                }
            }

            if (Input.GetKeyDown(InputMap.Map["Stats"]))
            {
                if (!statsPanel.gameObject.activeInHierarchy)
                {
                    statsPanel.GetComponent<StatsWindowUI>().Show(Game.Instance.ClientCharacter.GetComponent<ActorInstance>().Info);
                }
                else
                {
                    statsPanel.GetComponent<StatsWindowUI>().Hide();
                    InGameMainMenuUI.Instance.StatsInfo.Hide();

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
        ActorInfo info = Game.Instance.CurrentScene.ClientCharacter;

        RefreshXP(info);
        RefreshHP(info);
        RefreshMP(info);
        RefreshLevel(info);

        PrimaryAbilityImage.sprite = Content.Instance.GetPrimaryAbility(info.CurrentPrimaryAbility).Icon;
    }

    public void HideGameUI()
    {
        m_GameUI.SetActive(false);
    }

    public void ShowCharacterInfo(ActorInfo Info)
    {
        m_CharInfoUI.Open(Info);
    }

    public void SetLastChatMessage(string text)
    {
        LastChatMessageText.text = text;
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

    public void BeginDrag(ItemUI slot)
    {
        DraggedSlot = slot;



        CurrentDragged = ResourcesLoader.Instance.GetRecycledObject("DraggedItem");
        CurrentDragged.GetComponent<Image>().sprite = ResourcesLoader.Instance.GetSprite(slot.CurrentItem.IconKey);
        CurrentDragged.transform.SetParent(transform, false);
        CurrentDragged.transform.SetAsLastSibling();

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
        DraggedSlot.UnDrag();
        CurrentDragged.gameObject.SetActive(false);
        CurrentDragged = null;

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
                    SocketClient.Instance.SendDroppedItem(draggedIndex);
                    AudioControl.Instance.Play("sound_throw");
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
            info = Game.Instance.CurrentScene.ClientCharacter;
        }

        XPBar.SetValue(info.EXP / (info.NextLevelXP * 1f));
    }

    public void RefreshHP(ActorInfo info = null)
    {
        if (info == null)
        {
            info = Game.Instance.CurrentScene.ClientCharacter;
        }

        HPBar.SetValue(info.CurrentHealth / (info.MaxHealth * 1f));
    }

    public void RefreshMP(ActorInfo info = null)
    {
        if (info == null)
        {
            info = Game.Instance.CurrentScene.ClientCharacter;
        }

        MPBar.SetValue(info.CurrentMana / (info.MaxMana * 1f));
    }

    public void RefreshLevel(ActorInfo info = null)
    {
        if (info == null)
        {
            info = Game.Instance.CurrentScene.ClientCharacter;
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
}
