using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Rendering;

public class ActorInstance : MonoBehaviour
{

    #region References

    public Transform TorsoBone;

    [SerializeField]
    protected SpriteRenderer m_Head;

    [SerializeField]
    protected SpriteRenderer m_Chest;

    [SerializeField]
    protected SpriteRenderer m_RightArm;
    [SerializeField]
    protected SpriteRenderer m_RightElbow;
    [SerializeField]
    protected SpriteRenderer m_RightFist;

    [SerializeField]
    protected SpriteRenderer m_LeftArm;
    [SerializeField]
    protected SpriteRenderer m_LeftElbow;
    [SerializeField]
    protected SpriteRenderer m_LeftFist;

    [SerializeField]
    protected SpriteRenderer m_RightLeg;
    [SerializeField]
    protected SpriteRenderer m_RightKnee;
    [SerializeField]
    protected SpriteRenderer m_RightFoot;

    [SerializeField]
    protected SpriteRenderer m_LeftLeg;
    [SerializeField]
    protected SpriteRenderer m_LeftKnee;
    [SerializeField]
    protected SpriteRenderer m_LeftFoot;

    [SerializeField]
    protected SpriteRenderer m_Hair;
    [SerializeField]
    protected SpriteRenderer m_HairBack;

    [SerializeField]
    protected SpriteRenderer m_Eyes;
    [SerializeField]
    protected SpriteRenderer m_Nose;
    [SerializeField]
    protected SpriteRenderer m_Mouth;

    [SerializeField]
    protected SpriteRenderer m_Hat;

    [SerializeField]
    public SpriteRenderer Weapon;


    [SerializeField]
    public GameObject NameLabel;

    [SerializeField]
    protected MessageBubbleUI MessageBubble;

    [SerializeField]
    Animator Anim;

    [SerializeField]
    public SortingGroup SortingGroup;

    #endregion

    #region Public Parameters

    public ActorInfo Info;
    public ActorMovement MovementController { protected set; get; }

    public bool nameHidden = false;

    #endregion

    #region Public Methods

    public void RegisterInfo(ActorInfo info)
    {
        this.Info = info;
        this.Info.Instance = this;
    }

    public void Reset()
    {
        UpdateVisual(new ActorInfo());
    }

    public void SetOpacity(float fValue)
    {
        Color targetCLR = new Color(Color.white.r, Color.white.g, Color.white.b, fValue);

        m_Head.color = targetCLR;
        m_Eyes.color = targetCLR;
        m_Nose.color = targetCLR;
        m_Mouth.color = targetCLR;
        m_Hair.color = targetCLR;
        m_HairBack.color = targetCLR;

        m_Hat.color = targetCLR;
        m_Chest.color = targetCLR;

        m_RightArm.color = targetCLR;
        m_RightElbow.color = targetCLR;
        m_RightFist.color = targetCLR;
        m_LeftArm.color = targetCLR;
        m_LeftElbow.color = targetCLR;
        m_LeftFist.color = targetCLR;

        m_LeftLeg.color = targetCLR;
        m_LeftKnee.color = targetCLR;
        m_LeftFoot.color = targetCLR;
        m_RightLeg.color = targetCLR;
        m_RightKnee.color = targetCLR;
        m_RightFoot.color = targetCLR;

        Weapon.color = targetCLR;

    }

    #region Update Looks

    public void UpdateVisual(ActorInfo info)
    {
        RegisterInfo(info);

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        UpadeNameLabel();
        UpdateSkin();
        UpdateFace();
        UpdateHair();
        UpdateEquipment();
    }

    protected void UpadeNameLabel()
    {
        if (!nameHidden)
        {
            NameLabel.SetActive(true);
            NameLabel.transform.GetChild(0).GetComponent<Text>().text = Info.Name;
        }
        else
        {
            NameLabel.SetActive(false);
        }
    }

