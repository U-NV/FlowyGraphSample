using System.Collections.Generic;
using UnityEngine;

public class InteractableIconManager : MonoBehaviour
{
    [SerializeField] private InteractableIconUI iconPrefab;
    [SerializeField] private RectTransform iconRoot;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float interactionRadius = 1.0f;
    [SerializeField] private float showIconRadius = 2.5f;

    private readonly Dictionary<InteractableObject, InteractableIconUI> icons = new();
    private bool isFocused;

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
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

        var closestIcon = default(InteractableIconUI);
        var closestDistance = float.PositiveInfinity;

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
            
            if (isInRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestIcon = icon;
            }
            
            // 初始设置，后面会根据是否是最近的来更新交互状态
            icon.SetState(isVisible, false);
            target.UpdateInteractionState(isInRange, false);
        }

        if (closestIcon != null)
        {
            // 找到最近的图标，设置其为可交互
            foreach (var pair in icons)
            {
                if (pair.Value == closestIcon)
                {
                    var target = pair.Key;
                    var distance = Vector3.Distance(playerTransform.position, target.transform.position);
                    var isVisible = distance <= showIconRadius;
                    closestIcon.SetState(isVisible, true);
                    target.UpdateInteractionState(true, true);
                    break;
                }
            }
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
