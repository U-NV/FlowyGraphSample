using System;
using UnityEngine;
using FlowyGraph;
using U0UGames.Localization;

namespace FlowyGraph.Sample
{
    [NodeName("消息气泡")]
    [NodeSearchGroup("自定义")]
    [NodeIntroduce("在角色头顶显示一个简短的消息气泡。\n\n输入：显示气泡并等待玩家点击或超时。\n输出：气泡消失后继续流程。")]
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

                manager.ShowMessage(character, new LocalizeString(message.Key), () => {
                    MoveToNext();
                });
            }
            else
            {
                // 如果没有管理器，直接跳过，防止卡死
                MoveToNext();
            }
        }

        // 如果节点图被终止了，需要清除消息气泡
        public override void ExitNode(FlowyGraphRuntime graph, GraphFlowData flowData)
        {
            var manager = FlowyGraphBlackboard.GetClass<MessageBubbleManager>();
            if (manager != null)
            {
                manager.ClearMessage(character);
            }
        }
    }
}
