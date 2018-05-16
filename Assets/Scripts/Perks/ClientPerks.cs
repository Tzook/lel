public class ClientPerks
{
    public int MaxHealth = 5;

    public int MaxMana = 5;
    
    public float ManaCost = 1f;

    public float KnockbackModifier = 1f;

    private float _attackSpeed = 1f;
    public float AttackSpeed
    { 
        set 
        {
            _attackSpeed = value;
            if (LocalUserInfo.Me.ClientCharacter.Instance != null)
            {
                LocalUserInfo.Me.ClientCharacter.Instance.InputController.SetAttackSpeed(_attackSpeed);
            }
        }
        get
        {
            return _attackSpeed;
        }
    }

    public float QuestExpBonus = 0f;

    public float QuestGoldBonus = 0f;
    
    public float ShopsDiscount = 0f;
    
    public float SaleModifier = 0.5f;
    
    public float CooldownModifier = 1f;

    public int JumpBonus = 0;

    public int SpeedBonus = 0;
}