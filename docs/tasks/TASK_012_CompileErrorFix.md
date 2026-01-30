# Task: 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ菫ｮ豁｣・・ASK_010, TASK_011髢｢騾｣・・
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-17T04:30:00+09:00
Report: docs/reports/REPORT_TASK_012_CompileErrorFix.md

## Objective
TASK_010 縺ｨ TASK_011 縺ｮ螳溯｣・ｾ後↓逋ｺ逕溘＠縺溘さ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺ｨ NullReferenceException 繧剃ｿｮ豁｣縺吶ｋ縲・
螳溯｣・ｯｾ雎｡・・1. **ScenarioManager.cs 縺ｮ菫ｮ豁｣**: `ChatController` 縺ｮ繝｡繧ｽ繝・ラ蜿ら・繧ｨ繝ｩ繝ｼ繧剃ｿｮ豁｣
2. **DebugSceneBuilder.cs 縺ｮ菫ｮ豁｣**: NullReferenceException 繧剃ｿｮ豁｣・・30陦檎岼縲・0陦檎岼・・
## Context
- TASK_010 (MetaEffectController) 縺ｨ TASK_011 (TopicScriptableObjects) 縺ｮ螳溯｣・ｾ後↓繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺檎匱逕・- 繧ｨ繝ｩ繝ｼ蜀・ｮｹ:
  - `CS1061`: `ChatController` 縺ｮ繝｡繧ｽ繝・ラ・・AddMessage`, `ShowTypingIndicator`・峨′隕九▽縺九ｉ縺ｪ縺・ｼ・cenarioManager.cs・・  - `NullReferenceException`: `DebugSceneBuilder.cs` 縺ｮ130陦檎岼縺ｨ90陦檎岼縺ｧ逋ｺ逕・- 蜿ら・繝峨く繝･繝｡繝ｳ繝・ `docs/inbox/REPORT_TASK_010_MetaEffectController.md`, `docs/inbox/REPORT_TASK_011_TopicScriptableObjects.md`

## Focus Area
- `Assets/Scripts/Core/ScenarioManager.cs` 縺ｮ菫ｮ豁｣
- `Assets/Scripts/Debug/Editor/DebugSceneBuilder.cs` 縺ｮ菫ｮ豁｣
- Unity 縺ｮ繧ｳ繝ｳ繝代う繝ｫ鬆・ｺ上・蝠城｡後ｒ蝗樣∩縺吶ｋ縺溘ａ縺ｮ螳悟・菫ｮ鬟ｾ蜷阪・菴ｿ逕ｨ
- Null 繝√ぉ繝・け縺ｮ霑ｽ蜉縺ｫ繧医ｋ NullReferenceException 縺ｮ髦ｲ豁｢

## Forbidden Area
- 譌｢蟄倥ヵ繧｡繧､繝ｫ縺ｮ蜑企勁繝ｻ遐ｴ螢顔噪螟画峩
- Unity繝励Ο繧ｸ繧ｧ繧ｯ繝郁ｨｭ螳壹・螟画峩
- 繝代ャ繧ｱ繝ｼ繧ｸ縺ｮ霑ｽ蜉繝ｻ蜑企勁
- 莉悶・繧ｹ繧ｯ繝ｪ繝励ヨ繝輔ぃ繧､繝ｫ縺ｮ螟画峩・井ｿｮ豁｣蟇ｾ雎｡莉･螟厄ｼ・
## Constraints
- 繝・せ繝・ 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺ｨ NullReferenceException 縺瑚ｧ｣豸医＆繧後ｋ縺薙→繧堤｢ｺ隱・- 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: 譌｢蟄倥・繧ｳ繝ｼ繝画ｧ矩繧堤ｶｭ謖√＠縲∵怙蟆城剞縺ｮ菫ｮ豁｣縺ｫ逡吶ａ繧・- Unity繝舌・繧ｸ繝ｧ繝ｳ: Unity 6 (or 2022 LTS) 縺ｫ蟇ｾ蠢・- 繧ｳ繝ｼ繝・ぅ繝ｳ繧ｰ隕冗ｴ・ Unity C# 繧ｳ繝ｼ繝・ぅ繝ｳ繧ｰ隕冗ｴ・↓貅匁侠

## DoD
- [x] ScenarioManager.cs 縺ｮ繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
  - [x] `ChatController` 縺ｮ蝙九ｒ螳悟・菫ｮ鬟ｾ蜷搾ｼ・ProjectFoundPhone.UI.ChatController`・峨〒譏守､ｺ逧・↓謖・ｮ・  - [x] `FindFirstObjectByType<ChatController>()` 縺ｮ蜻ｼ縺ｳ蜃ｺ縺励ｒ菫ｮ豁｣
