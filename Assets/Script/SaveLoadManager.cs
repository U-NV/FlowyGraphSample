using System;
using System.Collections.Generic;
using FlowyGraph;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private const string SaveKey = "FlowyGraphSave";
    private const string EmptySnapshotJson = "{}";

    [Serializable]
    private class SaveSnapshot
    {
        public string blackboardJson;
        public PlayerState player;
        public CharacterVisualState[] characters;
    }

    [Serializable]
    private class PlayerState
    {
        public Vector3 position;
    }

    [Serializable]
    private class CharacterVisualState
    {
        public string id;
        public bool isVisible;
    }

    private PlayerController playerController;
    private CharacterVisual[] cachedCharacterVisuals;
    private PlayerState initialPlayerState;
    private CharacterVisualState[] initialCharacterStates;

    private void Awake()
    {
        CacheReferences();
        CaptureInitialState();
    }

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    public void SaveGame()
    {
        var snapshot = BuildSnapshot();
        var json = JsonUtility.ToJson(snapshot);
        Debug.Log($"[SaveLoadManager] SaveGame: {json}");
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        var json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        var snapshot = JsonUtility.FromJson<SaveSnapshot>(json);
        if (snapshot == null)
        {
            return;
        }

        Debug.Log($"[SaveLoadManager] LoadGame: {json}");
        ApplySnapshot(snapshot);
    }

    public void ResetGame()
    {
        FlowyGraphBlackboard.ImportValuesFromJson(EmptySnapshotJson, true);
        ApplyPlayerState(initialPlayerState);
        ApplyCharacterStates(initialCharacterStates);
        RefreshInventoryUI();
    }

    public void LoadOrReset()
    {
        var json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            var snapshot = JsonUtility.FromJson<SaveSnapshot>(json);
            if (snapshot != null)
            {
                ApplySnapshot(snapshot);
                return;
            }
        }

        ResetGame();
    }

    private void CacheReferences()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        cachedCharacterVisuals = FindObjectsOfType<CharacterVisual>(true);
    }

    private void CaptureInitialState()
    {
        initialPlayerState = BuildPlayerState(playerController);
        initialCharacterStates = BuildCharacterStates(cachedCharacterVisuals);
    }

    private SaveSnapshot BuildSnapshot()
    {
        return new SaveSnapshot
        {
            blackboardJson = FlowyGraphBlackboard.ExportValuesToJson(),
            player = BuildPlayerState(GetPlayerController()),
            characters = BuildCharacterStates(GetCharacterVisuals())
        };
    }

    private void ApplySnapshot(SaveSnapshot snapshot)
    {
        if (!string.IsNullOrEmpty(snapshot.blackboardJson))
        {
            FlowyGraphBlackboard.ImportValuesFromJson(snapshot.blackboardJson, true);
        }

        ApplyPlayerState(snapshot.player);
        ApplyCharacterStates(snapshot.characters);
        RefreshInventoryUI();
    }

    private PlayerController GetPlayerController()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        return playerController;
    }

    private CharacterVisual[] GetCharacterVisuals()
    {
        return FindObjectsOfType<CharacterVisual>(true);
    }

    private PlayerState BuildPlayerState(PlayerController controller)
    {
        if (controller == null)
        {
            return null;
        }

        return new PlayerState
        {
            position = controller.transform.position
        };
    }

    private CharacterVisualState[] BuildCharacterStates(CharacterVisual[] visuals)
    {
        if (visuals == null || visuals.Length == 0)
        {
            return Array.Empty<CharacterVisualState>();
        }

        var result = new List<CharacterVisualState>(visuals.Length);
        foreach (var visual in visuals)
        {
            if (visual == null)
            {
                continue;
            }

            var id = visual.SaveId;
            if (string.IsNullOrEmpty(id))
            {
                continue;
            }

            result.Add(new CharacterVisualState
            {
                id = id,
                isVisible = visual.IsVisible
            });
        }

        return result.ToArray();
    }

    private void ApplyPlayerState(PlayerState state)
    {
        if (state == null)
        {
            return;
        }

        var controller = GetPlayerController();
        if (controller == null)
        {
            return;
        }

        controller.SetPosition(state.position);
    }

    private void ApplyCharacterStates(CharacterVisualState[] states)
    {
        if (states == null || states.Length == 0)
        {
            return;
        }

        var stateById = new Dictionary<string, bool>(states.Length);
        foreach (var state in states)
        {
            if (state == null || string.IsNullOrEmpty(state.id))
            {
                continue;
            }

            stateById[state.id] = state.isVisible;
        }

        var visuals = GetCharacterVisuals();
        foreach (var visual in visuals)
        {
            if (visual == null)
            {
                continue;
            }

            if (stateById.TryGetValue(visual.SaveId, out var visible))
            {
                visual.SetVisible(visible);
            }
        }
    }

    private void RefreshInventoryUI()
    {
        var inventory = FlowyGraphBlackboard.GetClass<ItemInventoryUI>();
        inventory?.SyncWithBlackboard();
    }
}
