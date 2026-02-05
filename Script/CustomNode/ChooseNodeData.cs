using System;
using System.Collections.Generic;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    [NodeName("Choose")]
    [NodeSearchGroup("Custom")]
    [NodePortEdit(canEditorOutputPortNum: true, canEditorOutputPortText: true)]
    [Serializable]
    public class ChooseNodeData : BaseNodeData
    {
        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);
            
            var manager = FlowyGraphBlackboard.GetClass<ChooseManager>();
            if (manager != null)
            {
                List<string> choices = new List<string>();
                if (outputPortDataList != null)
                {
                    foreach (var port in outputPortDataList)
                    {
                        choices.Add(port.PortName.Text);
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
