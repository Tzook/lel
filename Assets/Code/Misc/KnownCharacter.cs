using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnownCharacter {

    public ActorInfo Info = null;
    public string Name = "Unknown";
    public bool isLoggedIn = false;

    public KnownCharacter(ActorInfo info)
    {
        this.Info = info;
        this.Name = this.Info.Name;
    }


}
