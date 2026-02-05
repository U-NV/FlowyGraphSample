using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private LayerMask interactableMask = 0;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private GameObject destinationIndicator;
    [SerializeField] private bool movementEnabled = true;
    [SerializeField] private bool blockWhenOverUI = true;
    [SerializeField] private GraphicRaycaster uiRaycaster;
    [SerializeField] private EventSystem eventSystem;

    private Vector3 targetPoint;
    private bool hasTarget;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        if (uiRaycaster == null)
        {
            uiRaycaster = FindObjectOfType<GraphicRaycaster>();
        }
    }

    private void Update()
    {
        if (!movementEnabled)
        {
            return;
        }
        if (mainCamera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (blockWhenOverUI && IsPointerOverBlockingUI())
            {
                return;
            }

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (interactableMask.value != 0 && Physics.Raycast(ray, out var interactHit, 200f, interactableMask))
            {
                if (interactHit.collider != null &&
                    interactHit.collider.GetComponentInParent<InteractableObject>() != null)
                {
                    return;
                }
            }

            if (Physics.Raycast(ray, out var hit, 200f, groundMask))
            {
                SetMoveTarget(hit.point);
            }
        }

            TickMove();
    }

    private void SetMoveTarget(Vector3 point)
    {
        ShowDestinationIndicator(point);
        targetPoint = point;
        hasTarget = true;
    }

    private void TickMove()
    {
        if (!hasTarget)
        {
            return;
        }

        var current = transform.position;
        var next = Vector3.MoveTowards(current, targetPoint, moveSpeed * Time.deltaTime);
        transform.position = next;

        if (Vector3.Distance(next, targetPoint) <= stopDistance)
        {
            hasTarget = false;
            HideDestinationIndicator();
        }
    }

    private void ShowDestinationIndicator(Vector3 point)
    {
        if (destinationIndicator == null)
        {
            return;
        }
        destinationIndicator.transform.position = point;
        destinationIndicator.SetActive(true);
    }

    private void HideDestinationIndicator()
    {
        if (destinationIndicator != null)
        {
            destinationIndicator.SetActive(false);
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!movementEnabled)
        {
            hasTarget = false;
            HideDestinationIndicator();
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    private bool IsPointerOverBlockingUI()
    {
        if (eventSystem != null && eventSystem.IsPointerOverGameObject())
        {
            return true;
        }

        if (eventSystem == null || uiRaycaster == null)
        {
            return false;
        }

        var pointerEvent = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        uiRaycaster.Raycast(pointerEvent, results);
        return results.Count > 0;
    }
}
