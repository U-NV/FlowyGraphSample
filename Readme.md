# FlowyGraphSample

这是一个关于 **FlowyGraph** 插件的官方示例项目。

### 官方文档

有关该项目的详细说明、快速上手指南及 API 参考，请访问网页版文档：

👉 **[FlowyGraph 官方文档](https://u0ugames.fun/flowy-graph-docs/)**

# 示例项目详解 (FlowyGraphSample)

为了展示 FlowyGraph 在实际游戏开发中的应用，我们提供了一个完整的示例项目：**[FlowyGraphSample](https://github.com/U-NV/FlowyGraphSample)**。

这是一个基于“公园冻死案”背景的推理探案小游戏，展示了如何利用节点图驱动复杂的剧情、对话、证据收集以及逻辑判定。

🎮 **在线试玩**：👉 [FlowyGraphSample on itch.io](https://u0ugames.itch.io/flowy-graph-sample)

---

## 1. 项目结构一览

示例项目位于 `Assets/FlowyGraphSample/` 目录下，其核心组织如下：

- **`FlowyGraphSample.unity`**: 演示场景。包含 3D 角色、交互物品、UI 管理器以及全局逻辑入口。
- **`FlowyGraphAsset/`**: 存放所有的节点图资产（`.asset`）。每个文件代表一个剧情片段或逻辑流。
- **`Script/`**: 核心逻辑代码。
    - **`CustomNode/`**: 示例项目特有的自定义节点实现（如对话、掷骰、物品检查）。
    - **`UI/`**: UI 管理器（Monologue, Choose, DiceCheck 等），通过 `FlowyGraphBlackboard` 与节点解耦。
    - **`DataStruct/`**: 游戏业务相关的数据结构定义。
- **`Data/`**: 存放游戏数据资产，按类别组织：
    - **`角色/`**: `CharacterData`（角色信息）。
    - **`线索/`**: `ItemData`（证据物品）。
    - **`数值/`** & **`分支判断/`**: 预设的逻辑判定数据。
- **`Localization/`**: 本地化资源，包含多语言 JSON 文本及原始 Excel 表格。
- **`Art/`**: 美术资源，包括字体（Font）、材质（Material）和精灵图（Sprite）。
- **`Prefabs/`**: 预制体资源，如玩家角色、交互物品、UI 组件等。

---

## 2. 核心工作流解析

### 场景启动与加载
在场景中的 `GameManager` 对象上挂载了 `FlowyGraphSampleManager` 脚本。

1. **初始化数据**：启动时通过 `SaveLoadManager` 加载存档或重置 `FlowyGraphBlackboard`。
2. **启动首个图**：实例化 `FlowyGraphRuntime` 并加载 `introGraphAsset`（通常是开场白或主菜单流程），随后调用 `Start()` 驱动逻辑。

### 对话与交互系统 (Monologue)
项目定义了 `MonologueNodeData` 自定义节点：

- **节点配置**：在编辑器中指定角色（`CharacterData`）和对话内容（`NodeTextData`）。
- **运行逻辑**：
    1. 节点进入时，通过 `FlowyGraphBlackboard.GetClass<MonologueManager>()` 获取 UI 引用。
    2. 调用 `manager.ShowMonologue()` 显示对话。
    3. 玩家点击 UI 后，触发回调执行 `MoveToNext()`，流程流向下一个节点。

### 分支选择系统 (Choose)
通过 `ChooseNodeData` 实现：

- **动态端口**：利用 `[NodePortEdit]` 特性，允许开发者在编辑器中自由增删输出端口，每个端口代表一个选项。
- **UI 映射**：节点会将所有端口的名称发送给 `ChooseManager` 生成按钮。玩家点击后，节点根据索引调用 `SetOutput(index)` 并跳转。

---

## 3. 进阶示例：掷骰判定 (Dice Check)

这是示例项目中最复杂的逻辑展示，位于 `DiceCheckNodeData`：

1. **属性关联**：使用 `OptionsSelector<FloatValueKey>` 关联 Blackboard 中的数值（如“观察力”）。
2. **异步判定**：
    - 节点进入后，请求 `DiceCheckManager` 开启掷骰 UI。
    - UI 动画结束后返回成功/失败结果。
    - 节点根据结果选择输出端口（0 为成功，1 为失败），实现剧情分流。

---

## 4. 存档与数据持久化

示例项目展示了如何利用 `FlowyGraphBlackboard` 实现一键存档：

- **全局变量**：所有的证据获取状态、好感度、剧情进度都存储在 Blackboard 的 `FloatValues` 和 `BranchValues` 中。
- **存档实现**：`SaveLoadManager` 调用 `FlowyGraphBlackboard.ExportValuesToJson()` 将所有状态序列化为 JSON 字符串并保存到本地。

---

## 5. 如何参考学习？

1. **查看场景结构**：打开 `FlowyGraphSample.unity`，观察 `Hierarchy` 中的 `UI` 根节点和 `Managers` 节点。
2. **阅读节点图**：在 `FlowyGraphAsset/` 中找到 `MenuFlow` 或 `IntroFlow`，观察它们是如何通过 `WaitSignal` 和 `Monologue` 构建逻辑的。
3. **研究自定义节点**：阅读 `Assets/FlowyGraphSample/Script/CustomNode/` 下的代码，这是学习如何为自己的项目编写扩展节点的最佳教材。
