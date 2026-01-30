<<<<<<< HEAD
# Task: MetaEffectController Implementation

Status: DONE
Tier: 2
Branch: feat/meta-effect-controller
Owner: Worker
Created: 2026-01-17T02:00:00+09:00
Report: docs/reports/REPORT_TASK_010_MetaEffectController.md 

## Objective
繝｡繧ｿ貍泌・・医げ繝ｪ繝・メ蜉ｹ譫懃ｭ会ｼ峨ｒ蛻ｶ蠕｡縺吶ｋ `MetaEffectController` 繧貞ｮ溯｣・☆繧九・`ScenarioManager` 縺ｮ `GlitchCommand` 縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧九げ繝ｪ繝・メ蜉ｹ譫懊す繧ｹ繝・Β繧呈ｧ狗ｯ峨☆繧九・
## Context
- `ScenarioManager.cs` 縺ｮ `GlitchCommand(int level)` 縺悟ｮ溯｣・ｸ医∩縺縺後∫樟蝨ｨ縺ｯ Debug.Log 縺ｮ縺ｿ
- `GlitchCommand` 縺ｯ `MetaEffectController.Instance.PlayGlitchEffect(level)` 繧貞他縺ｳ蜃ｺ縺呎Φ螳・- 繧ｰ繝ｪ繝・メ蜉ｹ譫懊・逕ｻ髱｢蜈ｨ菴薙↓繝弱う繧ｺ繧定｡ｨ遉ｺ縺吶ｋ貍泌・・医Ξ繝吶Ν1-5遞句ｺｦ繧呈Φ螳夲ｼ・- 蟆・擂逧・↓莉悶・繝｡繧ｿ貍泌・・育判髱｢謠ｺ繧後∬牡隱ｿ螟画峩遲会ｼ峨ｂ霑ｽ蜉蜿ｯ閭ｽ縺ｪ險ｭ險医′蠢・ｦ・
## Focus Area
- `Assets/Scripts/Effects/MetaEffectController.cs`: 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ縺ｾ縺溘・髱咏噪繧｢繧ｯ繧ｻ繧ｵ繧呈戟縺､繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ
- `Assets/Scripts/Effects/GlitchEffect.cs`: 繧ｰ繝ｪ繝・メ蜉ｹ譫懊・螳溯｣・ｼ・hader縺ｾ縺溘・Post-Processing・・- `Assets/Materials/Effects/GlitchMaterial.mat`: 繧ｰ繝ｪ繝・メ逕ｨ繝槭ユ繝ｪ繧｢繝ｫ・亥ｿ・ｦ√↓蠢懊§縺ｦ・・- Unity Post-Processing Stack 縺ｾ縺溘・ Shader Graph 繧剃ｽｿ逕ｨ縺励◆螳溯｣・- 繝ｬ繝吶Ν縺ｫ蠢懊§縺溷ｼｷ蠎ｦ隱ｿ謨ｴ・・-5遞句ｺｦ・・
## Forbidden Area
- 譌｢蟄倥・ `ChatController` 繧・`ScenarioManager` 縺ｮ遐ｴ螢顔噪螟画峩
- 驕主ｺｦ縺ｪ隕冶ｦ壼柑譫懶ｼ医∪縺壹・蝓ｺ譛ｬ逧・↑繧ｰ繝ｪ繝・メ蜉ｹ譫懊・縺ｿ・・- 螟夜Κ繧｢繧ｻ繝・ヨ縺ｮ霑ｽ蜉・・nity讓呎ｺ匁ｩ溯・縺ｾ縺溘・譌｢蟄倥ヱ繝・こ繝ｼ繧ｸ縺ｮ縺ｿ菴ｿ逕ｨ・・
## Constraints
- 繝・せ繝・ 荳ｻ隕√ヱ繧ｹ縺ｮ縺ｿ・・litchCommand縺九ｉ縺ｮ蜻ｼ縺ｳ蜃ｺ縺礼｢ｺ隱搾ｼ・- 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: 譁ｰ隕剰ｿｽ蜉遖∵ｭ｢・域里蟄倥・繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ縺ｨ縺ｮ騾｣謳ｺ繧堤ｶｭ謖・ｼ・- Unity 2022.3 LTS 莉･髯阪↓蟇ｾ蠢・- 繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ: 60fps邯ｭ謖√ｒ逶ｮ讓呻ｼ磯㍾縺・・逅・・驕ｿ縺代ｋ・・
## DoD (Definition of Done)
- [x] `MetaEffectController.cs` 縺悟ｮ溯｣・＆繧後※縺・ｋ・医す繝ｳ繧ｰ繝ｫ繝医Φ縺ｾ縺溘・髱咏噪繧｢繧ｯ繧ｻ繧ｵ・・- [x] `PlayGlitchEffect(int level)` 繝｡繧ｽ繝・ラ縺悟ｮ溯｣・＆繧後※縺・ｋ
- [x] `ScenarioManager.GlitchCommand` 縺九ｉ `MetaEffectController.Instance.PlayGlitchEffect(level)` 繧貞他縺ｳ蜃ｺ縺帙ｋ
- [x] 繝ｬ繝吶Ν1-5縺ｫ蠢懊§縺溘げ繝ｪ繝・メ蠑ｷ蠎ｦ縺悟渚譏縺輔ｌ繧・- [x] Unity Editor 荳翫〒蜍穂ｽ懃｢ｺ隱阪′縺ｧ縺阪ｋ・・ebugScript.yarn 縺ｮ `<<Glitch>>` 繧ｳ繝槭Φ繝峨〒遒ｺ隱搾ｼ・- [ ] **Evidence**: 繧ｰ繝ｪ繝・メ蜉ｹ譫懊・繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ縺ｾ縺溘・蜍慕判・医Θ繝ｼ繧ｶ繝ｼ遒ｺ隱阪′蠢・ｦ・ｼ・- [x] `docs/inbox/` 縺ｫ繝ｬ繝昴・繝・(`REPORT_TASK_010_MetaEffectController.md`) 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [x] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- 繧ｰ繝ｪ繝・メ蜉ｹ譫懊・螳溯｣・婿豕輔・ Shader縲￣ost-Processing縲√∪縺溘・ UI Image 縺ｮ繝槭ユ繝ｪ繧｢繝ｫ螟画峩縺ｪ縺ｩ縲ゞnity讓呎ｺ匁ｩ溯・縺ｧ螳溽樟蜿ｯ閭ｽ縺ｪ譁ｹ豕輔ｒ驕ｸ謚槭☆繧九％縺ｨ
- 蟆・擂逧・↑諡｡蠑ｵ諤ｧ・井ｻ悶・繝｡繧ｿ貍泌・霑ｽ蜉・峨ｒ閠・・縺励◆險ｭ險医ｒ謗ｨ螂ｨ
=======
# Task: MetaEffectController (貍泌・繧ｨ繝輔ぉ繧ｯ繝・ 螳溯｣・Status: OPEN
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-16T13:55:00Z
Report: docs/reports/REPORT_TASK_010_MetaEffectController.md

