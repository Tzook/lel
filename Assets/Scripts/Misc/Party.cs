using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party {

    public string Leader;

    public List<string> Members = new List<string>();

    public Party(string leader, List<string> members)
    {
        this.Leader = leader;
        this.Members = members;
    }

    public bool HasPlayerBelowLVL(int LVL)
    {
        for(int i=0;i<Members.Count;i++)
        {
            if(Members[i] == LocalUserInfo.Me.ClientCharacter.Name)
            {
                if (LocalUserInfo.Me.ClientCharacter.LVL < LVL)
                {
                    return true;
                }

                continue;
            }

            if(LocalUserInfo.Me.GetKnownCharacter(Members[i]).Info.LVL < LVL)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasPlayerAboveLVL (int LVL)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            if (Members[i] == LocalUserInfo.Me.ClientCharacter.Name)
            {
                if (LocalUserInfo.Me.ClientCharacter.LVL > LVL)
                {
                    return true;
                }

                continue;
            }

            if (LocalUserInfo.Me.GetKnownCharacter(Members[i]).Info.LVL > LVL)
            {
                return true;
            }
        }

        return false;
    }
}
