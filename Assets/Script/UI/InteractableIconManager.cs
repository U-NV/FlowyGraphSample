using System.Collections.Generic;
using UnityEngine;

public class InteractableIconManager : MonoBehaviour
{
    [SerializeField] private InteractableIconUI iconPrefab;
    [SerializeField] private RectTransform iconRoot;
    [SerializeField] private ThirdPersonCamera worldCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float interactionRadius = 1.0f;
    [SerializeField] private float showIconRadius = 2.5f;

    private readonly Dictionary<InteractableObject, InteractableIconUI> icons = new();
    private bool isFocused;

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = GameObject.FindObjectOfType<ThirdPersonCamera>();
        }
        if (playerTransform == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    private void OnEnable()
    {
        CameraFocusManager.FocusStateChanged += OnFocusStateChanged;
    }

    private void OnDisable()
    {
        CameraFocusManager.FocusStateChanged -= OnFocusStateChanged;
    }

    private void LateUpdate()
    {
        UpdateInteractableStates();
    }

    private void Start()
    {
        UpdateInteractableStates();
    }

    private void UpdateInteractableStates()
    {
        if (playerTransform == null)
        {
            return;
        }

        InteractableObject closestTarget = null;
        InteractableIconUI closestIcon = null;
        var closestDistance = float.PositiveInfinity;

        // 第一轮：计算距离，找到最近的可交互对象
        foreach (var pair in icons)
        {
            var target = pair.Key;
            var icon = pair.Value;
            if (target == null || icon == null)
            {
                continue;
            }

            if (isFocused || !target.IsEnabled)
            {
                continue;
            }

            var distance = Vector3.Distance(playerTransform.position, target.transform.position);
            var isInRange = distance <= interactionRadius;

            if (isInRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
                closestIcon = icon;
            }
        }

        // 第二轮：一次性设置所有对象的最终状态
        foreach (var pair in icons)
        {
            var target = pair.Key;
            var icon = pair.Value;
            if (target == null || icon == null)
            {
                continue;
            }

            if (isFocused || !target.IsEnabled)
            {
                icon.SetState(false, false);
                target.UpdateInteractionState(false, false);
                continue;
            }

            var distance = Vector3.Distance(playerTransform.position, target.transform.position);
            var isVisible = distance <= showIconRadius;
            var isInRange = distance <= interactionRadius;
            var isClosest = target == closestTarget;

            icon.SetState(isVisible, isClosest);
            target.UpdateInteractionState(isInRange, isClosest);
        }
    }

    private void OnFocusStateChanged(bool isFocused)
    {
        this.isFocused = isFocused;
        foreach (var pair in icons)
        {
            if (pair.Value == null)
            {
                continue;
            }
            var target = pair.Key;
            var shouldShow = !isFocused && target != null && target.IsEnabled;
            if (!shouldShow)
            {
                pair.Value.SetState(false, false);
            }
        }
    }

    public void Register(InteractableObject target)
    {
        if (target == null || icons.ContainsKey(target))
        {
            return;
        }

        if (iconPrefab == null || iconRoot == null)
        {
            Debug.LogWarning("[InteractableIconManager] Icon prefab or root not set.");
            return;
        }

        var icon = Instantiate(iconPrefab, iconRoot);
        icon.Init(target, worldCamera);
        icons[target] = icon;
    }

    public void Unregister(InteractableObject target)
    {
        if (target == null)
        {
            return;
        }

        if (icons.TryGetValue(target, out var icon))
        {
            if (icon != null)
            {
                Destroy(icon.gameObject);
            }
            icons.Remove(target);
        }
    }
}
