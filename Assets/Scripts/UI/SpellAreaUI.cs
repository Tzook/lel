using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAreaUI : MonoBehaviour
{
    [SerializeField]
    Transform SpellContainer;

    [SerializeField]
    ParticleSystem BurstParticles;

    int LastAmountOfSpells;

    public void Refresh(bool AbilitySwitch = false)
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        StopAllCoroutines();

        StartCoroutine(RefreshRoutine(AbilitySwitch));
    }

    IEnumerator RefreshRoutine(bool AbilitySwitch = false)
    {

        LastAmountOfSpells = SpellContainer.childCount;

        Clear();

        DevPrimaryAbility ability = Content.Instance.GetPrimaryAbility(LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.Key);

        List<DevSpell> AvailableSpells = new List<DevSpell>();

        for (int i = 0; i < ability.Spells.Count; i++)
        {
            if (ability.Spells[i].Level <= LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.LVL)
            {
                AvailableSpells.Add(ability.Spells[i]);
            }
        }

        if (AvailableSpells.Count > 0)
        {

            GameObject tempObj;

            for (int i = 0; i < AvailableSpells.Count - 1; i++)
            {
                tempObj = ResourcesLoader.Instance.GetRecycledObject("SpellBox");
                tempObj.transform.SetParent(SpellContainer, false);
                tempObj.GetComponent<SpellBoxUI>().Set(AvailableSpells[i]);

                yield return 0.1f;
            }

            if (AbilitySwitch)
            {
                tempObj = ResourcesLoader.Instance.GetRecycledObject("SpellBox");
                tempObj.transform.SetParent(SpellContainer, false);
                tempObj.GetComponent<SpellBoxUI>().Set(AvailableSpells[AvailableSpells.Count - 1]);
            }
            else
            {
                if (LastAmountOfSpells < AvailableSpells.Count)
                {
                    tempObj = ResourcesLoader.Instance.GetRecycledObject("SpellBox");
                    tempObj.transform.SetParent(SpellContainer, false);
                    tempObj.GetComponent<SpellBoxUI>().Set(AvailableSpells[AvailableSpells.Count - 1]);
                    tempObj.GetComponent<SpellBoxUI>().Hide();

                    StartCoroutine(GainEffectRoutine(tempObj.transform));

                    yield return new WaitForSeconds(0.5f);

                    tempObj.GetComponent<SpellBoxUI>().Show();
                    BurstParticles.transform.position = tempObj.transform.position;
                    BurstParticles.Play();
                }
            }
        }
    }

    public void RefreshMana()
    {
        for(int i=0;i<SpellContainer.childCount;i++)
        {
            SpellContainer.GetChild(i).GetComponent<SpellBoxUI>().RefreshMana();
        }
    }

    void Clear()
    {
        while(SpellContainer.childCount > 0)
        {
            SpellContainer.GetChild(0).gameObject.SetActive(false);
            SpellContainer.GetChild(0).transform.SetParent(transform);
        }
    }

    IEnumerator GainEffectRoutine(Transform targetTransform)
    {
        GameObject tempBlob = ResourcesLoader.Instance.GetRecycledObject("PowerBlobEffect");

        yield return tempBlob.GetComponent<SplineFloatEffect>().LaunchRoutine(LocalUserInfo.Me.ClientCharacter.Instance.transform, targetTransform, 1f, Random.Range(-3f, 3f));
    }

    public SpellBoxUI GetSpellBox(DevSpell withSpell)
    {
        SpellBoxUI tempBox;
        for(int i=0;i<SpellContainer.childCount;i++)
        {
            tempBox = SpellContainer.GetChild(i).GetComponent<SpellBoxUI>();

            if(tempBox != null)
            {
                return tempBox;
            }
        }

        return null;
    }

    public void ActivatedSpell(string spellKey)
    {
        GetSpellBox(Content.Instance.GetPlayerSpell(spellKey)).Activated();
    }
}
