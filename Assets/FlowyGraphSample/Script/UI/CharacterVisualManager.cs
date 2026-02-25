using UnityEngine;
using System.Collections.Generic;
using FlowyGraph;

public class CharacterVisualManager : MonoBehaviour
{
    private List<CharacterVisual> visuals = new List<CharacterVisual>();

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
        RefreshVisuals();
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    public void RefreshVisuals()
    {
        visuals.Clear();
#if UNITY_2022_2_OR_NEWER
        visuals.AddRange(Object.FindObjectsByType<CharacterVisual>(FindObjectsInactive.Include, FindObjectsSortMode.None));
#else
        visuals.AddRange(Object.FindObjectsByType<CharacterVisual>(FindObjectsSortMode.None));
#endif
    }

    public CharacterVisual GetVisual(CharacterData data)
    {
        foreach (var visual in visuals)
        {
            if (visual != null && visual.CharacterData == data)
            {
                return visual;
            }
        }
        
        // 如果没找到，尝试重新刷新一次（应对动态生成的角色）
        RefreshVisuals();
        foreach (var visual in visuals)
        {
            if (visual != null && visual.CharacterData == data)
            {
                return visual;
            }
        }
        
        return null;
    }
}
