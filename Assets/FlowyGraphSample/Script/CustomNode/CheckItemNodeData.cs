using System;
using System.Collections.Generic;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("检查道具")]
    [NodeSearchGroup("自定义/道具")]
    [NodeIntroduce("检查玩家背包中是否拥有指定的道具。\n\n输入：进入该节点时进行道具检查。\n输出：\n- 拥有：玩家拥有该道具时触发。\n- 未拥有：玩家不拥有该道具时触发。")]
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
                new NodeOutputData("拥有"),
                new NodeOutputData("未拥有")
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
