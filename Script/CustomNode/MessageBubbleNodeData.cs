using System;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("Message Bubble")]
    [NodeSearchGroup("Custom")]
    [Serializable]
    public class MessageBubbleNodeData : BaseNodeData
    {
        public CharacterData character;
        public NodeTextData message;
        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);
            
            var manager = FlowyGraphBlackboard.GetClass<MessageBubbleManager>();
            if (manager != null)
            {
                character = character != null ? character : flowData.GetData<CharacterData>(FlowDataKey.CharacterKey);
                if (character == null)
                {
                    Debug.LogWarning("MessageBubbleNodeData: 当前流程未设置角色数据。");
                    MoveToNext();
                    return;
                }

                string messageText = message.Text ?? "";;

                manager.ShowMessage(character, messageText, () => {
                    MoveToNext();
                });
            }
            else
            {
                // 如果没有管理器，直接跳过，防止卡死
                MoveToNext();
            }
        }
    }
}