- [x] DebugSceneBuilder.cs 縺ｮ NullReferenceException 縺瑚ｧ｣豸医＆繧後※縺・ｋ
  - [x] 130陦檎岼: `dialogueRunner` 縺ｨ `soRunner` 縺ｮ null 繝√ぉ繝・け繧定ｿｽ蜉
  - [x] 90陦檎岼: `chatController`縲～soChat`縲√☆縺ｹ縺ｦ縺ｮ `FindProperty()` 縺ｮ邨先棡縺ｮ null 繝√ぉ繝・け繧定ｿｽ蜉
  - [x] `soRunner.ApplyModifiedProperties()` 繧定ｿｽ蜉
- [x] 蜈ｨ縺ｦ縺ｮ繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- [x] 蜈ｨ縺ｦ縺ｮ NullReferenceException 縺瑚ｧ｣豸医＆繧後※縺・ｋ
- [x] 隴ｦ蜻翫′逋ｺ逕溘＠縺ｦ縺・↑縺・- [x] `docs/inbox/` 縺ｫ繝ｬ繝昴・繝茨ｼ・EPORT_TASK_012_CompileErrorFix.md・峨′菴懈・縺輔ｌ縺ｦ縺・ｋ
- [x] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## 螳溯｣・ｩｳ邏ｰ

### 1. ScenarioManager.cs 縺ｮ菫ｮ豁｣

#### 蝠城｡・```csharp
private ChatController m_ChatController;
m_ChatController = FindFirstObjectByType<ChatController>();
```
- 繧ｨ繝ｩ繝ｼ: `CS1061: 'ChatController' does not contain a definition for 'AddMessage'`
- 蜴溷屏: Unity 縺ｮ繧ｳ繝ｳ繝代う繝ｫ鬆・ｺ上・蝠城｡後↓繧医ｊ縲～ChatController` 繧ｯ繝ｩ繧ｹ縺梧ｭ｣縺励￥隗｣豎ｺ縺輔ｌ縺ｦ縺・↑縺・庄閭ｽ諤ｧ

#### 菫ｮ豁｣譁ｹ豕・`ChatController` 縺ｮ蝙九ｒ螳悟・菫ｮ鬟ｾ蜷搾ｼ・ProjectFoundPhone.UI.ChatController`・峨〒譏守､ｺ逧・↓謖・ｮ・
```csharp
private ProjectFoundPhone.UI.ChatController m_ChatController;
m_ChatController = FindFirstObjectByType<ProjectFoundPhone.UI.ChatController>();
```

#### 遒ｺ隱堺ｺ矩・- [x] 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- [x] 蝙句ｮ牙・諤ｧ縺御ｿ昴◆繧後※縺・ｋ

### 2. DebugSceneBuilder.cs 縺ｮ菫ｮ豁｣

#### 蝠城｡・: 130陦檎岼縺ｮ NullReferenceException
```csharp
SerializedObject soRunner = new SerializedObject(dialogueRunner);
soRunner.FindProperty("m_StartNode").stringValue = "Start";
```
- 繧ｨ繝ｩ繝ｼ: `NullReferenceException: Object reference not set to an instance of an object`
- 蜴溷屏: `dialogueRunner` 縺・null 縺ｮ蝣ｴ蜷医ｄ縲～SerializedObject` 縺ｮ菴懈・縺ｫ螟ｱ謨励＠縺溷ｴ蜷医・ null 繝√ぉ繝・け荳崎ｶｳ

#### 菫ｮ豁｣譁ｹ豕・
`dialogueRunner` 縺ｨ `soRunner` 縺ｮ null 繝√ぉ繝・け繧定ｿｽ蜉

```csharp
if (dialogueRunner == null)
{
    UnityEngine.Debug.LogError("DialogueRunner not found in scene.");
    return;
}

SerializedObject soRunner = new SerializedObject(dialogueRunner);
if (soRunner == null)
{
    UnityEngine.Debug.LogError("Failed to create SerializedObject for DialogueRunner.");
    return;
}

soRunner.FindProperty("m_StartNode").stringValue = "Start";
soRunner.ApplyModifiedProperties();
```

#### 蝠城｡・: 90陦檎岼縺ｮ NullReferenceException
```csharp
SerializedProperty scrollRectProp = soChat.FindProperty("m_ScrollRect");
scrollRectProp.objectReferenceValue = scrollRect;
```
- 繧ｨ繝ｩ繝ｼ: `NullReferenceException: Object reference not set to an instance of an object`
- 蜴溷屏: `FindProperty()` 縺・null 繧定ｿ斐＠縺溷ｴ蜷医↓縲√◎縺ｮ邨先棡縺ｫ蟇ｾ縺励※ `objectReferenceValue` 繧定ｨｭ螳壹＠繧医≧縺ｨ縺励※逋ｺ逕・
#### 菫ｮ豁｣譁ｹ豕・
縺吶∋縺ｦ縺ｮ `FindProperty()` 縺ｮ邨先棡繧・null 繝√ぉ繝・け

```csharp
if (chatController == null)
{
    UnityEngine.Debug.LogError("ChatController not found in scene.");
    return;
}

SerializedObject soChat = new SerializedObject(chatController);
if (soChat == null)
{
    UnityEngine.Debug.LogError("Failed to create SerializedObject for ChatController.");
    return;
}

SerializedProperty scrollRectProp = soChat.FindProperty("m_ScrollRect");
if (scrollRectProp != null)
{
    scrollRectProp.objectReferenceValue = scrollRect;
}
else
{
    UnityEngine.Debug.LogError("Failed to find property 'm_ScrollRect' in ChatController.");
}
```

#### 遒ｺ隱堺ｺ矩・- [x] NullReferenceException 縺瑚ｧ｣豸医＆繧後※縺・ｋ
- [x] 繧ｨ繝ｩ繝ｼ繝上Φ繝峨Μ繝ｳ繧ｰ縺碁←蛻・↓螳溯｣・＆繧後※縺・ｋ

## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- Unity 縺ｮ繧ｳ繝ｳ繝代う繝ｫ鬆・ｺ上・蝠城｡後ｒ蝗樣∩縺吶ｋ縺溘ａ縲∝ｮ悟・菫ｮ鬟ｾ蜷阪ｒ菴ｿ逕ｨ縺吶ｋ縺薙→縺碁㍾隕・- Null 繝√ぉ繝・け繧定ｿｽ蜉縺吶ｋ縺薙→縺ｧ縲∝ｮ溯｡梧凾繧ｨ繝ｩ繝ｼ繧帝亟豁｢縺ｧ縺阪ｋ
