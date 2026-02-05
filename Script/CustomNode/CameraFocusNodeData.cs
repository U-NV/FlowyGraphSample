using System;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("Camera Focus")]
    [NodeSearchGroup("Custom/Camera")]
    [Serializable]
    public class CameraFocusNodeData : BaseNodeData
    {
        [Tooltip("要聚焦的角色数据，如果为空则从流数据中获取")]
        public CharacterData character;

        [Tooltip("聚焦时的缩放倍率，<=0 表示使用管理器默认值")]
        public float zoomMultiplier = 0f;

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);

            var targetCharacter = character ?? flowData.GetData<CharacterData>(FlowDataKey.CharacterKey);
            var manager = FlowyGraphBlackboard.GetClass<CameraFocusManager>();

            if (manager != null && targetCharacter != null)
            {
                manager.FocusOnCharacter(targetCharacter, zoomMultiplier);
            }

            MoveToNext();
        }
    }
}
