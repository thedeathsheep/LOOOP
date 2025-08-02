# LOOOP 项目脚本文件结构

## 概述
本项目采用模块化的文件组织结构，将不同类型的脚本文件分类存放在相应的文件夹中，便于管理和维护。

## 文件夹结构

### 📁 UI/
**用户界面相关脚本**
- `MainMenuManager.cs` - 主菜单管理器
- `SettingsManager.cs` - 设置管理器
- `CreditsManager.cs` - 制作人员管理器
- `UIManager.cs` - UI管理器
- `UISetupHelper.cs` - UI设置助手

### 📁 Managers/
**核心管理器脚本**
- `GameManager.cs` - 游戏管理器（单例）
- `SceneLoader.cs` - 场景加载器（单例）

### 📁 Audio/
**音频系统脚本**
- `AudioManager.cs` - 音频管理器（单例）

### 📁 Camera/
**相机系统脚本**
- `CameraSettings.cs` - 相机设置
- `InitializeActiveCamera.cs` - 相机初始化

### 📁 Utils/
**工具类和辅助脚本**
- `DisableSpriteRenderer.cs` - 精灵渲染器禁用工具
- `StopObject.cs` - 对象停止工具
- `ScriptableSingletonFix.cs` - ScriptableObject单例修复
- `SpawnpointInitialization.cs` - 出生点初始化

### 📁 Player/
**玩家相关脚本**
- 玩家控制器
- 玩家状态管理
- 玩家输入处理

### 📁 Documentation/
**文档文件**
- `README_MainMenu.md` - 主菜单系统使用说明

## 文件分类原则

### 1. 按功能分类
- **UI**: 所有用户界面相关的脚本
- **Managers**: 核心管理器和系统控制器
- **Audio**: 音频系统和音效管理
- **Camera**: 相机控制和设置
- **Utils**: 通用工具类和辅助功能
- **Player**: 玩家相关的所有功能

### 2. 按职责分离
- 每个文件夹都有明确的职责范围
- 避免跨文件夹的强依赖关系
- 保持模块间的低耦合

### 3. 便于维护
- 相关功能集中管理
- 快速定位问题代码
- 便于团队协作开发

## 命名规范

### 文件命名
- 使用PascalCase命名法
- 文件名应清晰表达功能
- 管理器类以"Manager"结尾
- 工具类以功能描述命名

### 类命名
- 使用PascalCase命名法
- 单例类以"Manager"结尾
- 组件类以功能描述命名

## 扩展建议

### 新增文件夹
- **AI/**: 人工智能相关脚本
- **Combat/**: 战斗系统脚本
- **Inventory/**: 物品栏系统
- **Quest/**: 任务系统
- **Effects/**: 特效系统

### 最佳实践
1. 新增脚本时先确定其功能类别
2. 保持文件夹结构的清晰性
3. 定期整理和重构文件结构
4. 及时更新文档说明

## 注意事项

1. **Unity Meta文件**: 每个.cs文件都有对应的.meta文件，移动文件时需要同时移动.meta文件
2. **命名空间**: 建议为每个文件夹创建对应的命名空间
3. **依赖关系**: 注意脚本间的依赖关系，避免循环依赖
4. **版本控制**: 文件结构变更时及时提交到版本控制系统 