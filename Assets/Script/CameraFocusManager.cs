using System;
using FlowyGraph;
using UnityEngine;

public class CameraFocusManager : MonoBehaviour
{
    public static event Action<bool> FocusStateChanged;

    [SerializeField] private ThirdPersonCamera cameraController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float focusZoomMultiplier = 0.7f;

    private CharacterVisualManager characterVisualManager;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    public void FocusOnCharacter(CharacterData character)
    {
        FocusOnCharacter(character, focusZoomMultiplier);
    }

    public void FocusOnCharacter(CharacterData character, float zoomOverride)
    {
        ResolveReferences();
        if (cameraController == null || playerTransform == null || character == null)
        {
            return;
        }

        if (characterVisualManager == null)
        {
            characterVisualManager = FlowyGraphBlackboard.GetClass<CharacterVisualManager>()
                ?? FindObjectOfType<CharacterVisualManager>();
        }

        var visual = characterVisualManager != null ? characterVisualManager.GetVisual(character) : null;
        if (visual == null)
        {
            return;
        }

        var zoom = zoomOverride > 0f ? zoomOverride : focusZoomMultiplier;
        cameraController.EnableFocus(playerTransform, visual.transform, zoom);
        playerController?.SetMovementEnabled(false);
        FocusStateChanged?.Invoke(true);
    }

    public void CancelFocus()
    {
        cameraController?.DisableFocus();
        playerController?.SetMovementEnabled(true);
        FocusStateChanged?.Invoke(false);
    }

    private void ResolveReferences()
    {
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<ThirdPersonCamera>();
        }

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerTransform == null)
        {
            if (playerController != null)
            {
                playerTransform = playerController.transform;
            }
            else
            {
                var playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
            }
        }
    }
}
