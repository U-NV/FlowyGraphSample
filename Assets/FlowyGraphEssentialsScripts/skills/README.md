# FlowyGraph AI 开发辅助文件

本目录下的文件是 FlowyGraph 为 **AI 编程助手**准备的上下文知识文件，帮助 AI 更准确地理解 FlowyGraph 的开发规范，从而在你创建自定义节点、编写编辑器扩展时给出正确的代码建议。

---

## 文件说明

```
skills/
├── README.md                          ← 本文件
└── flowygraph-custom-node/
    └── SKILL.md                       ← 核心知识文件
```

`flowygraph-custom-node/SKILL.md` 涵盖：
- 自定义节点的创建方式（继承 BaseNodeData、Attributes、生命周期）
- Blackboard 与外部系统交互模式
- 信号系统的使用
- 编辑器扩展接口（INodeGraphExtension、INodeGraphToolTab）
- 编辑器界面适配（层级视图、错误检查、节点颜色）
- 本地化支持（NodeTextData）

---

## 使用方式

### Cursor 用户

将 `flowygraph-custom-node/` 文件夹**复制**到项目根目录（与 `Assets/` 同级）的 `.cursor/skills/` 目录下：

```
你的游戏项目/
├── Assets/
│   └── FlowyGraphEssentialsScripts/
│       └── skills/            ← 当前位置（Unity 可见）
└── .cursor/
    └── skills/
        └── flowygraph-custom-node/
            └── SKILL.md       ← 复制到这里，Cursor 会自动加载
```

完成后，Cursor 会在与 FlowyGraph 相关的对话中自动引用此文件，无需每次手动添加。

> 也可以不移动文件，在对话时直接将 `SKILL.md` 拖入对话窗口作为一次性上下文使用。

### 其他 AI 编辑器用户

**GitHub Copilot（VSCode）：**
将 `SKILL.md` 的内容添加到项目的 `.github/copilot-instructions.md` 文件中，或在对话时将文件直接添加为上下文。

**通义灵码、Windsurf 等：**
参考对应编辑器的"项目级 AI 上下文"或"自定义指令"功能，将 `SKILL.md` 的内容粘贴进去；或在需要时将文件直接拖入对话。

**通用方式（任何支持上下文的 AI 工具）：**
在开始开发自定义节点前，将 `SKILL.md` 的全部内容粘贴到对话的系统提示或首条消息中。

---

## 更新说明

FlowyGraph 插件更新后，此 `skills/` 目录下的文件**不会自动覆盖**已存在的版本。如需获取最新内容，请手动删除 `skills/` 目录下的旧文件，然后通过菜单 `工具 > FlowyGraph > 生成必要文件到项目根目录` 重新执行初始化。
