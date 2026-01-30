# Worker Prompt: TASK_006_CompileErrorFix

## 蜿ら・
- 繝√こ繝・ヨ: docs/tasks/TASK_006_CompileErrorFix.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- 蜑阪ち繧ｹ繧ｯ繝ｬ繝昴・繝・ docs/inbox/REPORT_TASK_005_PackageInstallationFix.md
- 繝励Ο繧ｸ繧ｧ繧ｯ繝井ｻ墓ｧ・ 譛蛻昴・繝励Ο繝ｳ繝励ヨ・医・繝ｭ繧ｸ繧ｧ繧ｯ繝医Ν繝ｼ繝茨ｼ・
## 蠅・阜

### Focus Area
- `Assets/Scripts/Core/ScenarioManager.cs` 縺ｮ菫ｮ豁｣
- Yarn Spinner縺ｮVariableStorage API縺ｮ豁｣縺励＞菴ｿ逕ｨ譁ｹ豕輔・螳溯｣・- Unity 6縺ｮ髱樊耳螂ｨAPI縺ｮ鄂ｮ縺肴鋤縺・
### Forbidden Area
- 譌｢蟄倥ヵ繧｡繧､繝ｫ縺ｮ蜑企勁繝ｻ遐ｴ螢顔噪螟画峩
- Unity繝励Ο繧ｸ繧ｧ繧ｯ繝郁ｨｭ螳壹・螟画峩
- 繝代ャ繧ｱ繝ｼ繧ｸ縺ｮ霑ｽ蜉繝ｻ蜑企勁
- 莉悶・繧ｹ繧ｯ繝ｪ繝励ヨ繝輔ぃ繧､繝ｫ縺ｮ螟画峩

## Tier / Branch
- Tier: 2・域ｩ溯・螳溯｣・ｼ・- Branch: main

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

## 蛛懈ｭ｢譚｡莉ｶ
- Forbidden Area 縺ｫ隗ｦ繧後↑縺・→螳碁≠縺ｧ縺阪↑縺・- 莉墓ｧ倥・莉ｮ螳壹′ 3 縺､莉･荳雁ｿ・ｦ・- 萓晏ｭ倩ｿｽ蜉/譖ｴ譁ｰ縲∫ｴ螢顔噪Git謫堺ｽ懊；itHubAutoApprove荳肴・縺ｧ縺ｮ push 縺悟ｿ・ｦ・- SSOT荳崎ｶｳ繧・`ensure-ssot.js` 縺ｧ隗｣豎ｺ縺ｧ縺阪↑縺・- 髟ｷ譎る俣蠕・ｩ溘′蠢・ｦ・ｼ亥ｮ夂ｾｩ縺励◆繧ｿ繧､繝繧｢繧ｦ繝郁ｶ・℃・・- Yarn Spinner縺ｮVariableStorage API縺檎｢ｺ隱阪〒縺阪↑縺・ｴ蜷茨ｼ医ラ繧ｭ繝･繝｡繝ｳ繝医′荳崎ｶｳ縺励※縺・ｋ蝣ｴ蜷茨ｼ・
蛛懈ｭ｢譎ゅ・莉･荳九ｒ螳滓命・・1. 繝√こ繝・ヨ縺ｮStatus繧達LOCKED縺ｫ譖ｴ譁ｰ
2. 莠句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ繝√こ繝・ヨ譛ｬ譁・↓霑ｽ險・3. docs/inbox/REPORT_TASK_006_CompileErrorFix.md 繧剃ｽ懈・縺励∝●豁｢逅・罰繧定ｨ倬鹸
4. 繝√こ繝・ヨ縺ｮReport谺・↓繝ｬ繝昴・繝医ヱ繧ｹ繧定ｿｽ險・
## 邏榊刀蜈・- docs/inbox/REPORT_TASK_006_CompileErrorFix.md

## 螳溯｣・ｩｳ邏ｰ

