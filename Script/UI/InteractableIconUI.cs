using UnityEngine;
using UnityEngine.UI;

public class InteractableIconUI : MonoBehaviour
{
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite hoverIcon;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private Button button;
    private InteractableObject target;
    private Camera worldCamera;
    private bool canInteract;

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

        var screenPos = worldCamera.WorldToScreenPoint(target.transform.position + worldOffset);
        iconRect.position = screenPos;

        if (iconImage != null && hoverIcon != null && defaultIcon != null)
        {
            iconImage.sprite = canInteract ? hoverIcon : defaultIcon;
        }

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
