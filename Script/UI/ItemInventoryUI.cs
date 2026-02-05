using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using FlowyGraph;

public class ItemInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text itemListText;
    [SerializeField] private string emptyText = "暂无线索";
    [SerializeField] private string linePrefix = "- ";
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    private readonly Dictionary<string, string> keyToName = new Dictionary<string, string>();
    private readonly List<string> activeKeys = new List<string>();

    private void Awake()
    {
        BuildIndex();
        SyncWithBlackboard();
    }

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
        SyncWithBlackboard();
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    private void BuildIndex()
    {
        keyToName.Clear();
        foreach (var item in items)
        {
            if (item == null || string.IsNullOrEmpty(item.Key))
            {
                continue;
            }
            var displayName = string.IsNullOrEmpty(item.itemName) ? item.name : item.itemName;
            keyToName[item.Key] = displayName;
        }
    }

    public void SyncWithBlackboard()
    {
        activeKeys.Clear();
        var values = FlowyGraphBlackboard.GetStringValues();
        if (values == null)
        {
            UpdateText();
            return;
        }

        foreach (var value in values)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            if (!keyToName.ContainsKey(value))
            {
                continue;
            }

            if (!activeKeys.Contains(value))
            {
                activeKeys.Add(value);
            }
        }

        UpdateText();
    }

    public void HandleItemAdded(string key)
    {
        if (string.IsNullOrEmpty(key) || activeKeys.Contains(key))
        {
            return;
        }

        if (!keyToName.ContainsKey(key))
        {
            return;
        }

        activeKeys.Add(key);
        UpdateText();
    }

    public void HandleItemRemoved(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        if (activeKeys.Remove(key))
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (root != null)
        {
            root.SetActive(true);
        }

        if (itemListText == null)
        {
            return;
        }

        if (activeKeys.Count == 0)
        {
            itemListText.text = emptyText;
            return;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < activeKeys.Count; i++)
        {
            if (i > 0)
            {
                builder.Append('\n');
            }

            if (!string.IsNullOrEmpty(linePrefix))
            {
                builder.Append(linePrefix);
            }

            var key = activeKeys[i];
            if (keyToName.TryGetValue(key, out var displayName))
            {
                builder.Append(displayName);
            }
            else
            {
                builder.Append(key);
            }
        }

        itemListText.text = builder.ToString();
    }
}
