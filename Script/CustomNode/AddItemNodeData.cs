using System;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("Add Item")]
    [NodeSearchGroup("Custom/Item")]
    [Serializable]
    public class AddItemNodeData : BaseNodeData
    {
        public ItemData item;

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            if (item == null)
            {
                Debug.LogWarning($"AddItemNodeData_{graph.FileName}_{NodeId}: item is null");
                MoveToNext();
                return;
            }

            if (FlowyGraphBlackboard.AddStringValue(item.Key))
            {
                var ui = FlowyGraphBlackboard.GetClass<ItemInventoryUI>();
                ui?.HandleItemAdded(item.Key);
            }
            MoveToNext();
        }
    }
}
