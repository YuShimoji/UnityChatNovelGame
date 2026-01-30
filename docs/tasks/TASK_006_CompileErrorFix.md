# Task: 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ菫ｮ豁｣・・cenarioManager.cs・・Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T14:30:00Z
Report: docs/reports/REPORT_TASK_006_CompileErrorFix.md 

## Objective
ScenarioManager.cs縺ｮ繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ繧剃ｿｮ豁｣縺吶ｋ縲・arn Spinner縺ｮVariableStorage API縺ｮ豁｣縺励＞菴ｿ逕ｨ譁ｹ豕輔ｒ螳溯｣・＠縲・撼謗ｨ螂ｨAPI縺ｮ隴ｦ蜻翫ｒ隗｣豸医☆繧九・
螳溯｣・ｯｾ雎｡・・1. **TryGetValue繧ｨ繝ｩ繝ｼ菫ｮ豁｣**: 蝙句ｼ墓焚縺ｮ謗ｨ隲悶お繝ｩ繝ｼ繧剃ｿｮ豁｣
2. **SetValue繧ｨ繝ｩ繝ｼ菫ｮ豁｣**: 蠑墓焚縺ｮ蝙句､画鋤繧ｨ繝ｩ繝ｼ繧剃ｿｮ豁｣
3. **髱樊耳螂ｨAPI隴ｦ蜻願ｧ｣豸・*: FindObjectOfType繧巽indFirstObjectByType縺ｫ鄂ｮ縺肴鋤縺・4. **譛ｪ菴ｿ逕ｨ螟画焚隴ｦ蜻願ｧ｣豸・*: m_IsInputLocked縺ｮ菴ｿ逕ｨ縺ｾ縺溘・蜑企勁

