# Task: Topic ScriptableObjects Creation

Status: IN_PROGRESS
Tier: 2
Branch: feat/topic-scriptableobjects
Owner: Worker
Created: 2026-01-17T02:00:00+09:00
Report: docs/inbox/REPORT_TASK_011_TopicScriptableObjects.md 

## Objective
`TopicData` ScriptableObject 縺ｮ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ・医い繧ｻ繝・ヨ・峨ｒ菴懈・縺吶ｋ縲・`UnlockTopicCommand` 繧・`DeductionBoard` 縺ｧ菴ｿ逕ｨ縺吶ｋ繝医ヴ繝・け繝・・繧ｿ繧呈ｺ門ｙ縺吶ｋ縲・
## Context
- `TopicData.cs` (ScriptableObject螳夂ｾｩ) 縺ｯ螳溯｣・ｸ医∩・・ASK_001・・- `UnlockTopicCommand` 縺ｯ `Resources.Load<TopicData>($"Topics/{topicID}")` 縺ｧ隱ｭ縺ｿ霎ｼ繧諠ｳ螳・- `DeductionBoard` (TASK_008) 縺ｧ繝医ヴ繝・け繧定｡ｨ遉ｺ縺吶ｋ縺溘ａ縺ｫ縲∝ｮ滄圀縺ｮ繧｢繧ｻ繝・ヨ縺悟ｿ・ｦ・- 繝励Ο繧ｸ繧ｧ繧ｯ繝医・蛻晄悄繧ｷ繝翫Μ繧ｪ縺ｧ菴ｿ逕ｨ縺吶ｋ繝医ヴ繝・け繧貞ｮ夂ｾｩ縺吶ｋ蠢・ｦ√′縺ゅｋ

## Focus Area
- `Assets/Resources/Topics/` 驟堺ｸ・ TopicData 繧｢繧ｻ繝・ヨ縺ｮ菴懈・
- 蛻晄悄繝医ヴ繝・け縺ｮ螳夂ｾｩ・井ｾ・ "Strange Signal", "Missing Person", "Found Phone" 遲会ｼ・- 蜷・ヨ繝斐ャ繧ｯ縺ｮ `State` (Hidden, Unlocked, Solved) 縺ｮ蛻晄悄蛟､險ｭ螳・- `SynthesisRecipe` 縺ｨ縺ｮ髢｢騾｣莉倥￠・亥粋謌舌Ξ繧ｷ繝斐・螳夂ｾｩ・・
## Forbidden Area
- `TopicData.cs` 縺ｮ螳夂ｾｩ螟画峩・域里蟄倥・讒矩繧堤ｶｭ謖・ｼ・- 驕主ｺｦ縺ｪ繝医ヴ繝・け謨ｰ縺ｮ菴懈・・亥・譛溘す繝翫Μ繧ｪ縺ｫ蠢・ｦ√↑譛蟆城剞縺ｮ縺ｿ・・- 譌｢蟄倥・繧｢繧ｻ繝・ヨ縺ｮ遐ｴ螢顔噪螟画峩

## Constraints
- 繝・せ繝・ 荳ｻ隕√ヱ繧ｹ縺ｮ縺ｿ・・esources.Load 縺ｧ縺ｮ隱ｭ縺ｿ霎ｼ縺ｿ遒ｺ隱搾ｼ・- 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: 譁ｰ隕剰ｿｽ蜉遖∵ｭ｢・域里蟄倥・ TopicData 讒矩縺ｫ貅匁侠・・- Unity Editor 縺ｧ縺ｮ謇句虚菴懈・・・criptableObject 縺ｮ CreateAssetMenu 繧剃ｽｿ逕ｨ・・
## DoD (Definition of Done)
- [x] `Assets/Resources/Topics/` 繝・ぅ繝ｬ繧ｯ繝医Μ縺悟ｭ伜惠縺吶ｋ・・縺､縺ｮ繧｢繧ｻ繝・ヨ縺悟ｭ伜惠縺吶ｋ縺溘ａ遒ｺ隱肴ｸ医∩・・- [x] 蛻晄悄繧ｷ繝翫Μ繧ｪ縺ｧ菴ｿ逕ｨ縺吶ｋ繝医ヴ繝・け繧｢繧ｻ繝・ヨ縺・縺､莉･荳贋ｽ懈・縺輔ｌ縺ｦ縺・ｋ・・縺､蟄伜惠・・- [x] 蜷・ヨ繝斐ャ繧ｯ繧｢繧ｻ繝・ヨ縺・`Resources.Load<TopicData>($"Topics/{topicID}")` 縺ｧ隱ｭ縺ｿ霎ｼ繧√ｋ・医ユ繧ｹ繝域・蜉・ 4 succeeded, 0 failed・・- [/] `UnlockTopicCommand` 縺ｧ繝医ヴ繝・け繧定ｧ｣謾ｾ縺ｧ縺阪ｋ・・ode Verified / Pending Runtime Check・・- [ ] `DeductionBoard` (TASK_008螳御ｺ・ｾ・ 縺ｧ繝医ヴ繝・け縺瑚｡ｨ遉ｺ縺ｧ縺阪ｋ
- [ ] **Evidence**: 繝医ヴ繝・け繧｢繧ｻ繝・ヨ縺ｮ Inspector 陦ｨ遉ｺ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ (Pending Manual Action)
- [x] `docs/inbox/` 縺ｫ繝ｬ繝昴・繝・(`REPORT_TASK_013_TopicDataVerification.md`) 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [x] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- 繝医ヴ繝・け縺ｮ蜀・ｮｹ縺ｯ繝励Ο繧ｸ繧ｧ繧ｯ繝医・莉墓ｧ俶嶌・・docs/Core Specification`・峨↓蝓ｺ縺･縺・※螳夂ｾｩ縺吶ｋ縺薙→
- 蟆・擂逧・↓繧ｷ繝翫Μ繧ｪ縺梧僑蠑ｵ縺輔ｌ繧九％縺ｨ繧定・・縺励√ヨ繝斐ャ繧ｯID縺ｮ蜻ｽ蜷崎ｦ丞援繧堤ｵｱ荳縺吶ｋ縺薙→
