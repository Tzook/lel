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
}
