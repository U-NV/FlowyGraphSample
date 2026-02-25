using FlowyGraph;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private Transform interactiveIconAnchor;
    public Transform InteractiveIconAnchor => interactiveIconAnchor;
    
    [Header("Graphs")]
    [SerializeField] private FlowyGraphAsset dialogueGraph;
    [SerializeField] private FlowyGraphAsset idleGraph;
    [SerializeField] private OptionsSelector<SignalOption> signalToEmit;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private bool allowRepeat = true;
    [SerializeField] private bool allowIdleRepeat = true;

    private FlowyGraphRuntime runtime;
    private InteractableIconManager iconManager;
    private GraphType currentGraphType = GraphType.None;
    private bool hasPlayedInteract;
    private bool hasPlayedApproach;
    private bool hasPlayedLeave;
    private bool hasPlayedIdle;
    private bool isInRange;
    private bool isSelected;
    private bool hasStateInitialized;

    private enum GraphType
    {
        None,
        Interact,
        Approach,
        Leave,
        Idle
    }

    private void OnEnable()
    {
        iconManager = FindObjectOfType<InteractableIconManager>();
        iconManager?.Register(this);
    }

    public bool IsEnabled => isEnabled;

    public void SetEnabled(bool enabled)
    {
        SetEnabled(enabled, true);
    }

    public void SetEnabled(bool enabled, bool stopCurrentGraph)
    {
        if (isEnabled == enabled)
        {
            return;
        }

        isEnabled = enabled;

        if (!isEnabled)
        {
            isInRange = false;
            isSelected = false;
            if (stopCurrentGraph)
            {
                StopCurrentGraph();
            }
            return;
        }

        TryPlayIdleIfNeeded();
    }

    public void Interact()
    {
        if (!isEnabled)
        {
            return;
        }

        if (!allowRepeat && hasPlayedInteract)
        {
            return;
        }

        var started = TryPlayGraph(GraphType.Interact, dialogueGraph, allowRepeat);
        if (started && signalToEmit != null)
        {
            FlowyGraphSystem.EmitSignal(signalToEmit.Options, signalToEmit.Index);
        }
    }

    private void OnGraphOver(FlowyGraphRuntime graph)
    {
        if (graph != null)
        {
            graph.OnGraphOver -= OnGraphOver;
        }

        if (runtime == graph)
        {
            runtime = null;
            currentGraphType = GraphType.None;
        }

        TryPlayIdleIfNeeded();
    }

    private void OnDisable()
    {
        iconManager?.Unregister(this);
        StopCurrentGraph();
    }

    public void UpdateInteractionState(bool inPlayerRange, bool selected)
    {
        if (!isEnabled)
        {
            if (isInRange || isSelected)
            {
                isInRange = false;
                isSelected = false;
            }
            return;
        }

        isInRange = inPlayerRange;
        isSelected = selected;

        if (!hasStateInitialized)
        {
            hasStateInitialized = true;
            TryPlayIdleIfNeeded();
        }
    }

    private bool TryPlayGraph(GraphType type, FlowyGraphAsset graph, bool allowRepeatForType)
    {
        if (graph == null)
        {
            return false;
        }

        if (!allowRepeatForType && HasPlayed(type))
        {
            return false;
        }

        if (runtime != null && currentGraphType == type)
        {
            return false;
        }

        if (type == GraphType.Idle && currentGraphType != GraphType.None && currentGraphType != GraphType.Idle)
        {
            return false;
        }

        StopCurrentGraph();

        runtime = new FlowyGraphRuntime(graph);
        currentGraphType = type;
        runtime.OnGraphOver += OnGraphOver;
        runtime.Start();
        MarkPlayed(type);
        return true;
    }

    private void StopCurrentGraph()
    {
        if (runtime != null)
        {
            runtime.OnGraphOver -= OnGraphOver;
            runtime.Clear();
            runtime = null;
        }
        currentGraphType = GraphType.None;
    }

    private void TryPlayIdleIfNeeded()
    {
        if (!hasStateInitialized)
        {
            return;
        }

        if (currentGraphType == GraphType.Interact)
        {
            return;
        }

        TryPlayGraph(GraphType.Idle, idleGraph, allowIdleRepeat);
    }

    private bool HasPlayed(GraphType type)
    {
        return type switch
        {
            GraphType.Interact => hasPlayedInteract,
            GraphType.Approach => hasPlayedApproach,
            GraphType.Leave => hasPlayedLeave,
            GraphType.Idle => hasPlayedIdle,
            _ => false
        };
    }

    private void MarkPlayed(GraphType type)
    {
        switch (type)
        {
            case GraphType.Interact:
                hasPlayedInteract = true;
                break;
            case GraphType.Approach:
                hasPlayedApproach = true;
                break;
            case GraphType.Leave:
                hasPlayedLeave = true;
                break;
            case GraphType.Idle:
                hasPlayedIdle = true;
                break;
        }
    }
}
