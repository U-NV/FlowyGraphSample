using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FlowyGraph;
using U0UGames.Localization;
using U0UGames.Localization.UI;

public class ItemInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject itemTextPrefab;
    [SerializeField] private LocalizeData emptyText = new LocalizeData("UI.Item.NoneTips","暂无线索");
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    private readonly Dictionary<string, ItemData> keyToItemData = new Dictionary<string, ItemData>();
    private readonly List<string> activeKeys = new List<string>();
    private readonly List<GameObject> spawnedItems = new List<GameObject>();

    private void Awake()
    {
        BuildIndex();
        SyncWithBlackboard();
        FlowyGraphBlackboard.RegisterClass(this);
    }

    private void OnDestroy()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    private void BuildIndex()
    {
        keyToItemData.Clear();
        foreach (var item in items)
        {
            if (item == null || string.IsNullOrEmpty(item.Key))
            {
                continue;
            }
            keyToItemData[item.Key] = item;
        }
    }

    public void SyncWithBlackboard()
    {
        activeKeys.Clear();
        var values = FlowyGraphBlackboard.GetStringValues();
        if (values == null)
        {
            UpdateUI();
            return;
        }

        foreach (var value in values)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            if (!keyToItemData.ContainsKey(value))
            {
                continue;
            }

            if (!activeKeys.Contains(value))
            {
                activeKeys.Add(value);
            }
        }

        UpdateUI();
    }

    public void HandleItemAdded(string key)
    {
        if (string.IsNullOrEmpty(key) || activeKeys.Contains(key))
        {
            return;
        }

        if (!keyToItemData.ContainsKey(key))
        {
            return;
        }

        activeKeys.Add(key);
        UpdateUI();
    }

    public void HandleItemRemoved(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        if (activeKeys.Remove(key))
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // 清理旧的预制件实例
        foreach (var obj in spawnedItems)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedItems.Clear();

        if (root == null || itemTextPrefab == null)
        {
            return;
        }

        // 根据 activeKeys 生成新的预制件
        if (activeKeys.Count == 0)
        {
            CreateEmptyText();
            return;
        }

        foreach (var key in activeKeys)
        {
            if (keyToItemData.TryGetValue(key, out var itemData))
            {
                CreateItemText(itemData);
            }
        }
    }

    private void CreateItemText(ItemData item)
    {
        var itemInstance = Instantiate(itemTextPrefab, root.transform);
        spawnedItems.Add(itemInstance);
        
        var text = itemInstance.GetComponentInChildren<LocalizeText>();
        if (text != null)
        {
            text.text = item.itemNameLocalizeData.LocalizeString;
        }
    }

    private void CreateEmptyText()
    {
        if (emptyText == null || string.IsNullOrEmpty(emptyText.LocalizeString.Value)) return;

        var itemInstance = Instantiate(itemTextPrefab, root.transform);
        spawnedItems.Add(itemInstance);

        var text = itemInstance.GetComponentInChildren<LocalizeText>();
        if (text != null)
        {
            text.text = emptyText.LocalizeString;
        }
    }
}
