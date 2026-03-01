---
name: flowygraph-custom-node
description: FlowyGraph 自定义节点与编辑器扩展开发指南。在用户询问如何创建自定义节点、继承 BaseNodeData、实现 INodeGraphExtension、编写编辑器扩展、适配层级视图/错误检查/节点颜色、使用 Blackboard、NodeTextData、GraphFlowData、信号系统或本地化时使用此 skill。
---

# FlowyGraph 自定义节点开发指南

## 模块一：创建自定义节点

### 基础结构

所有自定义节点必须继承 `BaseNodeData`，在 `namespace FlowyGraph` 内声明，并标注 `[System.Serializable]`。

```csharp
// 在 AutoGenNodeDataModule.cs 中添加（插件工具箱"添加新脚本"会自动完成）
public partial class CustomNodeDataModule
{
    public NodeDataList<MyCustomNodeData> MyCustomNodeData = new();
}
```

节点脚本本体：

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlowyGraph
{
    [Serializable]
    [NodeName("我的节点")]
    [NodeSearchGroup("游戏/自定义")]
    [NodeIntroduce("节点的功能说明。\n\n输入：触发时执行逻辑。\n输出：完成后继续流程。")]
    public class MyCustomNodeData : BaseNodeData
    {
        // 字段声明（见下方字段类型说明）
        public NodeTextData dialogueText;

        public override void Init(FlowyGraphRuntime graph)
        {
            base.Init(graph);
            // 重置所有私有运行时状态（计时器、标志位等）
        }

        public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData,
            BaseNodeData prevNode, NodeEdgeData edgeData)
        {
            // 主逻辑
            MoveToNext();
        }
    }
}
```

**同步步骤（必须）：** 创建或修改节点脚本后，执行菜单 `工具 > FlowyGraph > 添加新编写的脚本到节点图`，等待提示框出现后即完成。

---

### Attributes（编辑器外观）

| Attribute | 说明 |
| --- | --- |
| `[NodeName("名称")]` | 节点标题栏显示名 |
| `[NodeSearchGroup("分类/子类")]` | 右键搜索菜单分组路径 |
| `[NodeIcon("Icons/xxx")]` | 节点图标，路径为 Resources 目录下的相对路径 |
| `[NodePortEdit(canEditorOutputPortNum, canEditorInputPortNum, canEditorOutputPortText, canEditorInputPortText)]` | 控制端口是否可在编辑器中增删或编辑文本 |
| `[NodeHideProperty("fieldName")]` | 在 Inspector 中隐藏指定字段 |
| `[NodeStyleName("style-key")]` | 节点颜色分组，在工具箱"节点颜色"页签中配置对应颜色 |
| `[NodeLockedOutputPort(0, 1)]` | 锁定指定索引的输出端口文本，使其不可在编辑器中修改 |
| `[NodeIntroduce("说明")]` | 节点工具提示/说明文本 |

---

### 生命周期方法

- **`Init(graph)`** — 图每次启动或重启时调用。必须调用 `base.Init(graph)`，并在此重置所有私有运行时状态（计时器、标志位、列表等）。若不重写，图重启后状态会残留导致 Bug。
- **`EnterNode(graph, flowData, prevNode, edgeData)`** — 节点被激活时的主逻辑入口。
- **`ExitNode(graph, flowData)`** — 节点结束后的清理钩子（可选）。
- **`Tick(deltaTime)`** — 帧更新，需同时声明 `public override bool NeedTick => true;` 才会被调用。

---

### 流程推进

```csharp
// 单输出：直接推进
MoveToNext();

// 多输出（分支）：先指定端口索引，再推进
SetOutput(1);
MoveToNext();

