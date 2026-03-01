using UnityEditor.Callbacks;
using FlowyGraph.InspectorView;

namespace FlowyGraph
{
    /// <summary>
    /// 默认的节点图编辑器窗口，用于处理基础的 NodeDataContainerSO 资产
    /// </summary>
    public class AllNodesGraphEditorWindow : NodeGraphEditorView<AllNodesFlowGraphAsset, AllNodesGraphEditorWindow>
    {
        [OnOpenAsset(0)] // 优先级设为 0，作为基础类型的兜底打开方案
        public static bool OnOpenAsset(int instanceID, int line)
        {
            return TryOpen(instanceID);
        }

        protected override NodeInspectorView GetCustomInspectorView()
        {
            return new NodeFlowInspectorView();
        }
    }
}
