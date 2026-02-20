using System;
using System.Collections.Generic;
using UnityEngine;
using FlowyGraph;

namespace FlowyGraph.Sample
{
    /// <summary>
    /// 循环选项节点 - 具体UI实现
    /// 继承自 LoopChooseNodeData 基类，通过 ChooseManager 显示选项界面
    /// </summary>
    [NodeName("循环选择")]
    [NodeSearchGroup("自定义")]
    [NodePortEdit(canEditorOutputPortNum: true, canEditorOutputPortText: true)]
    [NodeLockedOutputPort(0)]
    [NodeIntroduce("循环选择的具体 UI 实现。允许玩家重复选择，直到所有选项都被选完。\n\n输入：\n- 重置并打开：重置状态并显示 UI。\n- 选项回调：逻辑完成后返回，检查剩余项。\n输出：\n- 全部完成：所有选项选完后触发。\n- 选项路径：被选中的对应选项流出。")]
    [Serializable]
    public class CustomLoopChooseNodeData : LoopChooseNodeData
    {
        /// <summary>
        /// 通过 ChooseManager 显示选项界面
        /// </summary>
        protected override void OnShowChoices(List<NodeTextData> choices, List<Action> onSelected)
        {
            var manager = FlowyGraphBlackboard.GetClass<ChooseManager>();
            if (manager != null)
            {
                var choiceTexts = new List<string>();
                foreach (var choice in choices)
                {
                    choiceTexts.Add(choice.Text);
                }
                // ChooseManager 回调传回显示列表中的索引，直接触发对应闭包即可
                manager.ShowChoices(choiceTexts, (index) =>
                {
                    if (index >= 0 && index < onSelected.Count)
                    {
                        onSelected[index]?.Invoke();
                    }
                });
            }
            else
            {
                Debug.LogError("CustomLoopChooseNodeData: 未找到ChooseManager，请确保场景中已注册ChooseManager");
                MoveToNext();
            }
        }
    }
}
