using TMPro;
using UnityEngine;
using System;
using System.Collections;
using U0UGames.Localization.UI;
using U0UGames.Localization;
using SubSystems.AllScenes.UI;

public class MessageBubble : MonoBehaviour
{
    [SerializeField] private TypewriteAnimController typewriteAnimController;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.2f, 0f);
    [SerializeField] private float messageStayTime = 3f;

    [SerializeField] private Transform messageAnchor;

    [Header("Auto Advance Settings")]
    [SerializeField] private float secondsPerChar = 0.05f;
    [SerializeField] private float minWaitSeconds = 1.0f;
    [SerializeField] private float maxWaitSeconds = 6.0f;

    private Canvas parentCanvas;
    private ThirdPersonCamera uiCamera;
    private CharacterData character;
    private string message;
    private Coroutine autoHideCoroutine;
    private Action onMessageHide;

    public CharacterData Character => character;
    public Transform MessageAnchor => messageAnchor;
    public string Message => message;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        uiCamera = null;
    }

    public void SetData(CharacterData character, Transform messageBubbleAnchor, LocalizeString message, Action onAutoHide)
    {
        this.character = character;
        messageAnchor = messageBubbleAnchor;
        onMessageHide = onAutoHide;
        UpdateMessage(message);
    }

    public void SetCamera(ThirdPersonCamera camera)
    {
        uiCamera = camera;
        if (uiCamera == null)
        {
            return;
        }
        uiCamera.OnPosChanaged += SyncPosition;
    }

    private void OnDestroy()
    {
        if (uiCamera != null)
        {
            uiCamera.OnPosChanaged -= SyncPosition;
        }
    }

    public void UpdateMessage(LocalizeString message)
    {
        this.message = message.Value;

        typewriteAnimController.ShowText(message.Value, onFinish: () =>
        {
            if (autoHideCoroutine != null)
                StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = StartCoroutine(HideAfterDelay());
        });
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(messageStayTime);
        autoHideCoroutine = null;
        var callback = onMessageHide;
        onMessageHide = null;
        callback?.Invoke();
    }

    private void Update()
    {
        SyncPosition();
    }

    public void RefreshPosition()
    {
        SyncPosition();
    }

    private void SyncPosition()
    {
        if (messageAnchor == null)
        {
            return;
        }

        if (uiCamera == null || uiCamera.Camera == null)
        {
            return;
        }

        var worldPos = messageAnchor.position + worldOffset;

        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            transform.position = worldPos;
            return;
        }

        transform.position = uiCamera.Camera.WorldToScreenPoint(worldPos);
    }

    public void StopMessageHide()
    {
        typewriteAnimController.Complete();
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);
    }

    private void OnDisable()
    {
        onMessageHide = null;
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);
    }
}