using System;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("Camera Focus Cancel")]
    [NodeSearchGroup("Custom/Camera")]
    [Serializable]
    public class CameraFocusCancelNodeData : BaseNodeData
    {
        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);

            var manager = FlowyGraphBlackboard.GetClass<CameraFocusManager>();
            manager?.CancelFocus();

            MoveToNext();
        }
    }
}