    protected void UpdateSkin()
    {
        if (Info.Gender == Gender.Male)
        {
            if (Info.SkinColor == 0)
            {
                m_Head.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_head");
                m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_torso");

                m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_arm");
                m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_arm");

                m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_elbow");
                m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_elbow");

                m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hand");
                m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hand");

                m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hip");
                m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hip");

                m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_knee");
                m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_knee");

                m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_foot");
                m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_foot");
            }
            else if (Info.SkinColor == 1)
            {
                m_Head.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_head_brown");
                m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_torso_brown");

                m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_arm_brown");
                m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_arm_brown");

                m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_elbow_brown");
                m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_elbow_brown");

                m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hand_brown");
                m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hand_brown");

                m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hip_brown");
                m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hip_brown");

                m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_knee_brown");
                m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_knee_brown");

                m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_foot_brown");
                m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_foot_brown");
            }
            else if (Info.SkinColor == 2)
            {
                m_Head.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_head_black");
                m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_torso_black");

                m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_arm_black");
                m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_arm_black");

                m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_elbow_black");
                m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_elbow_black");

                m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hand_black");
                m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hand_black");

                m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hip_black");
                m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hip_black");

                m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_knee_black");
                m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_knee_black");

                m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_foot_black");
                m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_foot_black");
            }
        }
        else
        {
            if (Info.SkinColor == 0)
            {
                m_Head.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_head");
                m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_torso");

                m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm");
                m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm");

                m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow");
                m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow");

                m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand");
                m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand");

                m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip");
                m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip");

                m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee");
                m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee");

                m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot");
                m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot");
            }
            else if (Info.SkinColor == 1)
            {
                m_Head.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_head_brown");
                m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_torso_brown");

                m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm_brown");
                m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm_brown");

                m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow_brown");
                m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow_brown");

                m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand_brown");
                m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand_brown");

                m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip_brown");
                m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip_brown");

                m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee_brown");
                m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee_brown");

                m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot_brown");
                m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot_brown");
            }
            else if (Info.SkinColor == 2)
            {
                m_Head.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_head_black");
                m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_torso_black");

                m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm_black");
                m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm_black");

                m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow_black");
                m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow_black");

                m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand_black");
                m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand_black");

                m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip_black");
                m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip_black");

                m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee_black");
                m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee_black");

                m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot_black");
                m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot_black");
            }
        }
    }

    protected void UpdateFace()
    {
        m_Eyes.sprite = ResourcesLoader.Instance.GetSprite(Info.Eyes);
        m_Nose.sprite = ResourcesLoader.Instance.GetSprite(Info.Nose);
        m_Mouth.sprite = ResourcesLoader.Instance.GetSprite(Info.Mouth);
        m_Hair.sprite = ResourcesLoader.Instance.GetSprite(Info.Hair);
    }

    protected void UpdateHair()
    {
        Sprite HairBackSprite = ResourcesLoader.Instance.GetSprite(Info.Hair + "_back");
        if (HairBackSprite != null)
        {
            m_HairBack.sprite = HairBackSprite;
        }
        else
        {
            m_HairBack.sprite = null;
        }
    }

    protected void UpdateEquipment()
    {
        m_Hat.sprite = null;
        Weapon.sprite = null;

        UpdateItem(Info.Equipment.Head);
        UpdateItem(Info.Equipment.Chest);
        UpdateItem(Info.Equipment.Gloves);
        UpdateItem(Info.Equipment.Legs);
        UpdateItem(Info.Equipment.Shoes);
        UpdateItem(Info.Equipment.Weapon);
    }

    private void UpdateItem(ItemInfo item)
    {
        if (item == null)
        {
            return;
        }

        string key;
        for(int i=0;i<item.Sprites.Count;i++)
        {
            key = item.Sprites.Keys.ElementAt(i);
            SetSprites(key, item.Sprites[key]);
        }
    }