### 1. TryGetValue繧ｨ繝ｩ繝ｼ菫ｮ豁｣・・87陦檎岼・・
#### 蝠城｡・```csharp
if (m_DialogueRunner.VariableStorage.TryGetValue(variableName, out var value))
```
- 繧ｨ繝ｩ繝ｼ: `CS0411: The type arguments for method 'VariableStorageBehaviour.TryGetValue<T>(string, out T?)' cannot be inferred from the usage.`

#### 菫ｮ豁｣譁ｹ豕・蝙句ｼ墓焚繧呈・遉ｺ逧・↓謖・ｮ壹☆繧句ｿ・ｦ√′縺ゅｊ縺ｾ縺吶・
**謗ｨ螂ｨ**: 蝙句ｼ墓焚繧呈・遉ｺ逧・↓謖・ｮ・```csharp
// object蝙九〒蜿門ｾ励＠縺ｦ縺九ｉ繧ｭ繝｣繧ｹ繝・if (m_DialogueRunner.VariableStorage.TryGetValue<object>(variableName, out var value))
{
    // value is T typedValue 縺ｮ繝代ち繝ｼ繝ｳ縺ｧ繧ｭ繝｣繧ｹ繝・    if (value is T typedValue)
    {
        return typedValue;
    }
}
```

**莉｣譖ｿ**: 豁｣縺励＞API繧ｷ繧ｰ繝阪メ繝｣繧堤｢ｺ隱阪＠縺ｦ縺九ｉ螳溯｣・- Yarn Spinner縺ｮVariableStorage API縺ｯ縲・壼ｸｸ `TryGetValue<T>(string name, out T value)` 縺ｮ蠖｢蠑・- 蝙句ｼ墓焚 `<T>` 繧呈・遉ｺ逧・↓謖・ｮ壹☆繧句ｿ・ｦ√′縺ゅｋ

#### 遒ｺ隱堺ｺ矩・- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- 蝙句ｮ牙・諤ｧ縺御ｿ昴◆繧後※縺・ｋ

### 2. SetValue繧ｨ繝ｩ繝ｼ菫ｮ豁｣・・22陦檎岼・・
#### 蝠城｡・```csharp
m_DialogueRunner.VariableStorage.SetValue(variableName, value);
```
- 繧ｨ繝ｩ繝ｼ: `CS1503: Argument 2: cannot convert from 'T' to 'string'`

#### 菫ｮ豁｣譁ｹ豕・Yarn Spinner縺ｮVariableStorage API繧堤｢ｺ隱阪＠縲∵ｭ｣縺励＞繧ｷ繧ｰ繝阪メ繝｣繧剃ｽｿ逕ｨ縺励∪縺吶・
**謗ｨ螂ｨ**: object蝙九↓繧ｭ繝｣繧ｹ繝医＠縺ｦ縺九ｉ險ｭ螳・```csharp
// SetValue縺ｯ騾壼ｸｸ縲｛bject蝙九ｒ蜿励￠蜿悶ｋ
m_DialogueRunner.VariableStorage.SetValue(variableName, (object)value);
```

**莉｣譖ｿ**: 蝙九↓蠢懊§縺滄←蛻・↑繝｡繧ｽ繝・ラ繧剃ｽｿ逕ｨ
- Yarn Spinner縺ｮVariableStorage API縺ｯ縲・壼ｸｸ `SetValue(string name, object value)` 縺ｮ蠖｢蠑・- 繧ｸ繧ｧ繝阪Μ繝・け蝙・`T` 繧・`object` 縺ｫ繧ｭ繝｣繧ｹ繝医☆繧句ｿ・ｦ√′縺ゅｋ

#### 遒ｺ隱堺ｺ矩・- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- 蝙句ｮ牙・諤ｧ縺御ｿ昴◆繧後※縺・ｋ

### 3. FindObjectOfType隴ｦ蜻願ｧ｣豸茨ｼ・0陦檎岼・・
#### 蝠城｡・```csharp
m_ChatController = FindObjectOfType<ChatController>();
```
- 隴ｦ蜻・ `CS0618: 'Object.FindObjectOfType<T>()' is obsolete`

#### 菫ｮ豁｣譁ｹ豕・Unity 6縺ｮ譁ｰ縺励＞API縺ｫ鄂ｮ縺肴鋤縺医∪縺吶・
**謗ｨ螂ｨ**: FindFirstObjectByType繧剃ｽｿ逕ｨ
```csharp
m_ChatController = FindFirstObjectByType<ChatController>();
```

**莉｣譖ｿ**: FindAnyObjectByType繧剃ｽｿ逕ｨ・医ヱ繝輔か繝ｼ繝槭Φ繧ｹ驥崎ｦ悶・蝣ｴ蜷茨ｼ・```csharp
m_ChatController = FindAnyObjectByType<ChatController>();
```

#### 遒ｺ隱堺ｺ矩・- 隴ｦ蜻翫′隗｣豸医＆繧後※縺・ｋ
- 蜍穂ｽ懊′豁｣縺励￥菫昴◆繧後※縺・ｋ

### 4. m_IsInputLocked隴ｦ蜻願ｧ｣豸茨ｼ・1陦檎岼・・
#### 蝠城｡・```csharp
private bool m_IsInputLocked = false;
```
- 隴ｦ蜻・ `CS0414: The field 'ScenarioManager.m_IsInputLocked' is assigned but its value is never used`

#### 菫ｮ豁｣譁ｹ豕・螟画焚繧剃ｽｿ逕ｨ縺吶ｋ縺九∝炎髯､縺吶ｋ縺九・pragma warning縺ｧ謚大宛縺励∪縺吶・
**謗ｨ螂ｨ**: #pragma warning縺ｧ謚大宛・亥ｰ・擂縺ｮ螳溯｣・畑縺ｨ縺励※菫晄戟・・```csharp
#pragma warning disable CS0414
private bool m_IsInputLocked = false;
#pragma warning restore CS0414
```

**莉｣譖ｿ1**: 螟画焚繧剃ｽｿ逕ｨ縺吶ｋ・・tartWaitCommand縺ｪ縺ｩ縺ｧ菴ｿ逕ｨ・・```csharp
// StartWaitCommand()縺ｧ菴ｿ逕ｨ
private void StartWaitCommand(string[] args)
{
    m_IsInputLocked = true;
    // ... 蠕・ｩ溷・逅・...
    m_IsInputLocked = false;
}
```

**莉｣譖ｿ2**: 螟画焚繧貞炎髯､・井ｽｿ逕ｨ莠亥ｮ壹′縺ｪ縺・ｴ蜷茨ｼ・```csharp
// 螟画焚繧貞炎髯､
```

#### 遒ｺ隱堺ｺ矩・- 隴ｦ蜻翫′隗｣豸医＆繧後※縺・ｋ
- 繧ｳ繝ｼ繝峨・諢丞峙縺梧・遒ｺ縺ｫ縺ｪ縺｣縺ｦ縺・ｋ

## 繧ｳ繝ｼ繝・ぅ繝ｳ繧ｰ隕冗ｴ・- Unity C# 繧ｳ繝ｼ繝・ぅ繝ｳ繧ｰ隕冗ｴ・↓貅匁侠
- 螟画焚蜷・ `m_VariableName` (private field)
- 繧ｯ繝ｩ繧ｹ/繝｡繧ｽ繝・ラ: PascalCase
- `#region`繧剃ｽｿ逕ｨ縺励※繧ｳ繝ｼ繝峨ｒ謨ｴ逅・
## 蜿り・ュ蝣ｱ
- 蜑阪ち繧ｹ繧ｯ繝ｬ繝昴・繝・ `docs/inbox/REPORT_TASK_005_PackageInstallationFix.md` 繧貞盾辣ｧ
- 繝励Ο繧ｸ繧ｧ繧ｯ繝井ｻ墓ｧ・ `譛蛻昴・繝励Ο繝ｳ繝励ヨ`・医・繝ｭ繧ｸ繧ｧ繧ｯ繝医Ν繝ｼ繝茨ｼ峨ｒ蜿ら・
- Unity繝舌・繧ｸ繝ｧ繝ｳ: Unity 6 (or 2022 LTS)
- Yarn Spinner API: 譛譁ｰ縺ｮ繝峨く繝･繝｡繝ｳ繝医ｒ蜿ら・・医ヰ繝ｼ繧ｸ繝ｧ繝ｳ萓晏ｭ倥・蜿ｯ閭ｽ諤ｧ縺ゅｊ・・- 繧ｨ繝ｩ繝ｼ逋ｺ逕溽ｮ・園: `Assets/Scripts/Core/ScenarioManager.cs` (287陦檎岼, 322陦檎岼, 60陦檎岼, 21陦檎岼)

