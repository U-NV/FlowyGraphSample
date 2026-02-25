using System;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("取消相机聚焦")]
    [NodeSearchGroup("自定义/相机")]
    [NodeIntroduce("取消当前的相机聚焦状态，恢复到默认视角。\n\n输入：执行取消聚焦逻辑。\n输出：取消指令发出后立即触发。")]
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
