# Task: Unity繝代ャ繧ｱ繝ｼ繧ｸ繧､繝ｳ繧ｹ繝医・繝ｫ
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T10:15:00Z
Report: docs/reports/REPORT_TASK_004_PackageInstallation.md 

## Objective
Unity繝励Ο繧ｸ繧ｧ繧ｯ繝医↓蠢・ｦ√↑繝代ャ繧ｱ繝ｼ繧ｸ・・arn Spinner, DOTween, TextMeshPro・峨ｒ繧､繝ｳ繧ｹ繝医・繝ｫ縺吶ｋ縲ゅさ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ繧定ｧ｣豸医＠縲∝ｮ溯｣・ｸ医∩繧ｳ繝ｼ繝峨′豁｣蟶ｸ縺ｫ蜍穂ｽ懊☆繧狗腸蠅・ｒ謨ｴ縺医ｋ縲・
螳溯｣・ｯｾ雎｡・・1. **Yarn Spinner**: 繧ｷ繝翫Μ繧ｪ邂｡逅・→繧ｫ繧ｹ繧ｿ繝繧ｳ繝槭Φ繝牙・逅・2. **DOTween Pro**: UI繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・医せ繧ｯ繝ｭ繝ｼ繝ｫ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ遲会ｼ・3. **TextMeshPro**: 繝・く繧ｹ繝郁｡ｨ遉ｺ・医Γ繝・そ繝ｼ繧ｸ繝舌ヶ繝ｫ縲√ち繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ・・
## Context
- 蜑阪ち繧ｹ繧ｯ・・ASK_001, TASK_002・峨〒繧ｳ繝ｼ繝牙ｮ溯｣・′螳御ｺ・- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺檎匱逕溘＠縺ｦ縺・ｋ・亥ｿ・ｦ√↑繝代ャ繧ｱ繝ｼ繧ｸ縺梧悴繧､繝ｳ繧ｹ繝医・繝ｫ・・- 繧ｨ繝ｩ繝ｼ蜀・ｮｹ:
  - `Yarn` 蜷榊燕遨ｺ髢薙′隕九▽縺九ｉ縺ｪ縺・  - `DG` (DOTween) 蜷榊燕遨ｺ髢薙′隕九▽縺九ｉ縺ｪ縺・  - `TMPro` (TextMeshPro) 蜷榊燕遨ｺ髢薙′隕九▽縺九ｉ縺ｪ縺・  - `UnityEngine.UI` 縺瑚ｦ九▽縺九ｉ縺ｪ縺・ｼ・nity UI繝｢繧ｸ繝･繝ｼ繝ｫ・・- 蜿ら・繝峨く繝･繝｡繝ｳ繝・ `譛蛻昴・繝励Ο繝ｳ繝励ヨ`・医・繝ｭ繧ｸ繧ｧ繧ｯ繝医Ν繝ｼ繝茨ｼ・
## Focus Area
- `Packages/manifest.json` 縺ｮ譖ｴ譁ｰ
- Unity Package Manager繧剃ｽｿ逕ｨ縺励◆繝代ャ繧ｱ繝ｼ繧ｸ繧､繝ｳ繧ｹ繝医・繝ｫ
- 繝代ャ繧ｱ繝ｼ繧ｸ繝舌・繧ｸ繝ｧ繝ｳ縺ｮ遒ｺ隱阪→莠呈鋤諤ｧ繝√ぉ繝・け
- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺ｮ隗｣豸育｢ｺ隱・
## Forbidden Area
- 譌｢蟄倥ヵ繧｡繧､繝ｫ縺ｮ蜑企勁繝ｻ遐ｴ螢顔噪螟画峩
- Unity繝励Ο繧ｸ繧ｧ繧ｯ繝郁ｨｭ螳壹・螟画峩・医ヱ繝・こ繝ｼ繧ｸ繧､繝ｳ繧ｹ繝医・繝ｫ莉･螟厄ｼ・- 螳溯｣・ｸ医∩繧ｳ繝ｼ繝峨・螟画峩・医ヱ繝・こ繝ｼ繧ｸ繧､繝ｳ繧ｹ繝医・繝ｫ縺ｮ縺ｿ・・- 譁ｰ隕上せ繧ｯ繝ｪ繝励ヨ縺ｮ菴懈・

