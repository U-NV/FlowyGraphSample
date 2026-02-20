using UnityEngine;
using UnityEngine.UI;

public class InteractableIconUI : MonoBehaviour
{
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private Image iconImage;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private Button button;
    private InteractableObject target;
    private Camera worldCamera;
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

    public void Init(InteractableObject newTarget, Camera newCamera)
    {
        target = newTarget;
        worldCamera = newCamera;
        canInteract = false;
        isVisible = false;
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

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (worldCamera == null)
        {
            return;
        }

        var screenPos = worldCamera.WorldToScreenPoint(target.InteractiveIconAnchor.position + worldOffset);
        iconRect.position = screenPos;


        iconImage.gameObject.SetActive(isVisible);
        

        if (button != null)
        {
            button.interactable = canInteract;
        }
    }

    public void OnClick()
    {
        if (canInteract)
        {
            target?.Interact();
        }
    }
}
