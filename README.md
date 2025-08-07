# MR三消游戏 - Unity项目

## 项目概述

这是一个基于Unity 6的MR（混合现实）三消游戏项目，使用状态机驱动的架构设计，支持XR交互和手势控制。

## 技术栈

- **Unity版本**: Unity 6000.0.54f1
- **渲染管线**: URP (Universal Render Pipeline)
- **平台目标**: Android APK - Meta Quest 3
- **XR SDK**: 
  - 底层插件: OpenXR
  - 高层SDK: XR Interaction Toolkit (XRI)
- **输入系统**: Unity Input System
- **架构模式**: 状态机 + 事件驱动 + ScriptableObject配置

## 项目结构

```
Assets/Scripts/
├── Core/                    # 核心系统
│   ├── Data/               # 数据模型和配置
│   │   ├── GameConfig.cs   # 游戏配置ScriptableObject
│   │   ├── ScoreConfig.cs  # 得分配置ScriptableObject
│   │   ├── ShapeData.cs    # 几何体数据定义
│   │   └── GameSessionData.cs # 游戏会话数据
│   ├── Events/             # 事件系统
│   │   ├── EventBus.cs     # 事件总线
│   │   └── GameStateChangedEvent.cs # 游戏状态变化事件
│   ├── GameManager.cs      # 游戏管理器（状态机）
│   ├── GameState.cs        # 游戏状态枚举
│   └── SceneBootstrapper.cs # 场景初始化器
├── UI/                     # 用户界面
│   ├── UIManager.cs        # UI管理器
│   └── LoadingUI.cs        # 加载界面组件
└── Test/                   # 测试脚本
    └── TestSceneSetup.cs   # 基础架构测试
```

## 核心架构

### 1. 游戏状态机 (GameManager)

游戏支持以下5个核心状态：

- **Initializing**: 初始化状态，显示加载界面
- **Ready**: 准备状态，显示主菜单
- **Playing**: 游戏进行状态
- **Paused**: 暂停状态
- **GameOver**: 游戏结束状态

### 2. 事件系统 (EventBus)

使用单例模式的事件总线实现模块间松耦合通信：

```csharp
// 订阅事件
EventBus.Instance.Subscribe<GameStateChangedEvent>(OnGameStateChanged);

// 发布事件
EventBus.Instance.Publish(new GameStateChangedEvent(oldState, newState));

// 取消订阅
EventBus.Instance.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
```

### 3. 配置系统 (ScriptableObject)

使用ScriptableObject实现数据驱动配置：

- **GameConfig**: 游戏核心配置（时间、生成规则、物理参数等）
- **ScoreConfig**: 得分系统配置（基础得分、连击、奖励等）
- **ShapeConfig**: 几何体配置（类型、颜色、材质等）

### 4. UI管理系统 (UIManager)

统一管理所有UI界面的显示和状态切换，自动响应游戏状态变化。

## 开发规范

### 禁用项

- ❌ 混用OpenXR和MetaSDK
- ❌ GameObject.Find() / FindObjectOfType()
- ❌ 非封装的Coroutine跑流程逻辑
- ❌ MonoBehaviour中过度依赖生命周期函数
- ❌ 主线程异步AssetBundle加载
- ❌ HDRP渲染管线 / 非URP Shader
- ❌ 无封装的魔法数字或硬编码路径

### 推荐模式

- ✅ 高内聚、低耦合设计
- ✅ 接口 + 依赖注入
- ✅ 数据驱动配置
- ✅ 状态机控制流程
- ✅ 事件总线通信

## 快速开始

### 1. 环境要求

- Unity 6000.0.54f1 或更高版本
- 已安装URP和XR Interaction Toolkit包
- 支持OpenXR的VR设备（推荐Meta Quest 3）

### 2. 项目设置

1. 在Unity中打开项目
2. 确保项目设置为Android平台
3. 配置XR设置启用OpenXR
4. 导入XR Interaction Toolkit示例（可选）

### 3. 测试基础架构

1. 创建一个新场景
2. 添加以下组件到场景中的GameObject：
   - `SceneBootstrapper`
   - `GameManager`
   - `UIManager`
   - `TestSceneSetup`（用于测试）

3. 运行场景，观察控制台输出和测试界面

### 4. 手动测试快捷键

在运行时可以使用以下快捷键测试状态切换：

- **1**: 切换到Initializing状态
- **2**: 切换到Ready状态
- **3**: 开始游戏（Playing状态）
- **4**: 暂停/继续游戏
- **5**: 结束游戏（GameOver状态）
- **R**: 重启自动测试

## 下一步开发计划

1. **输入系统**: 实现XR手势控制和交互
2. **几何体系统**: 实现3D几何体生成和物理交互
3. **匹配系统**: 实现三消匹配逻辑和检测
4. **得分系统**: 实现连击、奖励和统计
5. **音效系统**: 添加音效和反馈
6. **优化和测试**: 性能优化和完整测试

## 注意事项

- 所有脚本都包含详细的函数级注释
- 遵循Unity C#编码规范
- 使用命名空间避免冲突
- 所有公共接口都有完整的XML文档注释
- 错误处理和调试日志完整

## 联系信息

如有问题或建议，请查看代码注释或联系开发团队。