using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowerEntity : MonoBehaviour {

    [SerializeField]
    float AddedY;

    [SerializeField]
    float FollowSpeed = 5f;

    Vector3 initPos;

    Transform followingObject;

    Vector3 targetPos;

    private void Start()
    {
        initPos = transform.position;
    }

    void FixedUpdate()
    {
        if(followingObject == null)
        {
            if(LocalUserInfo.Me.ClientCharacter.Instance != null)
            {
                followingObject = LocalUserInfo.Me.ClientCharacter.Instance.transform;
            }
        }
        else
        {
            targetPos = new Vector3(followingObject.transform.position.x, followingObject.transform.position.y + AddedY, initPos.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed);
        }
    }

}
