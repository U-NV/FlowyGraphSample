using UnityEngine;
using UnityEngine.UI;

public class InteractableIconUI : MonoBehaviour
{
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private Image iconImage;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private Button button;
    private InteractableObject target;
    private ThirdPersonCamera worldCamera;
    private ThirdPersonCamera _listeningCamera;
    private bool canInteract;
    private bool isVisible;

    private void Awake()
    {
        if (iconRect == null)
        {
            iconRect = GetComponent<RectTransform>();
        }
        button.onClick.AddListener(OnClick);
    }

    private bool _isLisitingCameraPos = false;
    private void OnEnable()
    {
        SubscribeCameraPos(worldCamera);
    }

    private void OnDisable()
    {
        UnsubscribeCameraPos();
    }

    private void OnCameraPosChanaged()
    {
        if (worldCamera == null)
        {
            return;
        }
        UpdateIconPosition();
    }

    public void Init(InteractableObject newTarget, ThirdPersonCamera newCamera)
    {
        target = newTarget;
        worldCamera = newCamera;
        canInteract = false;
        isVisible = false;

        SubscribeCameraPos(worldCamera);
        UpdateIconPosition();
    }

    public void SetState(bool isVisible, bool isInteractable)
    {
        this.isVisible = isVisible;
        this.canInteract = isInteractable;
    }

    public void SetInteractableState(bool isInteractable)
    {
        canInteract = isInteractable;
    }

    private void Update()
    {
        if (worldCamera == null)
        {
            worldCamera = GameObject.FindObjectOfType<ThirdPersonCamera>();
        }

        if (worldCamera != null && _listeningCamera != worldCamera)
        {
            SubscribeCameraPos(worldCamera);
        }

        iconImage.gameObject.SetActive(isVisible);

        if (button != null)
        {
            button.interactable = canInteract;
        }
    }

    private void UpdateIconPosition()
    {
        if (target == null || worldCamera == null)
        {
            return;
        }

        var screenPos = worldCamera.Camera.WorldToScreenPoint(target.InteractiveIconAnchor.position + worldOffset);
        iconRect.position = screenPos;
    }

    private void SubscribeCameraPos(ThirdPersonCamera cameraToListen)
    {
        if (cameraToListen == null)
        {
            return;
        }

        if (_isLisitingCameraPos && _listeningCamera == cameraToListen)
        {
            return;
        }

        UnsubscribeCameraPos();
        cameraToListen.OnPosChanaged += OnCameraPosChanaged;
        _listeningCamera = cameraToListen;
        _isLisitingCameraPos = true;
    }

    private void UnsubscribeCameraPos()
    {
        if (_listeningCamera != null)
        {
            _listeningCamera.OnPosChanaged -= OnCameraPosChanaged;
        }
        _listeningCamera = null;
        _isLisitingCameraPos = false;
    }

    public void OnClick()
    {
        if (canInteract)
        {
            target?.Interact();
        }
    }
}
