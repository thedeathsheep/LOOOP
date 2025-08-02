# LOOOP 开始界面系统

## 概述
Unity开始界面系统，包含主菜单、设置、制作人员等功能。

## 脚本文件说明

### 1. MainMenuManager.cs
- **功能**: 主菜单管理器
- **主要功能**:
  - 开始游戏（清除存档并进入第一关）
  - 继续游戏（加载存档）
  - 设置（显示设置面板）
  - 制作人员（显示制作人员面板）
  - 退出游戏

### 2. SettingsManager.cs
- **功能**: 设置管理器
- **主要功能**:
  - 主音量控制
  - 音乐音量控制
  - 音效音量控制
  - 分辨率设置
  - 全屏切换
  - 设置保存和加载

### 3. CreditsManager.cs
- **功能**: 制作人员管理器
- **主要功能**:
  - 显示制作人员信息
  - 可滚动的制作人员列表
  - 返回主菜单

### 4. GameManager.cs
- **功能**: 游戏管理器（单例）
- **主要功能**:
  - 游戏进度保存和加载
  - 场景切换管理
  - 游戏设置加载

### 5. AudioManager.cs
- **功能**: 音频管理器（单例）
- **主要功能**:
  - 音频混合器控制
  - 音乐和音效播放
  - 音量控制

### 6. UIManager.cs
- **功能**: UI管理器
- **主要功能**:
  - Canvas缩放设置
  - 按钮动画效果
  - 面板显示控制

### 7. SceneLoader.cs
- **功能**: 场景加载器（单例）
- **主要功能**:
  - 异步场景加载
  - 加载进度显示
  - 加载动画

## 设置步骤

### 1. 创建主菜单场景
1. 创建新场景，命名为 "MainMenu"
2. 设置场景为启动场景

### 2. 设置UI层级结构
```
Canvas (Canvas)
├── MainMenuPanel (GameObject)
│   ├── BackgroundImage (Image) - 主题图
│   ├── TitleText (Text) - 游戏标题
│   ├── StartGameButton (Button)
│   ├── ContinueGameButton (Button)
│   ├── SettingsButton (Button)
│   ├── CreditsButton (Button)
│   └── ExitButton (Button)
├── SettingsPanel (GameObject)
│   ├── TitleText (Text) - "设置"
│   ├── MasterVolumeSlider (Slider)
│   ├── MusicVolumeSlider (Slider)
│   ├── SFXVolumeSlider (Slider)
│   ├── ResolutionDropdown (Dropdown)
│   ├── FullscreenToggle (Toggle)
│   ├── SaveButton (Button)
│   └── BackButton (Button)
├── CreditsPanel (GameObject)
│   ├── TitleText (Text) - "制作人员"
│   ├── CreditsScrollView (ScrollView)
│   │   └── CreditsText (Text)
│   └── BackButton (Button)
└── LoadingPanel (GameObject)
    ├── ProgressBar (Slider)
    ├── ProgressText (Text)
    └── LoadingText (Text)
```

### 3. 添加脚本组件
1. 在Canvas上添加 `MainMenuManager` 脚本
2. 在Canvas上添加 `UIManager` 脚本
3. 在SettingsPanel上添加 `SettingsManager` 脚本
4. 在CreditsPanel上添加 `CreditsManager` 脚本

### 4. 设置音频系统
1. 创建AudioMixer资源
2. 在AudioMixer中创建三个参数：
   - MasterVolume
   - MusicVolume
   - SFXVolume
3. 创建AudioManager GameObject并添加 `AudioManager` 脚本
4. 设置AudioMixer引用

### 5. 设置管理器
1. 创建GameManager GameObject并添加 `GameManager` 脚本
2. 创建SceneLoader GameObject并添加 `SceneLoader` 脚本

### 6. 配置按钮引用
在MainMenuManager中设置所有按钮的引用：
- startGameButton
- continueGameButton
- settingsButton
- creditsButton
- exitButton
- mainMenuPanel
- settingsPanel
- creditsPanel
- backgroundImage

### 7. 配置设置面板
在SettingsManager中设置所有UI元素的引用：
- audioMixer
- masterVolumeSlider
- musicVolumeSlider
- sfxVolumeSlider
- resolutionDropdown
- fullscreenToggle
- saveButton
- backButton

### 8. 配置制作人员面板
在CreditsManager中设置：
- backButton
- creditsScrollRect
- creditsText

## 使用说明

### 主菜单功能
- **开始游戏**: 清除存档并加载第一关
- **继续游戏**: 加载上次保存的游戏进度
- **设置**: 打开设置面板
- **制作人员**: 打开制作人员面板
- **退出**: 退出游戏

### 设置功能
- **音量控制**: 三个滑块分别控制主音量、音乐音量、音效音量
- **分辨率**: 下拉菜单选择分辨率
- **全屏**: 切换全屏模式
- **保存**: 保存当前设置
- **返回**: 返回主菜单

### 制作人员功能
- **滚动查看**: 可滚动的制作人员信息
- **返回**: 返回主菜单

## 注意事项

1. **场景名称**: 确保场景名称与脚本中的名称一致
2. **音频混合器**: 必须正确设置AudioMixer参数
3. **按钮引用**: 所有按钮引用必须正确设置
4. **存档系统**: 使用PlayerPrefs进行设置和进度保存
5. **分辨率**: 分辨率设置保持接口，实际功能需要根据项目需求实现

## 扩展功能

1. **多语言支持**: 可以添加本地化系统
2. **更多设置选项**: 可以添加图形质量、控制设置等
3. **存档管理**: 可以添加多个存档槽位
4. **成就系统**: 可以添加成就显示
5. **DLC支持**: 可以添加DLC内容管理 