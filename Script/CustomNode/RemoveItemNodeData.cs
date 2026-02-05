using System;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("Remove Item")]
    [NodeSearchGroup("Custom/Item")]
    [Serializable]
    public class RemoveItemNodeData : BaseNodeData
    {
        public ItemData item;

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            if (item == null)
            {
                Debug.LogWarning($"RemoveItemNodeData_{graph.FileName}_{NodeId}: item is null");
                MoveToNext();
                return;
            }

            if (FlowyGraphBlackboard.RemoveStringValue(item.Key))
            {
                var ui = FlowyGraphBlackboard.GetClass<ItemInventoryUI>();
                ui?.HandleItemRemoved(item.Key);
            }
            MoveToNext();
        }
    }
}
