using UnityEngine;
using FlowyGraph;
using System;
using System.Collections.Generic;
public class MessageBubbleManager : MonoBehaviour
{
    [SerializeField] private MessageBubble messageBubblePrefab;
    [SerializeField] private RectTransform bubbleRoot;

    private readonly Dictionary<CharacterData, MessageBubble> bubbleEntries = new Dictionary<CharacterData, MessageBubble>();

    private void Awake()
    {
    }

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    public void ShowMessage(CharacterData character, string message, Action onNext)
    {
        Transform anchor = null;
        var charManager = FlowyGraphBlackboard.GetClass<CharacterVisualManager>();
        if (charManager != null)
        {
            var visual = charManager.GetVisual(character);
            if (visual != null)
            {
                anchor = visual.MessageBubbleAnchor;
            }
        }

        if (anchor == null)
        {
            Debug.LogWarning($"Could not find anchor for character: {character.name}");
            return;
        }

        var autoHideCallback = BuildAutoHideCallback(character, onNext);
        if (bubbleEntries.TryGetValue(character, out var existingBubble))
        {
            existingBubble.SetData(character, anchor, message, autoHideCallback);
            existingBubble.RefreshPosition();
            existingBubble.transform.SetAsLastSibling();
            return;
        }

        var bubble = Instantiate(messageBubblePrefab, bubbleRoot);
        bubble.transform.position = anchor.position;
        bubble.transform.SetAsLastSibling();
        bubble.SetData(character, anchor, message, autoHideCallback);
        bubble.RefreshPosition();
        bubbleEntries.Add(character, bubble);
    }

    public void ClearMessage(CharacterData character)
    {
        ClearMessageInternal(character);  
    }

    public void ClearAllMessages()
    {
        if (bubbleEntries.Count == 0)
        {
            return;
        }

        var characters = new List<CharacterData>(bubbleEntries.Keys);
        foreach (var character in characters)
        {
            ClearMessageInternal(character);
        }
    }

    public void ClearMessage()
    {
        ClearAllMessages();
    }

    private Action BuildAutoHideCallback(CharacterData character, Action onNext)
    {
        return () =>
        {
            ClearMessageInternal(character);
            onNext?.Invoke();
        };
    }

    private void ClearMessageInternal(CharacterData character)
    {
        if (!bubbleEntries.TryGetValue(character, out var bubble))
        {
            return;
        }

        if (bubble != null)
        {
            bubble.StopAutoHide();
            Destroy(bubble.gameObject);
        }

        bubbleEntries.Remove(character);
    }
}
