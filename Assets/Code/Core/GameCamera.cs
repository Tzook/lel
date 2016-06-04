using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    [SerializeField]
    protected float FollowSpeed;

    [SerializeField]
    protected GameObject followingObject;

    protected Vector3 initPos;
    protected Vector3 targetPos;

	void Awake()
    {
        SM.GameCamera = this;
    }

    void Start()
    {
        initPos = transform.position;
    }

    public void Register(GameObject objToFollow)
    {
        followingObject = objToFollow;
    }

    void LateUpdate()
    {
        if (followingObject != null)
        {
            targetPos = new Vector3(followingObject.transform.position.x, followingObject.transform.position.y, initPos.z);
            transform.position = Vector3.Lerp(transform.position, targetPos , Time.deltaTime * FollowSpeed);
        }
    }
}