## 豕ｨ諢丈ｺ矩・1. **Yarn Spinner API**: VariableStorage API縺ｯ繝舌・繧ｸ繝ｧ繝ｳ縺ｫ繧医▲縺ｦ逡ｰ縺ｪ繧句庄閭ｽ諤ｧ縺後≠繧翫∪縺吶ょｮ溯｣・凾縺ｯ螳滄圀縺ｮAPI繧堤｢ｺ隱阪＠縺ｦ縺九ｉ螳溯｣・＠縺ｦ縺上□縺輔＞縲・2. **蝙句ｮ牙・諤ｧ**: TryGetValue縺ｨSetValue縺ｮ菫ｮ豁｣譎ゅ・縲∝梛螳牙・諤ｧ繧剃ｿ昴▽繧医≧縺ｫ豕ｨ諢上＠縺ｦ縺上□縺輔＞縲・3. **Unity 6 API**: FindObjectOfType縺ｯ髱樊耳螂ｨ縺ｮ縺溘ａ縲：indFirstObjectByType縺ｾ縺溘・FindAnyObjectByType縺ｫ鄂ｮ縺肴鋤縺医ｋ蠢・ｦ√′縺ゅｊ縺ｾ縺吶・4. **譛ｪ菴ｿ逕ｨ螟画焚**: m_IsInputLocked縺ｯ蟆・擂縺ｮ螳溯｣・畑縺ｨ縺励※菫晄戟縺吶ｋ蝣ｴ蜷医・縲・pragma warning縺ｧ謚大宛縺吶ｋ縺薙→繧呈耳螂ｨ縺励∪縺吶・
