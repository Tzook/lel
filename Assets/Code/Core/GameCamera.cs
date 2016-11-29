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

    public Camera Cam;

    RaycastHit2D CurrentMouseHit;

    #endregion

    Vector3 dampRef;

    protected Vector3 initPos;
    protected Vector3 targetPos;

    public static GameCamera Instance;
    public static Vector3 MousePosition;

	void Awake()
    {
        Instance = this;
        Cam = GetComponent<Camera>();
    }

    void Start()
    {
        initPos = transform.position;
        InGameMainMenuUI.Instance.GetComponent<Canvas>().worldCamera = this.Cam;
    }

    public void Register(GameObject objToFollow)
    {
        followingObject = objToFollow;
    }


    Coroutine DoubleClickInstance;

    void Update()
    {
        MousePosition = (Vector2)Cam.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetMouseButtonDown(0))
        {
            if (DoubleClickInstance == null)
            {
                DoubleClickInstance = StartCoroutine(DoubleClickRoutine());
            }

            CurrentMouseHit = Physics2D.Raycast(Cam.ScreenToWorldPoint(Input.mousePosition), transform.forward, Mathf.Infinity);

           
        }
    }

    private IEnumerator DoubleClickRoutine()
    {
        yield return 0;

        float t = 0f;
        while(t<1f)
        {
            t += 3f * Time.deltaTime;

            if(Input.GetMouseButtonDown(0))
            {
                DoubleClick();
                DoubleClickInstance = null;
                yield break;
            }

            yield return 0;
        }

        DoubleClickInstance = null;
    }

    private void DoubleClick()
    {
        if(CurrentMouseHit.collider != null)
        {
            if(CurrentMouseHit.collider.GetComponent<ActorInstance>() != null)
            {
                InGameMainMenuUI.Instance.ShowCharacterInfo(CurrentMouseHit.collider.GetComponent<ActorInstance>().Info);
            }
        }
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

    public void InstantFocusCamera()
    {
        if (this.CamType == CameraType.Horizontal)
        {
            transform.position = new Vector3(followingObject.transform.position.x, transform.position.y, initPos.z);
        }
        else if (this.CamType == CameraType.Vertical)
        {
            transform.position = new Vector3(transform.position.x, followingObject.transform.position.y, initPos.z);
        }
        else
        {
            transform.position = new Vector3(followingObject.transform.position.x, followingObject.transform.position.y, initPos.z);
        }
    }

    public enum CameraType
    {
        Normal, Vertical, Horizontal
    }
}