## Constraints
- 繝・せ繝・ 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後ｋ縺薙→繧堤｢ｺ隱・- 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: 繝代ャ繧ｱ繝ｼ繧ｸ繧､繝ｳ繧ｹ繝医・繝ｫ縺ｮ縺ｿ・医さ繝ｼ繝牙､画峩縺ｪ縺暦ｼ・- 繝代ャ繧ｱ繝ｼ繧ｸ繝舌・繧ｸ繝ｧ繝ｳ: Unity 6 (or 2022 LTS) 縺ｨ莠呈鋤諤ｧ縺ｮ縺ゅｋ繝舌・繧ｸ繝ｧ繝ｳ繧剃ｽｿ逕ｨ
- 繧､繝ｳ繧ｹ繝医・繝ｫ譁ｹ豕・ Unity Package Manager縺ｾ縺溘・manifest.json縺ｮ逶ｴ謗･邱ｨ髮・
## DoD
- [ ] Yarn Spinner 縺後う繝ｳ繧ｹ繝医・繝ｫ縺輔ｌ縺ｦ縺・ｋ
  - [ ] `com.yarnspinner.yarnspinner` 繝代ャ繧ｱ繝ｼ繧ｸ縺稽anifest.json縺ｫ霑ｽ蜉縺輔ｌ縺ｦ縺・ｋ
  - [ ] 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ・・arn蜷榊燕遨ｺ髢難ｼ峨′隗｣豸医＆繧後※縺・ｋ
- [ ] DOTween Pro 縺後う繝ｳ繧ｹ繝医・繝ｫ縺輔ｌ縺ｦ縺・ｋ
  - [ ] `com.demigiant.dottweenpro` 縺ｾ縺溘・ `com.demigiant.dottween` 繝代ャ繧ｱ繝ｼ繧ｸ縺稽anifest.json縺ｫ霑ｽ蜉縺輔ｌ縺ｦ縺・ｋ
  - [ ] 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ・・G蜷榊燕遨ｺ髢難ｼ峨′隗｣豸医＆繧後※縺・ｋ
- [ ] TextMeshPro 縺後う繝ｳ繧ｹ繝医・繝ｫ縺輔ｌ縺ｦ縺・ｋ
  - [ ] `com.unity.textmeshpro` 繝代ャ繧ｱ繝ｼ繧ｸ縺稽anifest.json縺ｫ霑ｽ蜉縺輔ｌ縺ｦ縺・ｋ
  - [ ] 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ・・MPro蜷榊燕遨ｺ髢難ｼ峨′隗｣豸医＆繧後※縺・ｋ
- [ ] UnityEngine.UI 縺悟茜逕ｨ蜿ｯ閭ｽ縺ｧ縺ゅｋ縺薙→繧堤｢ｺ隱・  - [ ] Unity UI繝｢繧ｸ繝･繝ｼ繝ｫ縺梧怏蜉ｹ縺ｫ縺ｪ縺｣縺ｦ縺・ｋ縺薙→繧堤｢ｺ隱・- [ ] 蜈ｨ縺ｦ縺ｮ繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ
- [ ] docs/inbox/ 縺ｫ繝ｬ繝昴・繝茨ｼ・EPORT_TASK_004_PackageInstallation.md・峨′菴懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## 螳溯｣・ｩｳ邏ｰ

### 1. Yarn Spinner 繧､繝ｳ繧ｹ繝医・繝ｫ

#### 繝代ャ繧ｱ繝ｼ繧ｸ諠・ｱ
- **繝代ャ繧ｱ繝ｼ繧ｸ蜷・*: `com.yarnspinner.yarnspinner`
- **繧､繝ｳ繧ｹ繝医・繝ｫ譁ｹ豕・*: 
  - Unity Package Manager: `Window` 竊・`Package Manager` 竊・`+` 竊・`Add package from git URL` 竊・`https://github.com/YarnSpinner/YarnSpinner-Unity.git?path=/YarnSpinner-Unity`
  - 縺ｾ縺溘・ manifest.json縺ｫ逶ｴ謗･霑ｽ蜉: `"com.yarnspinner.yarnspinner": "https://github.com/YarnSpinner/YarnSpinner-Unity.git?path=/YarnSpinner-Unity"`

#### 遒ｺ隱堺ｺ矩・- `Yarn.Unity` 蜷榊燕遨ｺ髢薙′蛻ｩ逕ｨ蜿ｯ閭ｽ
- `DialogueRunner` 繧ｯ繝ｩ繧ｹ縺悟茜逕ｨ蜿ｯ閭ｽ
- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ

### 2. DOTween Pro 繧､繝ｳ繧ｹ繝医・繝ｫ

