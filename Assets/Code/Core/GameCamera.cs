using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    #region Configurable

    [SerializeField]
    protected float FollowSpeed;

    [SerializeField]
    protected GameObject followingObject;

    [SerializeField]
    protected CameraType CamType = CameraType.Normal;

    Camera cam;

    #endregion

    Vector3 dampRef;

    protected Vector3 initPos;
    protected Vector3 targetPos;

    public static GameCamera Instance;
    public static Vector3 MousePosition;

	void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        initPos = transform.position;
    }

    public void Register(GameObject objToFollow)
    {
        followingObject = objToFollow;
    }

    void Update()
    {
        MousePosition = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        if (followingObject != null)
        {
            if (this.CamType == CameraType.Horizontal)
            {
                targetPos = new Vector3(followingObject.transform.position.x, transform.position.y, initPos.z);
            }
            else if (this.CamType == CameraType.Vertical)
            {
                targetPos = new Vector3(transform.position.x, followingObject.transform.position.y, initPos.z);
            }
            else
            {
                targetPos = new Vector3(followingObject.transform.position.x, followingObject.transform.position.y, initPos.z);
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPos ,ref dampRef, FollowSpeed);
        }
    }

    public enum CameraType
    {
        Normal, Vertical, Horizontal
    }
}
