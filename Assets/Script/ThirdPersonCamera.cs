using UnityEngine;

public interface ICameraFocusController
{
    void EnableFocus(Transform a, Transform b, float zoomMultiplier);
    void DisableFocus();
}

[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour, ICameraFocusController
{

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -6f);
    [SerializeField] private Vector3 focusOffset = new Vector3(0f, 5f, -6f);
    [SerializeField] private float focusZoomMultiplier = 0.7f;
    [Header("Follow Position Settings")]
    [Tooltip("0 = 立即吸附（无延迟），>0 = Lerp 追赶速度")]
    [SerializeField] private float positionFollowSpeed = 0f;

    private Camera _camera;
    public Camera Camera => _camera;
    private bool focusMode;
    private Transform focusTargetA;
    private Transform focusTargetB;
    private float currentFocusZoom;

    public event System.Action OnPosChanaged;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("ThirdPersonCamera requires a Camera component");
        }
    }

    private void LateUpdate()
    {
        if (focusMode)
        {
            UpdateFocus();
            return;
        }

        if (target == null)
        {
            return;
        }

        var desiredPosition = target.position + offset;
        transform.position = positionFollowSpeed > 0f
            ? Vector3.Lerp(transform.position, desiredPosition, positionFollowSpeed * Time.deltaTime)
            : desiredPosition;

        OnPosChanaged?.Invoke();
    }

    public void SetFollowTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void EnableFocus(Transform a, Transform b, float zoomMultiplier)
    {
        focusTargetA = a;
        focusTargetB = b;
        currentFocusZoom = zoomMultiplier > 0f ? zoomMultiplier : focusZoomMultiplier;
        focusMode = focusTargetA != null && focusTargetB != null;
    }

    public void DisableFocus()
    {
        focusMode = false;
        focusTargetA = null;
        focusTargetB = null;
    }

    private void UpdateFocus()
    {
        if (focusTargetA == null || focusTargetB == null)
        {
            focusMode = false;
            return;
        }

        var midpoint = (focusTargetA.position + focusTargetB.position) * 0.5f;
        var desiredPosition = midpoint + focusOffset * currentFocusZoom;
        transform.position = positionFollowSpeed > 0f
            ? Vector3.Lerp(transform.position, desiredPosition, positionFollowSpeed * Time.deltaTime)
            : desiredPosition;

        OnPosChanaged?.Invoke();
    }
}