#### 繝代ャ繧ｱ繝ｼ繧ｸ諠・ｱ
- **繝代ャ繧ｱ繝ｼ繧ｸ蜷・*: `com.demigiant.dottweenpro` (Pro迚・ 縺ｾ縺溘・ `com.demigiant.dottween` (Free迚・
- **繧､繝ｳ繧ｹ繝医・繝ｫ譁ｹ豕・*: 
  - Asset Store縺九ｉ繧､繝ｳ繧ｹ繝医・繝ｫ・域耳螂ｨ・・  - 縺ｾ縺溘・ manifest.json縺ｫ逶ｴ謗･霑ｽ蜉・・sset Store繝代ャ繧ｱ繝ｼ繧ｸ縺ｮ蝣ｴ蜷医・蛻･騾泌ｯｾ蠢懊′蠢・ｦ・ｼ・- **豕ｨ諢・*: DOTween Pro縺ｯ譛画侭繝代ャ繧ｱ繝ｼ繧ｸ縺ｮ縺溘ａ縲、sset Store縺九ｉ雉ｼ蜈･繝ｻ繧､繝ｳ繧ｹ繝医・繝ｫ縺悟ｿ・ｦ√↑蝣ｴ蜷医′縺ゅｊ縺ｾ縺・
#### 遒ｺ隱堺ｺ矩・- `DG.Tweening` 蜷榊燕遨ｺ髢薙′蛻ｩ逕ｨ蜿ｯ閭ｽ
- `DOTween.To()` 繝｡繧ｽ繝・ラ縺悟茜逕ｨ蜿ｯ閭ｽ
- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ

### 3. TextMeshPro 繧､繝ｳ繧ｹ繝医・繝ｫ

#### 繝代ャ繧ｱ繝ｼ繧ｸ諠・ｱ
- **繝代ャ繧ｱ繝ｼ繧ｸ蜷・*: `com.unity.textmeshpro`
- **繧､繝ｳ繧ｹ繝医・繝ｫ譁ｹ豕・*: 
  - Unity Package Manager: `Window` 竊・`Package Manager` 竊・`Unity Registry` 竊・`TextMeshPro` 繧呈､懃ｴ｢縺励※繧､繝ｳ繧ｹ繝医・繝ｫ
  - 縺ｾ縺溘・ manifest.json縺ｫ逶ｴ謗･霑ｽ蜉: `"com.unity.textmeshpro": "3.0.6"` (Unity 2022 LTS縺ｮ蝣ｴ蜷・

#### 遒ｺ隱堺ｺ矩・- `TMPro` 蜷榊燕遨ｺ髢薙′蛻ｩ逕ｨ蜿ｯ閭ｽ
- `TextMeshProUGUI` 繧ｯ繝ｩ繧ｹ縺悟茜逕ｨ蜿ｯ閭ｽ
- 繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺瑚ｧ｣豸医＆繧後※縺・ｋ

### 4. UnityEngine.UI 遒ｺ隱・
#### 遒ｺ隱堺ｺ矩・- Unity UI繝｢繧ｸ繝･繝ｼ繝ｫ縺梧怏蜉ｹ縺ｫ縺ｪ縺｣縺ｦ縺・ｋ縺薙→繧堤｢ｺ隱・- `UnityEngine.UI` 蜷榊燕遨ｺ髢薙′蛻ｩ逕ｨ蜿ｯ閭ｽ
- `ScrollRect`, `VerticalLayoutGroup` 繧ｯ繝ｩ繧ｹ縺悟茜逕ｨ蜿ｯ閭ｽ

## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- DOTween Pro縺梧怏譁吶ヱ繝・こ繝ｼ繧ｸ縺ｮ蝣ｴ蜷医、sset Store縺九ｉ雉ｼ蜈･繝ｻ繧､繝ｳ繧ｹ繝医・繝ｫ縺悟ｿ・ｦ√〒縺吶・ree迚茨ｼ・OTween・峨ｒ菴ｿ逕ｨ縺吶ｋ蝣ｴ蜷医・縲［anifest.json縺ｫ驕ｩ蛻・↑繝代ャ繧ｱ繝ｼ繧ｸID繧定ｿｽ蜉縺励※縺上□縺輔＞縲・- Unity繧ｨ繝・ぅ繧ｿ縺瑚ｵｷ蜍輔＠縺ｦ縺・↑縺・ｴ蜷医・縲［anifest.json繧堤峩謗･邱ｨ髮・＠縺ｦ繝代ャ繧ｱ繝ｼ繧ｸ繧定ｿｽ蜉縺吶ｋ縺薙→繧ょ庄閭ｽ縺ｧ縺吶ゅ◆縺縺励ゞnity繧ｨ繝・ぅ繧ｿ縺ｧ髢九＞縺滄圀縺ｫ繝代ャ繧ｱ繝ｼ繧ｸ縺梧ｭ｣縺励￥繧､繝ｳ繧ｹ繝医・繝ｫ縺輔ｌ繧九％縺ｨ繧堤｢ｺ隱阪＠縺ｦ縺上□縺輔＞縲・
