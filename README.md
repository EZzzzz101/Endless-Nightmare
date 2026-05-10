# Endless Nightmare

Unity 俯视角生存射击游戏 Demo

## 核心系统
- 对话系统：ScriptableObject 对话树 + 条件分支 + 打字机效果
- 任务系统：收集/击杀/升级三种类型，对话接取 → 追踪 → 提交完整闭环
- 存档系统：JSON 多槽位序列化，支持全状态读写
- FSM ：基于接口的状态机，3-4 状态 + 联动机制
- 导表工具：Excel → ScriptableObject 自动生成

## Demo 视频
[[链接]](https://www.bilibili.com/video/BV1orD3BDEMu)

## 项目结构
Assets/Scripts/
├── Enemy/     — 敌人 AI、FSM
├── Managers/  — 核心系统
├── UI/        — 背包、对话、成就面板
├── Player/    — 射击、移动、技能
└── System/    — 存档、导表、对象池
