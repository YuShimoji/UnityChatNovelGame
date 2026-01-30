# Task: Deduction Board Implementation

Status: CLOSED (Merged to TASK_018)
Tier: 2
Branch: feat/deduction-board
Owner: Worker
Created: 2026-01-17T00:35:00+09:00
Report: docs/inbox/REPORT_TASK_008_DeductionBoard.md

## Objective
謗ｨ逅・・繝ｼ繝会ｼ・eduction Board・峨・蝓ｺ譛ｬ繧ｷ繧ｹ繝・Β繧貞ｮ溯｣・☆繧九・繝励Ξ繧､繝､繝ｼ縺瑚ｨｼ諡・・opic・峨ｒ蜿朱寔縺励√◎繧後ｉ繧堤ｵ・∩蜷医ｏ縺帙※譁ｰ縺励＞邨占ｫ厄ｼ・ynthesis・峨ｒ蟆弱″蜃ｺ縺儷I縺ｨ繝ｭ繧ｸ繝・け繧呈ｧ狗ｯ峨☆繧九・
## Context
- `UnlockTopicCommand` 縺ｫ繧医▲縺ｦ隗｣謾ｾ縺輔ｌ縺・`TopicData` 繧貞庄隕門喧縺吶ｋ蝣ｴ謇縺悟ｿ・ｦ√・- `SynthesisRecipe` 縺ｫ蝓ｺ縺･縺・※繝医ヴ繝・け繧貞粋謌舌☆繧区ｩ溯・縺悟ｿ・ｦ√・- Core System (Task 001-006) 縺ｯ螳溯｣・ｸ医∩縲・
## Focus Area
- `Assets/Scripts/UI/DeductionBoard.cs`: 繝懊・繝峨・邂｡逅・け繝ｩ繧ｹ
- `Assets/Prefabs/UI/TopicNode.prefab`: 繝医ヴ繝・け繧定｡ｨ縺儷I隕∫ｴ
- `Assets/Scripts/Core/ScenarioManager.cs` or `Commands`: `UnlockTopic` 譎ゅ↓繝懊・繝峨∈騾夂衍縺吶ｋ莉慕ｵ・∩
- Drag & Drop 謫堺ｽ懊・螳溯｣・(Unity `IDragHandler` 遲・

## Forbidden Area
- 隕九◆逶ｮ縺ｮ驕主ｺｦ縺ｪ菴懊ｊ霎ｼ縺ｿ・医∪縺壹・讖溯・逍朱壹ｒ蜆ｪ蜈茨ｼ・- 譌｢蟄倥・ `ChatController` 縺ｮ遐ｴ螢・
## Constraints
- `TopicData` 縺ｮ `State` (Hidden, Unlocked, Solved) 繧貞渚譏縺吶ｋ縺薙→
- 蜷域・・・ynthesis・画・蜉滓凾縺ｫ譁ｰ縺励＞繝医ヴ繝・け繧定ｧ｣謾ｾ縺吶ｋ縺薙→

## DoD (Definition of Done)
- [x] `DeductionBoard.cs` 縺悟ｮ溯｣・＆繧後※縺・ｋ
- [x] `TopicNode.prefab` 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ (Implemented `TopicCard.cs`)
- [x] `UnlockTopic` 繧ｳ繝槭Φ繝峨ｒ螳溯｡後☆繧九→縲√・繝ｼ繝我ｸ翫↓繝医ヴ繝・け縺瑚｡ｨ遉ｺ縺輔ｌ繧・- [x] 2縺､縺ｮ繝医ヴ繝・け繧貞粋謌舌＠縺ｦ譁ｰ縺励＞繝医ヴ繝・け繧剃ｽ懈・縺ｧ縺阪ｋ・・ynthesisRecipe縺斐→縺ｮ蛻､螳夲ｼ・- [ ] **Evidence**: 繝懊・繝画桃菴懊・GIF/蜍慕判 縺ｾ縺溘・ 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ (Pending User Verification)
- [x] Report 菴懈・
