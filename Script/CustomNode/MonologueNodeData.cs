using System;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("Monologue")]
    [NodeSearchGroup("Custom")]
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
