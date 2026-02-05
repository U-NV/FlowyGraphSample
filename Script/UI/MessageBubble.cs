using TMPro;
using UnityEngine;
using System;
using System.Collections;

public class MessageBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.2f, 0f);

    [SerializeField] private Transform messageAnchor;

    [Header("Auto Advance Settings")]
    [SerializeField] private float secondsPerChar = 0.05f;
    [SerializeField] private float minWaitSeconds = 1.0f;
    [SerializeField] private float maxWaitSeconds = 6.0f;

    private Canvas parentCanvas;
    private Camera uiCamera;
    private CharacterData character;
    private string message;
    private Coroutine autoHideCoroutine;
    private Action onAutoHideCallback;

    public CharacterData Character => character;
    public Transform MessageAnchor => messageAnchor;
    public string Message => message;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        uiCamera = Camera.main;
    }

    public void SetData(CharacterData character, Transform messageBubbleAnchor, string message, Action onAutoHide)
    {
        this.character = character;
        messageAnchor = messageBubbleAnchor;
        onAutoHideCallback = onAutoHide;
        UpdateMessage(message);
    }

    public void UpdateMessage(string message)
    {
        this.message = message;
        messageText.text = message;
        StartAutoHide();
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

        var worldPos = messageAnchor.position + worldOffset;

        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            transform.position = worldPos;
            return;
        }

        if (uiCamera == null)
        {
            uiCamera = Camera.main;
            if (uiCamera == null)
            {
                return;
            }
        }

        transform.position = uiCamera.WorldToScreenPoint(worldPos);
    }

    public void StopAutoHide()
    {
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
    }

    private void OnDisable()
    {
        StopAutoHide();
        onAutoHideCallback = null;
    }

    private void StartAutoHide()
    {
        StopAutoHide();
        var waitSeconds = GetAutoAdvanceWaitSeconds(message);
        if (waitSeconds <= 0f)
        {
            waitSeconds = 0.01f;
        }
        autoHideCoroutine = StartCoroutine(AutoHideRoutine(waitSeconds));
    }

    private float GetAutoAdvanceWaitSeconds(string message)
    {
        var length = string.IsNullOrEmpty(message) ? 0 : message.Length;
        return Mathf.Clamp(length * secondsPerChar, minWaitSeconds, maxWaitSeconds);
    }

    private IEnumerator AutoHideRoutine(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        autoHideCoroutine = null;
        var callback = onAutoHideCallback;
        onAutoHideCallback = null;
        callback?.Invoke();
    }
}