## Context
- TASK_005螳御ｺ・ｾ後ヾcenarioManager.cs縺ｧ繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺檎匱逕・- 繧ｨ繝ｩ繝ｼ蜀・ｮｹ:
  - `CS0411`: TryGetValue縺ｮ蝙句ｼ墓焚縺梧耳隲悶〒縺阪↑縺・ｼ・87陦檎岼・・  - `CS1503`: SetValue縺ｮ蠑墓焚2繧痴tring縺ｫ螟画鋤縺ｧ縺阪↑縺・ｼ・22陦檎岼・・  - `CS0618`: FindObjectOfType縺碁撼謗ｨ螂ｨ・・0陦檎岼・・  - `CS0414`: m_IsInputLocked縺御ｽｿ逕ｨ縺輔ｌ縺ｦ縺・↑縺・ｼ・1陦檎岼・・- 蜿ら・繝峨く繝･繝｡繝ｳ繝・ `譛蛻昴・繝励Ο繝ｳ繝励ヨ`・医・繝ｭ繧ｸ繧ｧ繧ｯ繝医Ν繝ｼ繝茨ｼ峨～docs/inbox/REPORT_TASK_005_PackageInstallationFix.md`

## Focus Area
- `Assets/Scripts/Core/ScenarioManager.cs` 縺ｮ菫ｮ豁｣
- Yarn Spinner縺ｮVariableStorage API縺ｮ豁｣縺励＞菴ｿ逕ｨ譁ｹ豕輔・螳溯｣・- Unity 6縺ｮ髱樊耳螂ｨAPI縺ｮ鄂ｮ縺肴鋤縺・
## Forbidden Area
- 譌｢蟄倥ヵ繧｡繧､繝ｫ縺ｮ蜑企勁繝ｻ遐ｴ螢顔噪螟画峩
- Unity繝励Ο繧ｸ繧ｧ繧ｯ繝郁ｨｭ螳壹・螟画峩
- 繝代ャ繧ｱ繝ｼ繧ｸ縺ｮ霑ｽ蜉繝ｻ蜑企勁
- 莉悶・繧ｹ繧ｯ繝ｪ繝励ヨ繝輔ぃ繧､繝ｫ縺ｮ螟画峩

## Constraints
- 繝・せ繝・ 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後ｋ縺薙→繧堤｢ｺ隱・- 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: Yarn Spinner縺ｮVariableStorage API縺ｮ豁｣縺励＞繧ｷ繧ｰ繝阪メ繝｣繧堤｢ｺ隱阪＠縺ｦ縺九ｉ螳溯｣・- Unity繝舌・繧ｸ繝ｧ繝ｳ: Unity 6 (or 2022 LTS) 縺ｫ蟇ｾ蠢・- 繧ｳ繝ｼ繝・ぅ繝ｳ繧ｰ隕冗ｴ・ Unity C# 繧ｳ繝ｼ繝・ぅ繝ｳ繧ｰ隕冗ｴ・↓貅匁侠

## DoD
- [ ] TryGetValue繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
  - [ ] 蝙句ｼ墓焚繧呈・遉ｺ逧・↓謖・ｮ・  - [ ] 縺ｾ縺溘・縲∵ｭ｣縺励＞API繧ｷ繧ｰ繝阪メ繝｣繧剃ｽｿ逕ｨ
- [ ] SetValue繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
  - [ ] 蠑墓焚縺ｮ蝙九ｒ豁｣縺励￥謖・ｮ・  - [ ] 縺ｾ縺溘・縲∵ｭ｣縺励＞API繧ｷ繧ｰ繝阪メ繝｣繧剃ｽｿ逕ｨ
- [ ] FindObjectOfType隴ｦ蜻翫′隗｣豸医＆繧後※縺・ｋ
  - [ ] FindFirstObjectByType縺ｾ縺溘・FindAnyObjectByType縺ｫ鄂ｮ縺肴鋤縺・- [ ] m_IsInputLocked隴ｦ蜻翫′隗｣豸医＆繧後※縺・ｋ
  - [ ] 螟画焚繧剃ｽｿ逕ｨ縺吶ｋ縺九∝炎髯､縺吶ｋ縺九・pragma warning縺ｧ謚大宛
- [ ] 蜈ｨ縺ｦ縺ｮ繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- [ ] docs/inbox/ 縺ｫ繝ｬ繝昴・繝茨ｼ・EPORT_TASK_006_CompileErrorFix.md・峨′菴懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## 螳溯｣・ｩｳ邏ｰ

### 1. TryGetValue繧ｨ繝ｩ繝ｼ菫ｮ豁｣・・87陦檎岼・・
#### 蝠城｡・```csharp
if (m_DialogueRunner.VariableStorage.TryGetValue(variableName, out var value))
```
- 繧ｨ繝ｩ繝ｼ: `CS0411: The type arguments for method 'VariableStorageBehaviour.TryGetValue<T>(string, out T?)' cannot be inferred from the usage.`

#### 菫ｮ豁｣譁ｹ豕・蝙句ｼ墓焚繧呈・遉ｺ逧・↓謖・ｮ壹☆繧句ｿ・ｦ√′縺ゅｊ縺ｾ縺吶・arn Spinner縺ｮVariableStorage API繧堤｢ｺ隱阪＠縲∵ｭ｣縺励＞菴ｿ逕ｨ譁ｹ豕輔ｒ螳溯｣・＠縺ｾ縺吶・
**繧ｪ繝励す繝ｧ繝ｳ1**: 蝙句ｼ墓焚繧呈・遉ｺ逧・↓謖・ｮ・```csharp
if (m_DialogueRunner.VariableStorage.TryGetValue<T>(variableName, out var value))
```

**繧ｪ繝励す繝ｧ繝ｳ2**: 豁｣縺励＞API繧ｷ繧ｰ繝阪メ繝｣繧剃ｽｿ逕ｨ・・arn Spinner縺ｮ繝舌・繧ｸ繝ｧ繝ｳ縺ｫ繧医▲縺ｦ逡ｰ縺ｪ繧句庄閭ｽ諤ｧ・・```csharp
// Yarn Spinner縺ｮVariableStorage API繧堤｢ｺ隱阪＠縺ｦ縺九ｉ螳溯｣・// 萓・ TryGetValue<T>(string name, out T value) 縺ｮ蝣ｴ蜷・```

#### 遒ｺ隱堺ｺ矩・- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- 蝙句ｮ牙・諤ｧ縺御ｿ昴◆繧後※縺・ｋ

