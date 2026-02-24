using System;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("设置角色")]
    [NodeSearchGroup("自定义")]
    [NodeIntroduce("在当前的流程数据中设置一个全局引用的角色，后续节点可直接引用此角色。\n\n输入：设置角色引用。\n输出：设置完成后立即触发。")]
    [Serializable]
    public class SetCharacterNodeData : BaseNodeData
    {
        public CharacterData character;

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);

            flowData.SetData(FlowDataKey.CharacterKey, character);
            MoveToNext();
        }
    }
}