## Objective
逕ｻ髱｢蜈ｨ菴薙↓蠖ｱ髻ｿ繧剃ｸ弱∴繧区ｼ泌・繧ｨ繝輔ぉ繧ｯ繝・繧ｰ繝ｪ繝・メ縲√ヵ繧ｧ繝ｼ繝峨√ヮ繧､繧ｺ遲・繧堤ｮ｡逅・☆繧九勲etaEffectController縲阪ｒ螳溯｣・☆繧九・ScenarioManager縺ｮGlitchCommand縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ縲√Ξ繝吶Ν縺ｫ蠢懊§縺溘お繝輔ぉ繧ｯ繝医ｒ陦ｨ遉ｺ縺吶ｋ縲・
## Context
- **蜑榊ｷ･遞・*: TASK_002縺ｧGlitchCommand縺ｮ繧ｹ繧ｱ繝ｫ繝医Φ縺悟ｮ溯｣・ｸ医∩(Debug.Log縺ｧ莉｣譖ｿ荳ｭ)
- **繧ｴ繝ｼ繝ｫ**: 繝√Ε繝・ヨ繝弱・繝ｫ繧ｲ繝ｼ繝縺ｮ貍泌・蠑ｷ蛹・諱先匁ｼ泌・縲√す繧ｹ繝・Β逡ｰ蟶ｸ貍泌・縺ｪ縺ｩ)

