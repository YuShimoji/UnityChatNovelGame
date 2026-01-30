# Worker Prompt: TASK_010_MetaEffectController

縺ゅ↑縺溘・ Worker Agent 縺ｧ縺吶ゆｻ･荳九・謖・､ｺ縺ｫ蠕薙＞縲√ち繧ｹ繧ｯ繧帝≠陦後＠縺ｦ縺上□縺輔＞縲・
Orchestrator 縺ｫ繧医▲縺ｦ螳夂ｾｩ縺輔ｌ縺溷｢・阜(Focus/Forbidden)繧帝・螳医☆繧九％縺ｨ縺梧ｱゅａ繧峨ｌ縺ｾ縺吶・

## 蜿ら・諠・ｱ
- **繝√こ繝・ヨ**: `docs/tasks/TASK_010_MetaEffectController.md` (蠢・ｪｭ)
- **SSOT**: `docs/Windsurf_AI_Collab_Rules_latest.md`
- **HANDOVER**: `docs/HANDOVER.md`
- **Context**: `AI_CONTEXT.md`
- **髢｢騾｣螳溯｣・*: `Assets/Scripts/Core/ScenarioManager.cs` (GlitchCommand)

## 繝溘ャ繧ｷ繝ｧ繝ｳ
**MetaEffectController (貍泌・繧ｨ繝輔ぉ繧ｯ繝・ 螳溯｣・*

- `MetaEffectController.cs` 繧呈眠隕丈ｽ懈・縺励∫判髱｢蜈ｨ菴薙・繧ｨ繝輔ぉ繧ｯ繝育ｮ｡逅・ｒ螳溯｣・☆繧九・
- `GlitchEffect.cs` 繧呈眠隕丈ｽ懈・縺励√げ繝ｪ繝・メ貍泌・(繝弱う繧ｺ縲∫判髱｢謠ｺ繧檎ｭ・繧貞ｮ溯｣・☆繧九・
- `MetaEffectOverlay.prefab` 繧剃ｽ懈・縺励∫判髱｢繧ｪ繝ｼ繝舌・繝ｬ繧､縺ｨ縺励※驟咲ｽｮ蜿ｯ閭ｽ縺ｫ縺吶ｋ縲・
- `ScenarioManager.GlitchCommand` 縺九ｉ蜻ｼ縺ｳ蜃ｺ縺帙ｋ繧医≧縺ｫ縺吶ｋ縲・

## 蠅・阜 (Boundaries)

### Focus Area(螟画峩險ｱ蜿ｯ)
- `Assets/Scripts/UI/MetaEffectController.cs` (譁ｰ隕・
- `Assets/Scripts/UI/Effects/GlitchEffect.cs` (譁ｰ隕・
- `Assets/Prefabs/UI/MetaEffectOverlay.prefab` (譁ｰ隕・
- `Assets/Scripts/Core/ScenarioManager.cs` (GlitchCommand騾｣謳ｺ縺ｮ縺ｿ)

### Forbidden Area(螟画峩遖∵ｭ｢)
- `Assets/Scripts/UI/ChatController.cs`
- `Assets/Scripts/UI/DeductionBoard.cs`
- 譌｢蟄呂amera險ｭ螳壹・遐ｴ螢顔噪螟画峩

## Definition of Done (DoD)
- [ ] `MetaEffectController.cs` 縺悟ｮ溯｣・＆繧後√お繝輔ぉ繧ｯ繝医・髢句ｧ・蛛懈ｭ｢縺後〒縺阪ｋ
- [ ] `GlitchEffect.cs` 縺悟ｮ溯｣・＆繧後√Ξ繝吶Ν縺ｫ蠢懊§縺溘げ繝ｪ繝・メ貍泌・縺後〒縺阪ｋ
- [ ] `MetaEffectOverlay.prefab` 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] `ScenarioManager.GlitchCommand` 縺九ｉ繧ｨ繝輔ぉ繧ｯ繝亥他縺ｳ蜃ｺ縺励′讖溯・縺吶ｋ
- [ ] Unity Editor縺ｧ蜍穂ｽ懃｢ｺ隱阪′螳御ｺ・＠縺ｦ縺・ｋ
- [ ] `docs/reports/REPORT_TASK_010_MetaEffectController.md` 縺ｫ繝ｬ繝昴・繝医′菴懈・縺輔ｌ縺ｦ縺・ｋ

## 蛛懈ｭ｢譚｡莉ｶ (Stop & Report)
- Post Processing縺悟ｿ・医↓縺ｪ縺｣縺溷ｴ蜷・
- 繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ蝠城｡後′逋ｺ逕溘＠縺溷ｴ蜷・

## 謚陦薙ヲ繝ｳ繝・
- DOTween繧剃ｽｿ逕ｨ縺励※繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ(逕ｻ髱｢謠ｺ繧後√い繝ｫ繝輔ぃ螟画峩遲・
- UI Image縺ｮcolor/material繧貞､画峩縺励※繝弱う繧ｺ陦ｨ迴ｾ
- Canvas縺ｮ譛蜑埼擇縺ｫ繧ｪ繝ｼ繝舌・繝ｬ繧､繧帝・鄂ｮ

## 邏榊刀迚ｩ
- 譁ｰ隕丈ｽ懈・縺輔ｌ縺溘さ繝ｼ繝・MetaEffectController.cs, GlitchEffect.cs)
- 譁ｰ隕襲refab
- `docs/reports/REPORT_TASK_010_MetaEffectController.md`