// 异步场景（对话、动画、等待玩家操作）：
// 不在 EnterNode 末尾调用，而是在完成回调中调用
public override void EnterNode(...)
{
    var manager = FlowyGraphBlackboard.GetClass<DialogueManager>();
    manager?.Show(dialogueText.Text, onComplete: () => MoveToNext());
    // 注意：此处不调用 MoveToNext()
}
```

---

### 字段类型选择

**NodeTextData vs string：**

- 需要本地化导出的文本字段，必须使用 `NodeTextData` 而非 `string`
- 单节点单句文本 → 放在节点自身的 `NodeTextData` 字段中
- 多输出分支文本（选项节点、消息分支节点）→ 放在**输出端口的 `PortName`** 中（见下方）

**输出端口文本（PortName）用于多分支节点：**

当节点有多个输出分支，且每个分支的含义需要文字描述时（如选项节点），将文本放在输出端口的 `PortName` 中比放在节点字段里可读性更高——策划在连线处即可看到各分支内容。

配置方式：
```csharp
[NodePortEdit(canEditorOutputPortNum: true, canEditorOutputPortText: true)]
[NodeLockedOutputPort(0)]  // 锁定特殊用途的端口（如"全部完成"）
public class MyChoiceNodeData : BaseNodeData
{
    // 构造函数中预设端口
    public MyChoiceNodeData()
    {
        outputPortDataList = new List<NodeOutputData>
        {
            new NodeOutputData("全部完成"),  // 索引0，被锁定
            new NodeOutputData("选项1"),
            new NodeOutputData("选项2"),
        };
    }
}
```

运行时读取端口文本：
```csharp
// outputPortDataList[i].PortName 返回 NodeTextData
NodeTextData choiceText = outputPortDataList[i].PortName;
string text = choiceText.Text;  // 获取实际文本
```

参考实现：`Core/RunTime/GeneralNodeData/LoopChooseNodeData.cs`（输出端口 1+ 为选项，端口 0 为"全部完成"）

---

### 节点间数据传递（GraphFlowData）

在同一次流程执行中，可通过 `flowData` 在节点之间传递临时数据：

```csharp
// 上游节点写入
flowData.SetData("SelectedIndex", selectedIndex);

// 下游节点读取
var index = flowData.GetData("SelectedIndex");
```

---

### 动态端口（进阶）

当端口数量需要根据数据动态变化时（如根据配置列表生成输出端口数量）：

```csharp
// 声明哪些字段变化时触发端口刷新
public override List<string> PropertyNamesToUpdateDynamicPorts
{
    get
    {
        var list = new List<string>(base.PropertyNamesToUpdateDynamicPorts);
        list.Add(nameof(myDataList));
        return list;
    }
}

// 根据数据生成端口列表
public override List<NodeOutputData> GetDynamicOutputPorts()
{
    return myDataList
        .Select((item, i) => new NodeOutputData(item.name))
        .ToList();
}
```

---

## 模块二：通过 Blackboard 与外部系统交互

**核心原则：** 节点不应直接引用 GameObject 或场景中的具体对象，应通过 `FlowyGraphBlackboard` 以类型为键访问外部管理器，实现解耦。

**外部系统（MonoBehaviour）注册自身：**

```csharp
public class DialogueManager : MonoBehaviour
{
    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
    }
    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }
    public void Show(string text, Action onComplete) { /* ... */ }
}
```

**节点内访问：**

```csharp
public override void EnterNode(FlowyGraphRuntime graph, GraphFlowData flowData,
    BaseNodeData prevNode, NodeEdgeData edgeData)
{
    var manager = FlowyGraphBlackboard.GetClass<DialogueManager>();
    if (manager == null) { MoveToNext(); return; }
    manager.Show(dialogueText.Text, onComplete: MoveToNext);
}
```

**全局变量 API：**

```csharp
// 分支变量（基于 BranchOption ScriptableObject）
int index = FlowyGraphBlackboard.GetBranchValue(branchOption);
FlowyGraphBlackboard.SetBranchValue(branchOption, index);

// 浮点数值（基于 FloatValueKey ScriptableObject）
float value = FlowyGraphBlackboard.GetFloatValue(floatKey);
FlowyGraphBlackboard.SetFloatValue(floatKey, 0, value);

// 存档读档
string json = FlowyGraphBlackboard.ExportValuesToJson();
FlowyGraphBlackboard.ImportValuesFromJson(json);
```

---

## 模块三：信号系统

信号用于图与图之间、或外部代码与图之间的异步事件通信。

**从外部代码触发信号（如玩家点击按钮）：**

```csharp
using FlowyGraph;

