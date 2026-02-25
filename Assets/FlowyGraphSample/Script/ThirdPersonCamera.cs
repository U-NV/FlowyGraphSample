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
    [SerializeField] private Vector3 focusCamOffset = new Vector3(0f, 5f, -6f);
    [SerializeField] private Vector3 focusCenterOffset = new Vector3(0f, 5f, -6f);
    [SerializeField] private float focusZoomMultiplier = 0.7f;
    [Header("Follow Position Settings")]
    [Tooltip("0 = 立即吸附（无延迟），>0 = Lerp 追赶速度")]
    [SerializeField] private float positionFollowSpeed = 0f;
    [Tooltip("0 = 立即吸附（无延迟），>0 = Slerp 旋转追赶速度")]
    [SerializeField] private float rotationFollowSpeed = 0f;

    private Camera _camera;
    public Camera Camera => _camera;
    private bool focusMode;
    private Transform focusTargetA;
    private Transform focusTargetB;
    private float currentFocusZoom;
    private Quaternion defaultRotation;

    public event System.Action OnPosChanaged;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("ThirdPersonCamera requires a Camera component");
        }
        defaultRotation = transform.rotation;
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

        transform.rotation = rotationFollowSpeed > 0f
            ? Quaternion.Slerp(transform.rotation, defaultRotation, rotationFollowSpeed * Time.deltaTime)
            : defaultRotation;

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
        midpoint = midpoint + focusCenterOffset;
        var desiredPosition = midpoint + focusCamOffset * currentFocusZoom;
        transform.position = positionFollowSpeed > 0f
            ? Vector3.Lerp(transform.position, desiredPosition, positionFollowSpeed * Time.deltaTime)
            : desiredPosition;

        var desiredRotation = Quaternion.LookRotation(midpoint - transform.position);
        transform.rotation = rotationFollowSpeed > 0f
            ? Quaternion.Slerp(transform.rotation, desiredRotation, rotationFollowSpeed * Time.deltaTime)
            : desiredRotation;

        OnPosChanaged?.Invoke();
    }
}
