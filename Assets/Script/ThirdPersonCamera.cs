using UnityEngine;

public interface ICameraFocusController
{
    void EnableFocus(Transform a, Transform b, float zoomMultiplier);
    void DisableFocus();
}

public class ThirdPersonCamera : MonoBehaviour, ICameraFocusController
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -6f);
    [SerializeField] private Vector3 focusOffset = new Vector3(0f, 5f, -6f);
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private float focusZoomMultiplier = 0.7f;
    [Header("Focus Look Settings")]
    [SerializeField] private float focusPitchOffset = 0f;
    [SerializeField] private float focusLookLerp = 8f;

    private bool focusMode;
    private Transform focusTargetA;
    private Transform focusTargetB;
    private float currentFocusZoom;   

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
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        if (lookAtTarget)
        {
            ApplyPitchOnlyLookAt(target.position, followSpeed, 0f);
        }
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
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        if (lookAtTarget)
        {
            ApplyPitchOnlyLookAt(midpoint, focusLookLerp, focusPitchOffset);
        }
    }

    private void ApplyPitchOnlyLookAt(Vector3 targetPosition, float lerpSpeed, float pitchOffset)
    {
        var lookRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        var currentEuler = transform.rotation.eulerAngles;
        var targetEuler = lookRotation.eulerAngles;

        var newPitch = Mathf.LerpAngle(currentEuler.x, targetEuler.x + pitchOffset, lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(newPitch, currentEuler.y, currentEuler.z);
    }
}