public class InteractionHandler : MonoBehaviour
{
    public SignalOption interactSignal;  // 在 Inspector 中拖入 SignalOption 资产

    public void OnUserClick()
    {
        FlowyGraphSystem.SendSignal(interactSignal);
    }
}
```

图内使用内置的 `WaitSignal` 节点等待信号，`SendSignal` 节点发出信号。`SignalOption` 资产通过右键 `Create > FlowyGraph > SignalOption` 创建。

---

## 模块四：创建编辑器扩展

编辑器扩展允许向工具箱注入自定义逻辑，无需修改插件核心代码。

**接口定义：** `Core/Editor/NodeGraphToolInterfaces.cs`
**参考实现：** `Core/Editor/Extensions/GeneralNodeGraphExtension.cs`

**自动注册机制：** 只需在编辑器程序集中实现下列接口，`NodeGraphToolRegistry` 会在编辑器启动时通过反射自动发现并注册，无需任何手动注册代码。

### INodeGraphExtension（注入现有工具标签页）

```csharp
using FlowyGraph.Editor;
using System;
using System.Collections.Generic;

namespace YourGame.Editor
{
    public class MyGameExtension : INodeGraphExtension
    {
        // 1. 注册错误检查函数
        public void RegisterErrorCheckers(List<Func<BaseNodeData, FlowyGraphAsset, string>> checkers)
        {
            checkers.Add((node, asset) =>
            {
                if (node is MyCustomNodeData myNode && myNode.dialogueText.Text == "")
                    return "对话文本不能为空";
                return null;  // null 表示无错误
            });
        }

        // 2. 向层级视图添加过滤模式
        public void RegisterHierarchyFilters(Dictionary<string, List<Type>> filters)
        {
            filters["我的节点"] = new List<Type> { typeof(MyCustomNodeData) };
        }

        // 3. 填充层级视图中的节点显示内容
        public void OnUpdateHierarchy(ContainerHierarchyNodeData rootData,
            BaseNodeData node, string currentMode)
        {
            if (currentMode == "我的节点")
            {
                NodeGraphToolUtility.TryAddNode<MyCustomNodeData>(rootData, node,
                    target => $"对话: {target.dialogueText.Text}",
                    target => null);
            }
        }
    }
}
```

### INodeGraphToolTab（添加全新工具标签页）

```csharp
public class MyToolTab : INodeGraphToolTab
{
    public string TabName => "我的工具";
    public int Order => 10;

    public void OnEnable() { /* 初始化 */ }

    public void OnGUI()
    {
        GUILayout.Label("自定义工具内容");
        if (GUILayout.Button("执行操作")) { /* ... */ }
    }
}
```

---

## 模块五：编辑器界面适配新节点

| 界面区域 | 适配方式 |
| --- | --- |
| 右键搜索菜单 | `[NodeSearchGroup("分类/子类")]`，支持多级路径 |
| 层级视图过滤 | `RegisterHierarchyFilters` 注册新的过滤模式名 |
| 层级视图显示 | `OnUpdateHierarchy` + `NodeGraphToolUtility.TryAddNode<T>()` |
| 错误检查 | `RegisterErrorCheckers` 添加校验函数 |
| Inspector 隐藏字段 | `[NodeHideProperty("fieldName")]` |
| 节点背景颜色 | `[NodeStyleName("my-style")]`，在工具箱"节点颜色"页签中为该 key 配置颜色 |

---

## 模块六：本地化支持

- 需要导出到本地化 JSON 的文本字段，使用 `NodeTextData` 类型（而非 `string`）
- 输出端口的 `PortName` 同样是 `NodeTextData` 类型，会自动被纳入本地化导出
- 在工具箱的"本地化"标签页中，点击"生成并导出所有本地化数据"，产出 `FlowyGraphLocalizationExport.json`
- Key 格式为 `[文件名].[节点ID].[索引]`，由插件自动生成，不要手动修改
