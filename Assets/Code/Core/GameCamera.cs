using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GameCamera : MonoBehaviour {

    #region Configurable

    [SerializeField]
    protected float FollowSpeed;

    [SerializeField]
    float AddedY = 1f;

    [SerializeField]
    protected GameObject followingObject;

    [SerializeField]
    protected CameraType CamType = CameraType.Normal;

    [SerializeField]
    GameObject BlurCamera;

    [SerializeField]
    Blur m_BlurEffect;

    public Camera Cam;

    RaycastHit2D CurrentMouseHit;

    #endregion

    Vector3 dampRef;

    protected Vector3 initPos;
    protected Vector3 targetPos;

    public static GameCamera Instance;
    public static Vector3 MousePosition;

    public bool HoveringOnInteractable
    {
        get
        {
            return CurrentMouseHit.collider != null && isMouseHitInteractable;
        }
    }

    bool isMouseHitInteractable
    {
        get
        {
            return CurrentMouseHit.collider.tag == "NPC" || CurrentMouseHit.collider.tag == "Actor";
        }
    }

	void Awake()
    {
        Instance = this;
        Cam = GetComponent<Camera>();
    }

    void Start()
    {
        initPos = transform.position;

        InGameMainMenuUI.Instance.SetCurrentCamera(this.Cam);
    }

    public void Register(GameObject objToFollow)
    {
        followingObject = objToFollow;
    }


    Coroutine DoubleClickInstance;

    void Update()
    {
        MousePosition = (Vector2)Cam.ScreenToWorldPoint(Input.mousePosition);

        CurrentMouseHit = Physics2D.Raycast(Cam.ScreenToWorldPoint(Input.mousePosition), transform.forward, Mathf.Infinity);


        if (Input.GetMouseButton(0) && !InGameMainMenuUI.Instance.isWindowOpen)
        {
            Cursor.SetCursor(Content.Instance.CrosshairCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            if (CurrentMouseHit.collider != null)
            {
                if (isMouseHitInteractable)
                {
                    Cursor.SetCursor(Content.Instance.ClickableCursor, Vector2.zero, CursorMode.Auto);
                }
                else
                {
                    Cursor.SetCursor(Content.Instance.DefaultCursor, Vector2.zero, CursorMode.Auto);
                }
            }
            else
            {
                Cursor.SetCursor(Content.Instance.DefaultCursor, Vector2.zero, CursorMode.Auto);
            }
        }

        if (Input.GetMouseButtonDown(0) && HoveringOnInteractable && !InGameMainMenuUI.Instance.isWindowOpen)
        {
            if (DoubleClickInstance == null)
            {
                DoubleClickInstance = StartCoroutine(DoubleClickRoutine());
            }
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
        if(Game.Instance.CanUseUI && CurrentMouseHit.collider != null)
        {
            if(CurrentMouseHit.collider.GetComponent<ActorInstance>() != null)
            {
                InGameMainMenuUI.Instance.ShowCharacterInfo(CurrentMouseHit.collider.GetComponent<ActorInstance>().Info);
            }
            else if (CurrentMouseHit.collider.GetComponent<NPC>() != null)
            {
                CurrentMouseHit.collider.GetComponent<NPC>().Interact();
            }

        }
    }

    void FixedUpdate()
    {
        if (followingObject != null)
        {
            if (this.CamType == CameraType.Horizontal)
            {
                targetPos = new Vector3(followingObject.transform.position.x, transform.position.y + AddedY, initPos.z);
            }
            else if (this.CamType == CameraType.Vertical)
            {
                targetPos = new Vector3(transform.position.x, followingObject.transform.position.y + AddedY, initPos.z);
            }
            else if (this.CamType == CameraType.Static)
            {
                targetPos = initPos;
            }
            else
            {
                targetPos = new Vector3(followingObject.transform.position.x, followingObject.transform.position.y + AddedY, initPos.z);
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPos ,ref dampRef, FollowSpeed);
        }
    }

    public void SetBlurMode(bool state)
    {
        m_BlurEffect.enabled = state;
        BlurCamera.gameObject.SetActive(state);
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
        else if (this.CamType == CameraType.Static)
        {
        }
        else
        {
            transform.position = new Vector3(followingObject.transform.position.x, followingObject.transform.position.y, initPos.z);
        }
    }

    public enum CameraType
    {
        Normal, Vertical, Horizontal, Static
    }
}
