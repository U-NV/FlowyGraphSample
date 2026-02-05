using System;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("Character Move")]
    [NodeSearchGroup("Custom/Character")]
    [Serializable]
    public class CharacterMoveNodeData : BaseNodeData
    {
        [Tooltip("要操作的角色数据，如果为空则从流数据中获取")]
        public CharacterData character;

        [Tooltip("移动的偏移量（正数为右，负数为左）")]
        public float moveOffset;

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
                    // 3. 找到对应的 CharacterVisual 并修改其位置
                    var visual = visualManager.GetVisual(targetCharacter);
                    if (visual != null)
                    {
                        Vector3 currentPos = visual.transform.localPosition;
                        currentPos.x += moveOffset;
                        visual.transform.localPosition = currentPos;
                    }
                    else
                    {
                        Debug.LogWarning($"[CharacterMoveNodeData] 未找到角色 {targetCharacter.name} 的 Visual 实例。");
                    }
                }
                else
                {
                    Debug.LogError("[CharacterMoveNodeData] CharacterVisualManager 未注册到 Blackboard。");
                }
            }
            else
            {
                Debug.LogWarning("[CharacterMoveNodeData] 未指定角色且流数据中没有角色信息。");
            }

            // 4. 继续流程
            MoveToNext();
        }
    }
}
