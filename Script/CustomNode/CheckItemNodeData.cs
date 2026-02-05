using System;
using System.Collections.Generic;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("Check Item")]
    [NodeSearchGroup("Custom/Item")]
    [Serializable]
    public class CheckItemNodeData : BaseNodeData
    {
        public ItemData item;

        public CheckItemNodeData()
        {
            inputPortDataList = new List<NodeInputData>()
            {
                new NodeInputData(null)
            };
            outputPortDataList = new List<NodeOutputData>
            {
                new NodeOutputData("Have"),
                new NodeOutputData("Not Have")
            };
        }

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            if (item == null)
            {
                Debug.LogWarning($"CheckItemNodeData_{graph.FileName}_{NodeId}: item is null");
                MoveToNext();
                return;
            }

            SetOutput(FlowyGraphBlackboard.HasStringValue(item.Key) ? 0 : 1);
            MoveToNext();
        }
    }
}
