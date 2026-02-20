using System;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("移除道具")]
    [NodeSearchGroup("自定义/道具")]
    [NodeIntroduce("从玩家背包中移除指定的道具。\n\n输入：执行移除逻辑。\n输出：移除完成后立即触发。")]
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
