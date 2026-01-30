# Worker Prompt: TASK_018 Deduction Board Implementation

## 孱・・縺薙・繝励Ο繝ｳ繝励ヨ縺ｮ蠖ｹ蜑ｲ
縺ゅ↑縺溘・ **Worker** 縺ｧ縺吶ゅ％縺ｮ繝励Ο繝ｳ繝励ヨ縺ｫ蠕薙＞縲∵欠螳壹＆繧後◆繧ｿ繧ｹ繧ｯ繧貞ｮ溯｣・＠縺ｦ縺上□縺輔＞縲・**Orchestrator** (遘・ 縺ｯ縲√≠縺ｪ縺溘・謌先棡迚ｩ繧堤ｵｱ蜷医＠縲∝刀雉ｪ繧剃ｿ晁ｨｼ縺励∪縺吶・
## 識 莉雁屓縺ｮ繝溘ャ繧ｷ繝ｧ繝ｳ
**繧ｿ繧ｹ繧ｯ**: [TASK_018_DeductionBoard_Implementation.md](../tasks/TASK_018_DeductionBoard_Implementation.md)
**逶ｮ逧・*: 謗ｨ逅・・繝ｼ繝・(Deduction Board) 縺ｮ蝓ｺ譛ｬ繧ｷ繧ｹ繝・Β繧貞ｮ溯｣・＠縲√す繝翫Μ繧ｪ騾ｲ陦後→騾｣蜍輔＆縺帙ｋ縲・
## 唐 蜿ら・繝輔ぃ繧､繝ｫ (SSOT)
- **Task Definition**: `docs/tasks/TASK_018_DeductionBoard_Implementation.md` (DoD 縺ｯ縺薙％縺ｫ蠕薙≧)
- **TopicData**: `Assets/Scripts/Data/TopicData.cs` (譌｢蟄倥け繝ｩ繧ｹ)
- **ScenarioManager**: `Assets/Scripts/Core/ScenarioManager.cs` (`UnlockTopicCommand` 縺ｮ菫ｮ豁｣蟇ｾ雎｡)
- **Topic Assets**: `Assets/Resources/Topics/` (繝・せ繝医ョ繝ｼ繧ｿ)

## 屏・・螳溯｣・焔鬆・(Step-by-Step)

### Step 1: UI Script Implementation
1. `Assets/Scripts/UI/DeductionBoard.cs` 繧剃ｽ懈・縲・   - Singleton 繝代ち繝ｼ繝ｳ (or Manager reference)
   - `AddTopic(TopicData data)` 繝｡繧ｽ繝・ラ
   - 驥崎､・メ繧ｧ繝・け讖溯・
2. `Assets/Scripts/UI/TopicCard.cs` 繧剃ｽ懈・縲・   - `Setup(TopicData data)` 繝｡繧ｽ繝・ラ
   - 繧｢繧､繧ｳ繝ｳ縺ｨ繧ｿ繧､繝医Ν縺ｮ陦ｨ遉ｺ

### Step 2: Prefab Creation
1. Unity Editor 縺ｯ謫堺ｽ懊〒縺阪↑縺・◆繧√√せ繧ｯ繝ｪ繝励ヨ縺ｧ Prefab 讒狗ｯ臥畑 Editor Script 繧剃ｽ懊ｋ縺九√≠繧九＞縺ｯ **繧ｳ繝ｼ繝峨・繝ｼ繧ｹ縺ｧ Prefab 縺ｮ讒区・隕∽ｻｶ** 繧貞ｮ夂ｾｩ縺励√Θ繝ｼ繧ｶ繝ｼ縺ｫ謇句虚菴懈・繧剃ｾ晞ｼ縺吶ｋ蠖｢縺ｫ縺ｪ繧具ｼ医ｂ縺励￥縺ｯ縲∫ｰ｡蜊倥↑ EditorWindow 縺ｧ閾ｪ蜍慕函謌舌☆繧九せ繧ｯ繝ｪ繝励ヨ繧呈署萓幢ｼ峨・   - 謗ｨ螂ｨ: `DeductionBoard` 縺ｯ Canvas 逶ｴ荳九・ Panel縲ＡTopicCard` 縺ｯ縺昴・荳ｭ縺ｮ Content 隕∫ｴ縲・2. 莉雁屓縺ｯ **Unity Editor 讀懆ｨｼ譎ゅ↓ Prefab 蛹悶☆繧・* 蜑肴署縺ｧ縲√∪縺壹・繧ｹ繧ｯ繝ｪ繝励ヨ繧貞ｮ檎挑縺ｫ縺吶ｋ縲・
### Step 3: Integration
1. `Assets/Scripts/Core/ScenarioManager.cs` 繧剃ｿｮ豁｣縲・   - `UnlockTopicCommand` 繝｡繧ｽ繝・ラ蜀・〒縲～DeductionBoard.Instance` 縺悟ｭ伜惠縺吶ｌ縺ｰ `AddTopic` 繧貞他縺ｶ繧医≧縺ｫ螟画峩縲・
### Step 4: Verification Setup
1. `Assets/Scripts/Dev/DeductionBoardVerifier.cs` (Editor Script or Runtime Script) 繧剃ｽ懈・縲・   - `AddTopic` 繧堤峩謗･蜻ｼ繧薙〒繧ｫ繝ｼ繝峨′蠅励∴繧九°繝・せ繝医〒縺阪ｋ繧ｹ繧ｯ繝ｪ繝励ヨ縲・
## 笨・螳御ｺ・擅莉ｶ (DoD)
- [ ] `DeductionBoard.cs` 縺ｨ `TopicCard.cs` 縺後さ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺ｪ縺丞ｮ溯｣・＆繧後※縺・ｋ縲・- [ ] `ScenarioManager` 縺後・繝ｼ繝峨→騾｣謳ｺ縺励※縺・ｋ縲・- [ ] 繝・せ繝育畑繧ｹ繧ｯ繝ｪ繝励ヨ (`DeductionBoardVerifier.cs`遲・ 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ縲・- [ ] 繝ｬ繝昴・繝・(`docs/reports/REPORT_TASK_018_DeductionBoard_Implementation.md`) 繧剃ｽ懈・縲・  - 繝ｬ繝昴・繝医↓縺ｯ縲袈nity Editor 縺ｧ縺ｮ謇矩・阪ｒ蜷ｫ繧√ｋ・医Θ繝ｼ繧ｶ繝ｼ縺ｫ Prefab 菴懈・繧剃ｽ懈･ｭ萓晞ｼ縺吶ｋ縺溘ａ・峨・
## 笞・・豕ｨ諢丈ｺ矩・- **ChatUI 縺ｨ縺ｯ迢ｬ遶九＆縺帙ｋ**: ChatController 縺ｫ萓晏ｭ倥＠縺ｪ縺・％縺ｨ縲・- **繝・・繧ｿ鬧・虚**: TopicData (ScriptableObject) 縺ｮ蜀・ｮｹ繧呈ｭ｣縺励￥陦ｨ遉ｺ縺吶ｋ縺薙→縲・- **繧ｨ繝ｩ繝ｼ繝上Φ繝峨Μ繝ｳ繧ｰ**: `TopicData` 縺・null 縺ｮ蝣ｴ蜷医↑縺ｩ繧定・・縲・
## 統 謠仙・迚ｩ
螳溯｣・ｮ御ｺ・ｾ後∽ｻ･荳九・繝輔か繝ｼ繝槭ャ繝医〒繝ｬ繝昴・繝医ｒ菴懈・縺励～docs/inbox/` 縺ｫ菫晏ｭ倥＠縺ｦ縺上□縺輔＞縲・
```markdown
# Report: TASK_018 Deduction Board Implementation

## Status
IMPLEMENTED (Verification Pending)

## Changes
- Created: Assets/Scripts/UI/DeductionBoard.cs
- Created: Assets/Scripts/UI/TopicCard.cs
- Modified: Assets/Scripts/Core/ScenarioManager.cs
- Created: Assets/Scripts/Dev/DeductionBoardVerifier.cs

## Verification Steps (For User)
1. Hierarchy 縺ｫ Canvas/DeductionBoard 繧剃ｽ懈・縺励．eductionBoard 繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ繧｢繧ｿ繝・メ縲・2. ...
```