    private void SetSprites(string key, string spriteKey)
    {
        if (Info.Gender == Gender.Female)
        {
            spriteKey = spriteKey.Replace("male","female");
        }

        switch (key)
        {
            case "head":
                {
                    m_Head.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "chest":
                {
                    m_Chest.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "arm":
                {
                    m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    m_RightArm.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "elbow":
                {
                    m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "hand":
                {
                    m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    m_RightFist.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "hip":
                {
                    m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "knee":
                {
                    m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "foot":
                {
                    m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "hat":
                {
                    m_Hat.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
            case "weapon":
                {
                    Weapon.sprite = ResourcesLoader.Instance.GetSprite(spriteKey);
                    break;
                }
        }
    }

    #endregion


    public void ChatBubble(string Message)
    {
        if (Message.Length < 65)
        {
            MessageBubble.GetComponent<MessageBubbleUI>().PopMessage(Message);
        }
    }

    public void SetElementsLayer(string layer = "Default", int minLevel = 0, Material matType = null)
    {
        SortingGroup.sortingLayerName = layer;

        SetElementLayer(m_Head, layer, minLevel, matType);
        SetElementLayer(m_Chest, layer, minLevel, matType);
        SetElementLayer(m_Eyes, layer, minLevel, matType);
        SetElementLayer(m_Nose, layer, minLevel, matType);
        SetElementLayer(m_Mouth, layer, minLevel, matType);
        SetElementLayer(m_Hat, layer, minLevel, matType);
        SetElementLayer(m_Hair, layer, minLevel, matType);
        SetElementLayer(m_HairBack, layer, minLevel, matType);
        SetElementLayer(m_RightArm, layer, minLevel, matType);
        SetElementLayer(m_RightElbow, layer, minLevel, matType);
        SetElementLayer(m_RightFist, layer, minLevel, matType);
        SetElementLayer(m_LeftArm, layer, minLevel, matType);
        SetElementLayer(m_LeftElbow, layer, minLevel, matType);
        SetElementLayer(m_LeftFist, layer, minLevel, matType);
        SetElementLayer(m_RightLeg, layer, minLevel, matType);
        SetElementLayer(m_RightKnee, layer, minLevel, matType);
        SetElementLayer(m_RightFoot, layer, minLevel, matType);
        SetElementLayer(m_LeftLeg, layer, minLevel, matType);
        SetElementLayer(m_LeftKnee, layer, minLevel, matType);
        SetElementLayer(m_LeftFoot, layer, minLevel, matType);
        SetElementLayer(Weapon, layer, minLevel, matType);
    }


    void SetElementLayer(SpriteRenderer m_Renderer, string layer = "Default", int layerPositionAddition = 0, Material matType = null)
    {
        m_Renderer.sortingLayerName = layer;
        m_Renderer.sortingOrder += layerPositionAddition;

        if (matType == null)
        {
            m_Renderer.material = ResourcesLoader.Instance.UnlitSprite;
        }
        else
        {
            m_Renderer.material = matType;
        }
    }

    void SetElementGame(SpriteRenderer m_Renderer)
    {
        m_Renderer.sortingLayerName = "Sprites";
        m_Renderer.sortingOrder--;
        m_Renderer.material = ResourcesLoader.Instance.LitSprite;
    }

    public void SetRenderingLayer(int iLayer, Transform body = null)
    {
        if(body == null)
        {
            body = this.transform;
        }

        if (body.gameObject.layer != 2) // Wont break "Ignore raycast" layer (Maybe a good reason to make this function work differently)
        {
            body.gameObject.layer = iLayer;
        }

        for (int i = 0; i < body.childCount; i++)
        {
            SetRenderingLayer(iLayer, body.GetChild(i));
        }
    }

    public void HideName()
    {
        nameHidden = true;
        NameLabel.SetActive(false);
    }

    public void ShowName()
    {
        nameHidden = true;
        NameLabel.SetActive(true);
    }

    public void AttemptPickUp()
    {
        ItemInstance tempItem;
        for (int i = 0; i < Game.Instance.CurrentScene.SceneItemsCount; i++)
        {
            string itemKey = Game.Instance.CurrentScene.Items.Keys.ElementAt(i);
            ItemInstance item = Game.Instance.CurrentScene.Items[itemKey];
            tempItem = item.GetComponent<ItemInstance>();

            if (Vector2.Distance(tempItem.transform.position, transform.position) < 0.4f)
            {
                SocketClient.Instance.SendPickedItem(itemKey);
            }
        }

    }

    public void PickUpItem(string instanceID)
    {
        if (LocalUserInfo.Me.ClientCharacter.Instance == this)
        {
            AudioControl.Instance.Play("sound_item");
        }

        StartCoroutine(PickingUpItemRoutine(instanceID));
    }

    public void AddItem(int slot, ItemInfo item)
    {
        Info.Inventory.AddItemAt(slot, item);

        InGameMainMenuUI.Instance.MinilogMessage("Picked '" + item.Name + "'");

        LocalUserInfo.Me.ClientCharacter.UpdateProgress(item.Key, Info.Inventory.GetInventoryCounts()[item.Key]);

        InGameMainMenuUI.Instance.RefreshInventory();
    }

    public void ChangeItemStack(int slot, int stack)
    {
        ItemInfo item = Info.Inventory.GetItemAt(slot);
        int stackPicked = stack - item.Stack;
        InGameMainMenuUI.Instance.MinilogMessage("Picked up " + stackPicked.ToString("N0") + " '" + item.Name + "'s (" + stack.ToString("N0") + ")");
        
        Info.Inventory.ChangeItemStack(slot, stack);

        InGameMainMenuUI.Instance.RefreshInventory();
    }
  
    internal void Hurt()
    {
        Anim.SetInteger("HurtType", Random.Range(0, 3));
        Anim.SetTrigger("Hurt");
    }

    public void Death()
    {
        Anim.SetTrigger("Kill");
        Anim.SetBool("isDead", true);

        PlayEyesEmote("cry");
        PlayMouthEmote("angry");
    }

    public void LevelUp()
    {
        GameObject temObj = ResourcesLoader.Instance.GetRecycledObject("LevelUpEffect");

        temObj.transform.position = transform.position;

        temObj.GetComponent<LevelUpEffect>().Play();
    }

    public void PopHint(string text, Color clr)
    {
        GameObject pop = ResourcesLoader.Instance.GetRecycledObject("PopHint");
        pop.transform.position = transform.position + new Vector3(0f,1f,0f);
        pop.GetComponent<PopText>().Pop(text, clr);
    }

    private IEnumerator PickingUpItemRoutine(string instanceID)
    {
        GameObject itemObject = Game.Instance.CurrentScene.Items[instanceID].gameObject;

        Vector3 itemInitPos = itemObject.transform.position;
        float rndHeight = UnityEngine.Random.Range(-2f, 2f);

        Game.Instance.CurrentScene.Items.Remove(instanceID);

        itemObject.GetComponent<ItemInstance>().Collect();

        float t = 0f;
        while(t<1f)
        {
            t += 2f * Time.deltaTime;

            itemObject.transform.position = Game.SplineLerp(itemInitPos, transform.position, rndHeight, t);
            

            yield return 0;
        }

        
    }

    public void PlayMouthEmote(string emoteKey)
    {
        if (MouthEmoteInstance != null)
        {
            StopCoroutine(MouthEmoteInstance);
        }

        MouthEmoteInstance = StartCoroutine(MouthEmoteRoutine(emoteKey));
    }

    public Coroutine MouthEmoteInstance;
    private IEnumerator MouthEmoteRoutine(string emoteKey)
    {
        m_Mouth.sprite = ResourcesLoader.Instance.GetSprite(Info.Mouth + "_" + emoteKey);

        yield return new WaitForSeconds(3f);

        m_Mouth.sprite = ResourcesLoader.Instance.GetSprite(Info.Mouth);

        MouthEmoteInstance = null;
    }

    public void PlayEyesEmote(string emoteKey)
    {
        if (EyesEmoteInstance != null)
        {
            StopCoroutine(EyesEmoteInstance);
        }

        EyesEmoteInstance = StartCoroutine(EyesEmoteRoutine(emoteKey));
    }

    public Coroutine EyesEmoteInstance;
    private IEnumerator EyesEmoteRoutine(string emoteKey)
    {
        m_Eyes.sprite = ResourcesLoader.Instance.GetSprite(Info.Eyes + "_" + emoteKey);

        yield return new WaitForSeconds(3f);

        m_Eyes.sprite = ResourcesLoader.Instance.GetSprite(Info.Eyes);

        EyesEmoteInstance = null;
    }

    public void StartCombatMode()
    {
        if (CombatModeInstance != null)
        {
            StopCoroutine(CombatModeInstance);
        }

        CombatModeInstance = StartCoroutine(CombatModeRoutine());
    }

    public Coroutine CombatModeInstance { get; private set; }
    private IEnumerator CombatModeRoutine()
    {
        Anim.SetBool("Combat", true);
        yield return new WaitForSeconds(3f);
        Anim.SetBool("Combat", false);
    }

    public void LoadAttack(string ability)
    {
        Anim.SetInteger("AttackType", Random.Range(0, 3));
        Anim.SetBool("Charging", true);
        Debug.Log(Anim.GetBool("Charging"));
    }

    public void PreformAttack(string ability, float AttackValue)
    {
        Anim.SetBool("Charging", false);

        StartCombatMode();
    }

    public void BendBow()
    {
        Weapon.sprite = ResourcesLoader.Instance.GetSprite(Info.Equipment.Weapon.Sprites.ElementAt(0).Value + "_charged");
    }

    public void UnbendBow()
    {
        Weapon.sprite = ResourcesLoader.Instance.GetSprite(Info.Equipment.Weapon.Sprites.ElementAt(0).Value);
    }

    #endregion

}
