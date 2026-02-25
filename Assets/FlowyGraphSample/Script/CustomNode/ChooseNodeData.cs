using System;
using System.Collections.Generic;
using UnityEngine;
using FlowyGraph;
using U0UGames.Localization;

namespace FlowyGraph.Sample
{
    [NodeName("选择")]
    [NodeSearchGroup("自定义")]
    [NodePortEdit(canEditorOutputPortNum: true, canEditorOutputPortText: true)]
    [NodeIntroduce("显示一个多选一的选择界面供玩家交互。\n\n输入：打开选择界面并暂停流程。\n输出：玩家点击对应的选项后，从匹配的端口流出。")]
    [Serializable]
    public class ChooseNodeData : BaseNodeData
    {
        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);
            
            var manager = FlowyGraphBlackboard.GetClass<ChooseManager>();
            if (manager != null)
            {
                List<LocalizeString> choices = new List<LocalizeString>();
                if (outputPortDataList != null)
                {
                    foreach (var port in outputPortDataList)
                    {
                        choices.Add(new LocalizeString(port.PortName.Key));
                    }
                }

                manager.ShowChoices(choices, (index) => {
                    SetOutput(index);
                    MoveToNext();
                });
            }
            else
            {
                MoveToNext();
            }
        }
    }
}
