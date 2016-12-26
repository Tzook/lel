﻿using UnityEngine;
using System.Collections;

public class ItemInstance : MonoBehaviour {

    public ItemInfo Info;

    [SerializeField]
    SpriteRenderer m_Renderer;

    [SerializeField]
    Animator m_Animator;

    public void SetInfo(ItemInfo info)
    {
        this.Info = info;
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
