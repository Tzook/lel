using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatingIdler : MonoBehaviour {

    [SerializeField]
    int AnimationIndex;

    private void Start()
    {
        GetComponent<Animator>().SetInteger("AnimIndex", AnimationIndex);
    }
}
