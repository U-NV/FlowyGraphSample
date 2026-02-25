using System;
using System.Collections.Generic;
using FlowyGraph;
using U0UGames.Localization;
using UnityEngine;

namespace FlowyGraph.Sample
{
    [NodeName("骰子检定")]
    [NodeSearchGroup("自定义/检定")]
    [NodePortEdit(canEditorOutputPortText: false)]
    [NodeIntroduce("进行一次基于角色属性的 1d20 骰子检定。\n\n输入：发起检定请求并显示 UI。\n输出：\n- 成功：骰子结果 + 属性值 >= 难度时触发。\n- 失败：检定未通过时触发。")]
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

            // 计算属性调整值
            attributeValue = Mathf.FloorToInt((attributeValue - 10)/2.0f);
            var request = new DiceCheckRequest
            {
                attrValue = attributeValue,
                difficulty = difficulty,
                prompt = new LocalizeString(prompt.Key)
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
