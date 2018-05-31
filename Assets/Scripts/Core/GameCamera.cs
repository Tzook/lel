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
    float MaxFarAim = 3f;

    float farAimX;
    float farAimY;

    [SerializeField]
    protected GameObject followingObject;

    [SerializeField]
    protected CameraType CamType = CameraType.Normal;

    [SerializeField]
    GameObject BlurCamera;

    [SerializeField]
    Blur m_BlurEffect;

    [SerializeField]
    LayerMask EssentialsInteractableMask = 1 << 14;

    [SerializeField]
    LayerMask InteractableMask = ~0;

    public Camera Cam;
    public Camera BlurCam;

    RaycastHit2D CurrentMouseHit;

    public bool CamFollowing = true;

    #endregion

    Vector3 dampRef;

    protected Vector3 initPos;
    protected Vector3 targetPos;

    protected Coroutine ShakeInstance;

    public static GameCamera Instance;
    public static Vector3 MousePosition;

    protected Coroutine FarAimModeInstance;
    public bool FarAimMode = false;

    float InitCamSize;

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
            return CurrentMouseHit.collider.tag == "NPC" || CurrentMouseHit.collider.tag == "NPCBubble" || CurrentMouseHit.collider.tag == "Actor";
        }
    }

    public GameObject CurrentHovered
    {
        get
        {
            if(CurrentMouseHit.collider != null)
            {
                return CurrentMouseHit.collider.gameObject;
            }

            return null;
        }
    }

	void Awake()
    {
        Instance = this;
        Cam = GetComponent<Camera>();
        BlurCam = BlurCamera.GetComponent<Camera>();
    }

    void Start()
    {
        initPos = transform.position;

        InGameMainMenuUI.Instance.SetCurrentCamera(this.Cam);

        InitCamSize = Cam.orthographicSize;
    }

    public void Register(GameObject objToFollow)
    {
        followingObject = objToFollow;
    }

    void Update()
    {

        MousePosition = (Vector2)Cam.ScreenToWorldPoint(Input.mousePosition);

        CurrentMouseHit = Physics2D.Raycast(MousePosition, transform.forward, Mathf.Infinity, EssentialsInteractableMask);

        if (!CurrentMouseHit)
        {
            CurrentMouseHit = Physics2D.Raycast(MousePosition, transform.forward, Mathf.Infinity, InteractableMask);
        }

        if (Input.GetMouseButton(0) && !Game.Instance.isInteractingWithUI)
        {
            Vector2 cursorHotspot = new Vector2(Content.Instance.CrosshairCursor.width / 2, Content.Instance.CrosshairCursor.height / 2);
            Cursor.SetCursor(Content.Instance.CrosshairCursor, cursorHotspot, CursorMode.Auto);
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

        //TODO Temporary change?
        if (Input.GetMouseButtonUp(1) && HoveringOnInteractable)
        {
            DoubleClick();
        }

        UpdateFarAimMode();
    }

    protected void UpdateFarAimMode()
    {
        if (FarAimModeInstance == null)
        {
            if (Input.GetMouseButton(1))
            {
                FarAimModeInstance = StartCoroutine(FarAimModeRoutine());
            }
        }
        else
        {
            if (!Input.GetMouseButton(1))
            {
                StopCoroutine(FarAimModeInstance);
                FarAimModeInstance = null;
                FarAimMode = false;
            }
        }
    }

    private IEnumerator FarAimModeRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        FarAimMode = true;
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
                yield break;
            }

            yield return 0;
        }
    }

    private void DoubleClick()
    {
        if(Game.Instance.CanUseUI && CurrentMouseHit.collider != null)
        {
            if (CurrentMouseHit.collider.GetComponent<NPC>() != null)
            {
                CurrentMouseHit.collider.GetComponent<NPC>().Interact();
            }
            else if (CurrentMouseHit.collider.GetComponent<ActorInstance>() != null)
            {
                InGameMainMenuUI.Instance.ShowCharacterInfo(CurrentMouseHit.collider.GetComponent<ActorInstance>().Info);
            }
            else if (CurrentMouseHit.collider.GetComponent<QuestBubbleCollider>() != null)
            {
                CurrentMouseHit.collider.GetComponent<QuestBubbleCollider>().npc.Interact();
            }
        }
    }

    void FixedUpdate()
    {
        if (CamFollowing)
        {
            if (followingObject != null)
            {

                if(FarAimMode)
                {
                    farAimX = Mathf.Clamp((MousePosition - followingObject.transform.position).x/2f, -MaxFarAim, MaxFarAim);
                    farAimY = Mathf.Clamp((MousePosition - followingObject.transform.position).y/2f, -MaxFarAim, MaxFarAim);
                }
                else
                {
                    farAimX = 0f;
                    farAimY = 0f;
                }
                
                if (this.CamType == CameraType.Horizontal)
                {
                    targetPos = new Vector3(followingObject.transform.position.x + farAimX, transform.position.y + AddedY, initPos.z);
                }
                else if (this.CamType == CameraType.Vertical)
                {
                    targetPos = new Vector3(transform.position.x, followingObject.transform.position.y + AddedY + farAimY, initPos.z);
                }
                else if (this.CamType == CameraType.Static)
                {
                    targetPos = initPos;
                }
                else
                {
                    targetPos = new Vector3(followingObject.transform.position.x + farAimX, followingObject.transform.position.y + AddedY + farAimY, initPos.z);
                }

                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref dampRef, FollowSpeed);

                
            }
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

    public void FocusOnTransform(Transform targetTransform)
    {
        StopAllCoroutines();

        CamFollowing = false;

        StartCoroutine(FocusOnTransformRoutine(targetTransform));
    }

    IEnumerator FocusOnTransformRoutine(Transform targetTransform)
    {
        while (true)
        {
            targetPos = new Vector3(targetTransform.position.x, targetTransform.position.y, initPos.z);

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref dampRef, FollowSpeed * 2f);

            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 1.5f, Time.deltaTime * 2f);
            BlurCam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 1.5f, Time.deltaTime * 2f);

            yield return 0;
        }
    }

    public void FocusDefault()
    {
        StopAllCoroutines();

        CamFollowing = true;

        StartCoroutine(FocusDefaultRoutine());
    }

    public void Shake(float Duration)
    {
        if (ShakeInstance != null)
        {
            StopCoroutine(ShakeInstance);
        }

        ShakeInstance = StartCoroutine(ShakeRoutine(Duration, 10f));
    }

    public void Shake(float Duration, float Power)
    {
        if(ShakeInstance != null)
        {
            StopCoroutine(ShakeInstance);
        }

        ShakeInstance = StartCoroutine(ShakeRoutine(Duration, Power));
    }

    IEnumerator FocusDefaultRoutine()
    {
        float t = 0f;
        while(t<1f)
        {
            t += 1f * Time.deltaTime;

            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, InitCamSize, t);
            BlurCam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, InitCamSize, t);

            yield return 0;
        }

        Cam.orthographicSize = InitCamSize;
        BlurCam.orthographicSize = InitCamSize;
    }

    IEnumerator ShakeRoutine(float startDuration, float ShakePower, bool smooth = true )
    {
        transform.localRotation = Quaternion.identity;

        float shakePercentage;
        float shakeDuration = startDuration;

        while (shakeDuration > 0.01f)
        {
            Vector3 rotationAmount = Random.insideUnitSphere * ShakePower;//A Vector3 to add to the Local Rotation
            rotationAmount.z = 0;//Don't change the Z; it looks funny.

            shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

            ShakePower = ShakePower * shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.


            if (smooth)
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * ShakePower);
            else
                transform.localRotation = Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.

            yield return 0;
        }

        float t = 0f;
        while(t > 1f)
        {
            t += 1f * Time.deltaTime;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, t);

            yield return 0;
        }
        transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        ShakeInstance = null;
    }

}
