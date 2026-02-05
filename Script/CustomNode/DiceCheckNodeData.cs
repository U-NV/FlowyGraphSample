using System;
using System.Collections.Generic;
using FlowyGraph;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("Dice Check")]
    [NodeSearchGroup("Custom/Check")]
    [NodePortEdit(canEditorOutputPortText: false)]
    [Serializable]
    public class DiceCheckNodeData : BaseNodeData
    {
        public OptionsSelector<FloatValueKey> attribute;
        public int difficulty = 12;
        public NodeTextData prompt;

        public DiceCheckNodeData()
        {
            outputPortDataList = new List<NodeOutputData>
            {
                new NodeOutputData("成功"),
                new NodeOutputData("失败")
            };
        }

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData, BaseNodeData prevNode,
            NodeEdgeData edgeData)
        {
            base.EnterNode(graph, flowData, prevNode, edgeData);

            var attributeValue = Mathf.RoundToInt(FlowyGraphBlackboard.GetFloatValue(attribute.Options, attribute.Index));
            var manager = FlowyGraphBlackboard.GetClass<DiceCheckManager>();
            var request = new DiceCheckRequest
            {
                attrValue = attributeValue,
                difficulty = difficulty,
                prompt = prompt.Text
            };

            if (manager != null)
            {
                manager.RequestCheck(request, success =>
                {
                    SetOutput(success ? 0 : 1);
                    MoveToNext();
                });
                return;
            }

            // 当找不到manager时，使用fallback逻辑
            var fallbackSuccess = ResolveFallback(request);
            SetOutput(fallbackSuccess ? 0 : 1);
            MoveToNext();
        }

        private bool ResolveFallback(DiceCheckRequest request)
        {
            var attrValue = request.attrValue;
            var roll = UnityEngine.Random.Range(1, 21);
            var total = roll + attrValue;
            return total >= request.difficulty;
        }
    }
}