### 2. SetValue繧ｨ繝ｩ繝ｼ菫ｮ豁｣・・22陦檎岼・・
#### 蝠城｡・```csharp
m_DialogueRunner.VariableStorage.SetValue(variableName, value);
```
- 繧ｨ繝ｩ繝ｼ: `CS1503: Argument 2: cannot convert from 'T' to 'string'`

#### 菫ｮ豁｣譁ｹ豕・Yarn Spinner縺ｮVariableStorage API繧堤｢ｺ隱阪＠縲∵ｭ｣縺励＞繧ｷ繧ｰ繝阪メ繝｣繧剃ｽｿ逕ｨ縺励∪縺吶・
**繧ｪ繝励す繝ｧ繝ｳ1**: SetValue縺ｮ豁｣縺励＞繧ｷ繧ｰ繝阪メ繝｣繧剃ｽｿ逕ｨ
```csharp
// Yarn Spinner縺ｮVariableStorage API繧堤｢ｺ隱阪＠縺ｦ縺九ｉ螳溯｣・// 萓・ SetValue(string name, object value) 縺ｮ蝣ｴ蜷・m_DialogueRunner.VariableStorage.SetValue(variableName, (object)value);
```

**繧ｪ繝励す繝ｧ繝ｳ2**: 蝙九↓蠢懊§縺滄←蛻・↑繝｡繧ｽ繝・ラ繧剃ｽｿ逕ｨ
```csharp
// 萓・ SetValue<T>(string name, T value) 縺ｮ蝣ｴ蜷・m_DialogueRunner.VariableStorage.SetValue<T>(variableName, value);
```

#### 遒ｺ隱堺ｺ矩・- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- 蝙句ｮ牙・諤ｧ縺御ｿ昴◆繧後※縺・ｋ

### 3. FindObjectOfType隴ｦ蜻願ｧ｣豸茨ｼ・0陦檎岼・・
#### 蝠城｡・```csharp
m_DialogueRunner = FindObjectOfType<DialogueRunner>();
```
- 隴ｦ蜻・ `CS0618: 'Object.FindObjectOfType<T>()' is obsolete: 'Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType'`

#### 菫ｮ豁｣譁ｹ豕・Unity 6縺ｮ譁ｰ縺励＞API縺ｫ鄂ｮ縺肴鋤縺医∪縺吶・
**謗ｨ螂ｨ**: FindFirstObjectByType繧剃ｽｿ逕ｨ・域怙蛻昴↓隕九▽縺九▲縺溘う繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ繧貞叙蠕暦ｼ・```csharp
m_DialogueRunner = FindFirstObjectByType<DialogueRunner>();
```

**莉｣譖ｿ**: FindAnyObjectByType繧剃ｽｿ逕ｨ・医ヱ繝輔か繝ｼ繝槭Φ繧ｹ驥崎ｦ悶・蝣ｴ蜷茨ｼ・```csharp
m_DialogueRunner = FindAnyObjectByType<DialogueRunner>();
```

#### 遒ｺ隱堺ｺ矩・- 隴ｦ蜻翫′隗｣豸医＆繧後※縺・ｋ
- 蜍穂ｽ懊′豁｣縺励￥菫昴◆繧後※縺・ｋ

### 4. m_IsInputLocked隴ｦ蜻願ｧ｣豸茨ｼ・1陦檎岼・・
#### 蝠城｡・```csharp
private bool m_IsInputLocked = false;
```
- 隴ｦ蜻・ `CS0414: The field 'ScenarioManager.m_IsInputLocked' is assigned but its value is never used`

#### 菫ｮ豁｣譁ｹ豕・螟画焚繧剃ｽｿ逕ｨ縺吶ｋ縺九∝炎髯､縺吶ｋ縺九・pragma warning縺ｧ謚大宛縺励∪縺吶・
**繧ｪ繝励す繝ｧ繝ｳ1**: 螟画焚繧剃ｽｿ逕ｨ縺吶ｋ・亥ｰ・擂縺ｮ螳溯｣・畑縺ｨ縺励※菫晄戟・・```csharp
// StartWaitCommand縺ｪ縺ｩ縺ｧ菴ｿ逕ｨ縺吶ｋ
// 縺ｾ縺溘・縲・pragma warning disable CS0414 縺ｧ謚大宛
```

**繧ｪ繝励す繝ｧ繝ｳ2**: 螟画焚繧貞炎髯､・井ｽｿ逕ｨ莠亥ｮ壹′縺ｪ縺・ｴ蜷茨ｼ・```csharp
// 螟画焚繧貞炎髯､
```

**繧ｪ繝励す繝ｧ繝ｳ3**: #pragma warning縺ｧ謚大宛・亥ｰ・擂縺ｮ螳溯｣・畑縺ｨ縺励※菫晄戟縺吶ｋ蝣ｴ蜷茨ｼ・```csharp
#pragma warning disable CS0414
private bool m_IsInputLocked = false;
#pragma warning restore CS0414
```

#### 遒ｺ隱堺ｺ矩・- 隴ｦ蜻翫′隗｣豸医＆繧後※縺・ｋ
- 繧ｳ繝ｼ繝峨・諢丞峙縺梧・遒ｺ縺ｫ縺ｪ縺｣縺ｦ縺・ｋ

## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- Yarn Spinner縺ｮVariableStorage API縺ｯ繝舌・繧ｸ繝ｧ繝ｳ縺ｫ繧医▲縺ｦ逡ｰ縺ｪ繧句庄閭ｽ諤ｧ縺後≠繧九◆繧√∝ｮ滄圀縺ｮAPI繧堤｢ｺ隱阪＠縺ｦ縺九ｉ螳溯｣・☆繧句ｿ・ｦ√′縺ゅｋ
- Unity 6縺ｮ髱樊耳螂ｨAPI縺ｯ縲∵眠縺励＞API縺ｫ鄂ｮ縺肴鋤縺医ｋ蠢・ｦ√′縺ゅｋ
