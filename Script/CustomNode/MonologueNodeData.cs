using System;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("独白")]
    [NodeSearchGroup("自定义")]
    [NodeIntroduce("显示角色的内心独白或旁白文本。\n\n输入：显示独白界面。\n输出：玩家点击继续后触发，并开始关闭界面。")]
    [Serializable]
    public class MonologueNodeData : BaseNodeData
    {
        public CharacterData character;
        public NodeTextData content;

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {   
            var characterData = character;
            if (characterData == null)
            {
                characterData = flowData.GetData<CharacterData>(FlowDataKey.CharacterKey);
            }

            var manager = FlowyGraphBlackboard.GetClass<MonologueManager>();
            if (manager != null)
            {
                manager.ShowMonologue(characterData, content.Text, () => {
                    MoveToNext();
                });
            }
            else
            {
                MoveToNext();
            }
        }

        public override void ExitNode(FlowyGraphRuntime graph, GraphFlowData flowData)
        {
            base.ExitNode(graph, flowData);
            var manager = FlowyGraphBlackboard.GetClass<MonologueManager>();
            manager?.StartClosing();
        }
    }
}
