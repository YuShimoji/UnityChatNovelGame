# Task: Deduction Board Polish (Effects)

Status: DONE
Tier: 3 (Polish)
Branch: feat/deduction-polish
Owner: Worker
Created: 2026-01-29T09:00:00+09:00
Report: docs/reports/REPORT_TASK_020_DeductionBoard_Effects.md

## Objective
DeductionBoard 縺ｧ繝医ヴ繝・け蜷域・縺梧・蜉溘＠縺滄圀縺ｫ縲∬ｦ冶ｦ夂噪縺ｪ繝輔ぅ繝ｼ繝峨ヰ繝・け・・etaEffect・峨ｒ蜀咲函縺励√Θ繝ｼ繧ｶ繝ｼ菴馴ｨ薙ｒ蜷台ｸ翫＆縺帙ｋ縲・`MetaEffectController` (TASK_010) 繧剃ｽｿ逕ｨ縺励※縲∫判髱｢蜈ｨ菴薙∪縺溘・迚ｹ螳壹・蠎ｧ讓吶↓繧ｨ繝輔ぉ繧ｯ繝医ｒ陦ｨ遉ｺ縺吶ｋ縲・
## Context
- **Dependencies**:
  - `TASK_010`: MetaEffectController (Implemented)
  - `TASK_018`: DeductionBoard (Implemented)
  - `TASK_019`: Synthesis Logic (In Progress)
- **Goal**: "譁ｰ縺励＞逋ｺ隕・ 縺ｮ迸ｬ髢薙・蝟懊・繧呈ｼ泌・縺吶ｋ縲・
## Focus Area
- `Assets/Scripts/UI/DeductionBoard.cs`: 蜷域・謌仙粥蛻､螳夂ｮ・園
- `Assets/Resources/Effects/` (繧ゅ＠譁ｰ隕上お繝輔ぉ繧ｯ繝医′蠢・ｦ√↑繧・
- `MetaEffectController` 縺ｨ縺ｮ騾｣謳ｺ

## Constraints
- **Performance**: 繧ｨ繝輔ぉ繧ｯ繝医′繧ｲ繝ｼ繝繝励Ξ繧､繧帝仆螳ｳ縺励↑縺・％縺ｨ縲・- **Reusability**: 豎守畑逧・↑繧ｨ繝輔ぉ繧ｯ繝茨ｼ・Sparkle", "Confetti" 遲会ｼ峨ｒ菴ｿ逕ｨ縺ｾ縺溘・菴懈・縺吶ｋ縲・
## Steps
1. `DeductionBoard.cs` 縺ｮ `CheckSynthesis` 繝｡繧ｽ繝・ラ・亥粋謌先・蜉滓凾・臥ｭ峨・驕ｩ蛻・↑邂・園繧堤音螳壹☆繧九・2. `MetaEffectController.Instance.PlayEffect(...)` 繧貞他縺ｳ蜃ｺ縺吝・逅・ｒ霑ｽ蜉縺吶ｋ縲・3. 繧ｨ繝輔ぉ繧ｯ繝育函謌蝉ｽ咲ｽｮ繧定ｪｿ謨ｴ縺吶ｋ・医ラ繝ｩ繝・げ&繝峨Ο繝・・縺励◆蝨ｰ轤ｹ縲√∪縺溘・逕ｻ髱｢荳ｭ螟ｮ・峨・4. 螳滓ｩ滂ｼ・ditor・峨〒蜍穂ｽ懃｢ｺ隱阪ｒ陦後≧縲・
## DoD (Definition of Done)
- [ ] 蜷域・謌仙粥譎ゅ↓ MetaEffect 縺悟・逕溘＆繧後ｋ
- [ ] 繧ｨ繝輔ぉ繧ｯ繝医′驕ｩ蛻・↑菴咲ｽｮ縺ｧ陦ｨ遉ｺ縺輔ｌ繧・- [ ] 繧ｨ繝ｩ繝ｼ縺悟・縺ｪ縺・％縺ｨ・・etaEffectController 縺悟ｭ伜惠縺励↑縺・ｴ蜷医・繧ｫ繝ｪ繝ｳ繧ｰ・・- [ ] Report 菴懈・