## Focus Area
- `Assets/Scripts/UI/MetaEffectController.cs` (譁ｰ隕・
- `Assets/Scripts/UI/Effects/GlitchEffect.cs` (譁ｰ隕・
- `Assets/Prefabs/UI/MetaEffectOverlay.prefab` (譁ｰ隕上∫判髱｢繧ｪ繝ｼ繝舌・繝ｬ繧､)
- `Assets/Scripts/Core/ScenarioManager.cs` (GlitchCommand騾｣謳ｺ驛ｨ蛻・・縺ｿ)

## Forbidden Area
- `Assets/Scripts/UI/ChatController.cs` 縺ｸ縺ｮ螟画峩
- `Assets/Scripts/UI/DeductionBoard.cs` 縺ｸ縺ｮ螟画峩
- 譌｢蟄倥・Camera險ｭ螳壹・遐ｴ螢顔噪螟画峩

## Constraints
- 繧ｨ繝輔ぉ繧ｯ繝医・UI Canvas荳翫・繧ｪ繝ｼ繝舌・繝ｬ繧､縺ｨ縺励※螳溯｣・Post Processing荳堺ｽｿ逕ｨ)
- DOTween繧剃ｽｿ逕ｨ縺励◆繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ
- 繧ｨ繝輔ぉ繧ｯ繝医Ξ繝吶Ν(0-3遞句ｺｦ)縺ｫ蠢懊§縺溷ｼｷ蠎ｦ隱ｿ謨ｴ

## Steps
1. MetaEffectController.cs 縺ｮ蝓ｺ譛ｬ讒矩繧貞ｮ溯｣・繧ｨ繝輔ぉ繧ｯ繝磯幕蟋・蛛懈ｭ｢)
2. GlitchEffect.cs 繧貞ｮ溯｣・繝弱う繧ｺ/逕ｻ髱｢謠ｺ繧・濶ｲ蜿主ｷｮ鬚ｨ)
3. MetaEffectOverlay.prefab 繧剃ｽ懈・
4. ScenarioManager.GlitchCommand縺ｨ騾｣謳ｺ
5. 繝・せ繝医す繝ｼ繝ｳ縺ｧ蜍穂ｽ懃｢ｺ隱・
## DoD (Definition of Done)
- [ ] `MetaEffectController.cs` 縺悟ｮ溯｣・＆繧後√お繝輔ぉ繧ｯ繝医・髢句ｧ・蛛懈ｭ｢縺後〒縺阪ｋ
- [ ] `GlitchEffect.cs` 縺悟ｮ溯｣・＆繧後√Ξ繝吶Ν縺ｫ蠢懊§縺溘げ繝ｪ繝・メ貍泌・縺後〒縺阪ｋ
- [ ] `MetaEffectOverlay.prefab` 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] `ScenarioManager.GlitchCommand` 縺九ｉ繧ｨ繝輔ぉ繧ｯ繝亥他縺ｳ蜃ｺ縺励′讖溯・縺吶ｋ
- [ ] Unity Editor縺ｧ蜍穂ｽ懃｢ｺ隱阪′螳御ｺ・＠縺ｦ縺・ｋ
- [ ] `docs/reports/REPORT_TASK_010_MetaEffectController.md` 縺ｫ繝ｬ繝昴・繝医′菴懈・縺輔ｌ縺ｦ縺・ｋ

## 蛛懈ｭ｢譚｡莉ｶ
- Post Processing縺悟ｿ・医↓縺ｪ縺｣縺溷ｴ蜷・隕∝挨騾泌愛譁ｭ)
- 繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ蝠城｡後′逋ｺ逕溘＠縺溷ｴ蜷・
## Notes
- TASK_008 (ChatUI Integration), TASK_009 (DeductionBoard) 縺ｨ荳ｦ陦悟ｮ溯｡悟庄閭ｽ
- 蟆・擂逧・↓縺ｯ繝輔ぉ繝ｼ繝峨√ヮ繧､繧ｺ縲∫判髱｢謠ｺ繧後↑縺ｩ隍・焚繧ｨ繝輔ぉ繧ｯ繝医ｒ霑ｽ蜉莠亥ｮ・>>>>>>> origin/main
