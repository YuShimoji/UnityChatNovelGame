# Task: Synthesis System Implementation

Status: DONE
Tier: 2
Branch: feat/synthesis-system
Owner: Worker
Created: 2026-01-28T23:10:00+09:00
Report: docs/reports/REPORT_TASK_019_Synthesis_Implementation.md

## Objective
DeductionBoard 縺ｮ譬ｸ縺ｨ縺ｪ繧九梧耳隲厄ｼ亥粋謌撰ｼ峨阪す繧ｹ繝・Β繧堤ｨｼ蜒阪＆縺帙ｋ縲・`SynthesisRecipe` ScriptableObject 縺ｮ繧｢繧ｻ繝・ヨ繧剃ｽ懈・縺励～Resources/Recipes` 縺ｫ驟咲ｽｮ縺励※縲．eductionBoard 荳翫〒縺ｮ蜷域・蜍穂ｽ懊ｒ遒ｺ隱阪☆繧九・
## Context
- **Pre-requisites**:
  - `DeductionBoard.cs`: Implemented (TASK_018)
  - `SynthesisRecipe.cs`: Implemented (Data)
  - `TopicData`: Implemented.
- **Missing**:
  - `Assets/Resources/Recipes/` 繝・ぅ繝ｬ繧ｯ繝医Μ
  - 螳滄圀縺ｮ Recipe 繧｢繧ｻ繝・ヨ
- **Goal**: 2縺､縺ｮ險ｼ諡・・opic・峨ｒ邨・∩蜷医ｏ縺帙※譁ｰ縺励＞險ｼ諡繧貞ｾ励ｋ繧ｲ繝ｼ繝繝ｫ繝ｼ繝励ｒ謌千ｫ九＆縺帙ｋ縲・
## Focus Area
- `Assets/Resources/Recipes/` (New Directory & Assets)
- `Assets/Scripts/UI/DeductionBoard.cs` (Logic verification/fixes)
- `Assets/Data/Topics/` (If new test topics are needed)

## Forbidden Area
- `ChatController.cs`
- `UnityEditor` namespace in Runtime scripts

## Constraints
- **Path**: Recipes must be in `Resources/Recipes` so `DeductionBoard.LoadRecipes()` can find them.
- **Data**: Create at least 1 valid recipe pair for testing (e.g., Topic A + Topic B = Topic C).

## Steps
1. Create directory `Assets/Resources/Recipes`.
2. Create dummy `TopicData` if needed (Topic A, B, C) or use existing ones.
3. Create a `SynthesisRecipe` asset (A + B -> C).
4. Run `DebugChatScene` (or similar).
5. Open DeductionBoard, add Topic A and B.
6. Drag A onto B.
7. Verify Topic C appears.
8. Capture Evidence.

## DoD (Definition of Done)
- [ ] `Assets/Resources/Recipes` 繝・ぅ繝ｬ繧ｯ繝医Μ縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] 蟆代↑縺上→繧・縺､縺ｮ蜍穂ｽ懊☆繧・`SynthesisRecipe` 繧｢繧ｻ繝・ヨ縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] **Verification**:
  - [ ] 繝峨Λ繝・げ・・ラ繝ｭ繝・・縺ｧ蜷域・縺梧・蜉溘＠縲∵眠縺励＞繝医ヴ繝・け縺瑚ｧ｣謾ｾ縺輔ｌ繧・  - [ ] 蜷域・邨先棡縺後Ο繧ｰ縺ｫ陦ｨ遉ｺ縺輔ｌ繧・- [ ] Evidence (`docs/evidence/`) 蜿門ｾ・- [ ] Report 菴懈・
