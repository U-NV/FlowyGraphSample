using System;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("添加道具")]
    [NodeSearchGroup("自定义/道具")]
    [NodeIntroduce("向玩家背包中添加指定的道具。\n\n输入：进入该节点时执行添加逻辑。\n输出：添加完成后立即触发。")]
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
