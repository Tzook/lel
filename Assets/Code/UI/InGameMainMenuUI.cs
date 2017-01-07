using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

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
    protected EquipmentWindowUI equipmentPanel;

    [SerializeField]
    protected CanvasGroup m_DimmerCanvasGroup;

    public static InGameMainMenuUI Instance;

    public bool isWindowOpen
    {
        set
        {

        }
        get
        {
            for(int i=0;i<transform.childCount;i++)
            {
                if(transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    return true;
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
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
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

        if (Input.GetKeyDown(InputMap.Map["Inventory"]))
        {
            if (!inventoryPanel.gameObject.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                inventoryPanel.GetComponent<InventoryUI>().ShowInventory(Game.Instance.ClientCharacter.GetComponent<ActorInstance>().Info);
            }
            else
            {
                inventoryPanel.GetComponent<InventoryUI>().Hide();
            }
        }

        if (Input.GetKeyDown(InputMap.Map["Equipment"]))
        {
            if (!equipmentPanel.gameObject.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                equipmentPanel.Open(Game.Instance.ClientCharacter.GetComponent<ActorInstance>().Info);
            }
            else
            {
                equipmentPanel.Hide();
            }
        }

        if (Input.GetKeyDown(InputMap.Map["Chat"]))
        {
            if (!chatPanel.gameObject.activeInHierarchy && Game.Instance.InGame && !Game.Instance.InChat)
            {
                chatPanel.Open();
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

    public void ShowCharacterInfo(ActorInfo Info)
    {
        m_CharInfoUI.Open(Info);
    }


    public void RefreshInventory()
    {
        inventoryPanel.GetComponent<InventoryUI>().RefreshInventory();
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

                    }

                    HoveredSlot.UnDrag();
                }
                else if (HoveredSlot.ParentContainer == equipmentPanel) // --TO EQUIPMENT
                {
                    if (equipmentPanel.CanEquip(DraggedSlot.CurrentItem, HoveredSlot))
                    {
                        SocketClient.Instance.SendEquippedItem(draggedIndex, HoveredSlot.slotKey);
                    }
                }
            }
            else // --TO SPACE
            {
                SocketClient.Instance.SendDroppedItem(draggedIndex);
            }
        }
        if (DraggedSlot.ParentContainer == equipmentPanel)// -FROM EQUIPMENT
        {
            if (HoveredSlot != null)
            {
                if (HoveredSlot.ParentContainer == equipmentPanel) // --TO EQUIPMENT
                {
                    //TODO MOVE SLOT
                }
                else if (HoveredSlot.ParentContainer == inventoryPanel) // --TO INVENTORY
                {
                    int releasedIndex = HoveredSlot.transform.GetSiblingIndex();

                    SocketClient.Instance.SendUnequippedItem(DraggedSlot.slotKey, releasedIndex);
                }
            }
            else // --TO SPACE
            {
                SocketClient.Instance.SendDroppedEquip(DraggedSlot.slotKey);
            }
        }

    }

}
