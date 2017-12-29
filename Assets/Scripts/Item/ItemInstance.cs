using UnityEngine;
using System.Collections;

public class ItemInstance : MonoBehaviour {

    public ItemInfo Info;

    public string ID;
    
    // this item's owner name (the one who can pick it up)
    public string Owner; 

    [SerializeField]
    SpriteRenderer m_Renderer;

    [SerializeField]
    Animator m_Animator;

    public void SetInfo(ItemInfo info, string id, string owner)
    {
        this.Info = info;
        this.ID = id;
        this.Owner = owner;

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        m_Renderer.sprite = ResourcesLoader.Instance.GetSprite(Info.IconKey);
        m_Animator.SetTrigger("Spawn");
    }

    public void Collect()
    {
        m_Animator.SetTrigger("Collect");
    }

    public void CanShut()
    {
        this.gameObject.SetActive(false);
    }

}
