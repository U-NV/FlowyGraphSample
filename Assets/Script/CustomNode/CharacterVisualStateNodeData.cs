using System;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("角色显示状态")]
    [NodeSearchGroup("自定义/角色")]
    [NodeIntroduce("设置角色在场景中的显示或隐藏状态。\n\n输入：修改角色显示状态。\n输出：状态设置后立即触发。")]
    [Serializable]
    public class CharacterVisualStateNodeData : BaseNodeData
    {
        [Tooltip("要操作的角色数据，如果为空则从流数据中获取")]
        public CharacterData character;
        
        [Tooltip("是否显示角色")]
        public bool isVisible = true;

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);

            // 1. 确定目标角色
            CharacterData targetCharacter = character;
            if (targetCharacter == null)
            {
                targetCharacter = flowData.GetData<CharacterData>(FlowDataKey.CharacterKey);
            }

            if (targetCharacter != null)
            {
                // 2. 通过 Blackboard 获取 CharacterVisualManager
                var visualManager = FlowyGraphBlackboard.GetClass<CharacterVisualManager>();
                if (visualManager != null)
                {
                    // 3. 找到对应的 CharacterVisual 并修改其状态
                    var visual = visualManager.GetVisual(targetCharacter);
                    if (visual != null)
                    {
                        visual.SetVisible(isVisible);
                    }
                    else
                    {
                        Debug.LogWarning($"[CharacterVisualStateNodeData] 未找到角色 {targetCharacter.name} 的 Visual 实例。");
                    }
                }
                else
                {
                    Debug.LogError("[CharacterVisualStateNodeData] CharacterVisualManager 未注册到 Blackboard。");
                }
            }
            else
            {
                Debug.LogWarning("[CharacterVisualStateNodeData] 未指定角色且流数据中没有角色信息。");
            }

            // 4. 继续流程
            MoveToNext();
        }
    }
}
