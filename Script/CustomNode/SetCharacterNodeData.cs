using System;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("Set Character")]
    [NodeSearchGroup("Custom")]
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
