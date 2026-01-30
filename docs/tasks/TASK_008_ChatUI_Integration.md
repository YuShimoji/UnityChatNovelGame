# Task: Chat UI Data Integration
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-16T13:50:00Z
Report: docs/reports/REPORT_TASK_008_ChatUI_Integration.md

## Objective
`ChatController` 縺ｨ `ScenarioManager` (縺ｾ縺溘・繝・・繧ｿ繝ｭ繝ｼ繝繝ｼ) 繧帝｣謳ｺ縺輔○縲∝､夜Κ繝輔ぃ繧､繝ｫ・・SON/CSV/ScriptableObject・峨°繧峨メ繝｣繝・ヨ繝・・繧ｿ繧定ｪｭ縺ｿ霎ｼ縺ｿ縲ゞI縺ｫ陦ｨ遉ｺ縺吶ｋ讖溯・繧貞ｮ溯｣・☆繧九・
縺ｾ縺溘√Θ繝ｼ繧ｶ繝ｼ縺ｮ驕ｸ謚橸ｼ医・繧ｿ繝ｳ謚ｼ荳九↑縺ｩ・峨↓蠢懊§縺ｦ谺｡縺ｮ繝｡繝・そ繝ｼ繧ｸ繧定｡ｨ遉ｺ縺吶ｋ繝輔Ο繝ｼ繧呈ｧ狗ｯ峨☆繧九・

## Context
- **蜑榊ｷ･遞・*: TASK_007縺ｧChat UI縺ｮ隕九◆逶ｮ縺ｨ蝓ｺ譛ｬ蜍穂ｽ懊・螳梧・貂医∩縲・
- **繧ｴ繝ｼ繝ｫ**: 螳夂ｾｩ縺輔ｌ縺溘す繝翫Μ繧ｪ繝・・繧ｿ縺ｫ蝓ｺ縺･縺・※縲√メ繝｣繝・ヨ縺瑚・蜍輔・蟇ｾ隧ｱ逧・↓騾ｲ陦後☆繧九ｈ縺・↓縺吶ｋ縲・
- **蜿ら・**: `ChatController.cs`, `ScenarioManager.cs` (譌｢蟄倥Ο繧ｸ繝・け繧堤｢ｺ隱・

## Focus Area
- `Assets/Scripts/Core/` (繝・・繧ｿ隱ｭ縺ｿ霎ｼ縺ｿ縲・ｲ陦檎ｮ｡逅・
- `Assets/Scripts/UI/ChatController.cs` (螟夜Κ縺九ｉ縺ｮ繝・・繧ｿ豕ｨ蜈･IF)
- `Assets/Resources/Data/` (繝・せ繝育畑繧ｷ繝翫Μ繧ｪ繝・・繧ｿ菴懈・)

## Forbidden Area
- `Assets/Scripts/UI/` 縺ｮ隕九◆逶ｮ縺ｫ髢｢縺吶ｋ螟ｧ蟷・↑螟画峩・域ｩ溯・霑ｽ蜉縺ｯOK・・
- 譌｢蟄倥・莉悶・繧ｷ繧ｹ繝・Β・域爾邏｢繝代・繝医↑縺ｩ・峨∈縺ｮ蠖ｱ髻ｿ

## Constraints
- 繝・・繧ｿ蠖｢蠑上・繝励Ο繧ｸ繧ｧ繧ｯ繝医・讓呎ｺ厄ｼ・SON縺ｾ縺溘・ScriptableObject・峨↓蠕薙≧縲・
- 髱槫酔譛溯ｪｭ縺ｿ霎ｼ縺ｿ縺悟ｿ・ｦ√↑蝣ｴ蜷医・ `UniTask` 縺ｾ縺溘・ `Coroutine` 繧剃ｽｿ逕ｨ縺吶ｋ縲・

## Steps
1. **繝・・繧ｿ螳夂ｾｩ**: 繝√Ε繝・ヨ繧ｷ繝翫Μ繧ｪ逕ｨ縺ｮ繝・・繧ｿ讒矩・・essage, Sender, Delay, Choices遲会ｼ峨ｒ螳夂ｾｩ縺吶ｋ・域里縺ｫ蟄伜惠縺吶ｌ縺ｰ蜀榊茜逕ｨ・峨・
2. **繝ｭ繝ｼ繝繝ｼ螳溯｣・*: `ChatScenarioLoader` 縺ｾ縺溘・ `ScenarioManager` 縺ｫ繝√Ε繝・ヨ繝・・繧ｿ隱ｭ縺ｿ霎ｼ縺ｿ讖溯・繧定ｿｽ蜉縲・
3. **繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ諡｡蠑ｵ**: `ChatController` 縺ｫ `PlayScenario(ScenarioData data)` 縺ｮ繧医≧縺ｪ繝｡繧ｽ繝・ラ繧定ｿｽ蜉縲・
4. **蛻・ｲ舌Ο繧ｸ繝・け**: 驕ｸ謚櫁い・医・繧ｿ繝ｳ・峨′陦ｨ遉ｺ縺輔ｌ縲・∈謚槭↓繧医▲縺ｦ螻暮幕縺悟､峨ｏ繧倶ｻ慕ｵ・∩繧貞ｮ溯｣・ｼ医∪縺溘・譌｢蟄伜茜逕ｨ・峨・
5. **邨仙粋繝・せ繝・*: `ChatDevScene` 縺ｧ繧ｷ繝翫Μ繧ｪ繝・・繧ｿ繧呈ｵ√＠霎ｼ縺ｿ縲∽ｸ騾｣縺ｮ莨夊ｩｱ縺梧・遶九☆繧九％縺ｨ繧堤｢ｺ隱阪・

## DoD (Definition of Done)
- [x] 繝√Ε繝・ヨ繧ｷ繝翫Μ繧ｪ繝・・繧ｿ・医ユ繧ｹ繝育畑蜷ｫ繧・峨′螳夂ｾｩ縺輔ｌ縺ｦ縺・ｋ
- [x] 繧ｷ繝翫Μ繧ｪ繝・・繧ｿ繧定ｪｭ縺ｿ霎ｼ縺ｿ縲～ChatController` 縺ｧ鬆・ｬ｡陦ｨ遉ｺ縺ｧ縺阪ｋ
- [x] 逶ｸ謇句・縺ｮ繝｡繝・そ繝ｼ繧ｸ縺ｫ縺ｯ驕ｩ蛻・↑繝・ぅ繝ｬ繧､・・yping貍泌・・峨′蜈･繧・
- [x] 繝ｦ繝ｼ繧ｶ繝ｼ蜈･蜉幢ｼ磯∈謚櫁い縺ｾ縺溘・閾ｪ逕ｱ蜈･蜉幢ｼ牙ｾ・■縺ｮ迥ｶ諷九ｒ菴懊ｌ繧・
- [x] **Evidence**: Implement automated verification using `VerificationCapture` (TASK_016).
  - Screenshots of Chat UI in action saved to `docs/evidence/`.
- [x] `docs/reports/REPORT_TASK_008_ChatUI_Integration.md` 縺ｫ蜍穂ｽ懃｢ｺ隱阪・繝ｭ繧ｰ/繧ｹ繧ｯ繧ｷ繝ｧ縺後≠繧・
