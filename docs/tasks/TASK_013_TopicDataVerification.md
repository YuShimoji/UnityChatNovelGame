# Task: TopicData Verification and Evidence Collection

Status: IN_PROGRESS
Tier: 3
Branch: feat/topic-verification
Owner: Worker
Created: 2026-01-17T06:00:00+09:00
Report: docs/inbox/REPORT_TASK_013_TopicDataVerification.md

## Objective
TASK_011縺ｧ菴懈・縺励◆TopicData繧｢繧ｻ繝・ヨ縺ｮ蜍穂ｽ懃｢ｺ隱阪→Evidence蜿門ｾ励ｒ陦後≧縲・`UnlockTopicCommand` 縺梧ｭ｣蟶ｸ縺ｫ蜍穂ｽ懊☆繧九％縺ｨ繧堤｢ｺ隱阪＠縲√ヨ繝斐ャ繧ｯ繧｢繧ｻ繝・ヨ縺ｮInspector陦ｨ遉ｺ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕励☆繧九・
螳溯｣・ｯｾ雎｡・・1. **Evidence蜿門ｾ・*: 繝医ヴ繝・け繧｢繧ｻ繝・ヨ縺ｮInspector陦ｨ遉ｺ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ
2. **UnlockTopicCommand遒ｺ隱・*: DebugScript.yarn縺ｧ繝医ヴ繝・け隗｣謾ｾ繧堤｢ｺ隱・
## Context
- TASK_011縺ｧTopicData繧｢繧ｻ繝・ヨ縺ｮ菴懈・縺ｨResources.Load繝・せ繝医・螳御ｺ・- 4縺､縺ｮ繝医ヴ繝・け繧｢繧ｻ繝・ヨ縺梧ｭ｣蟶ｸ縺ｫ隱ｭ縺ｿ霎ｼ縺ｾ繧後ｋ縺薙→繧堤｢ｺ隱肴ｸ医∩・・ succeeded, 0 failed・・- 谿九ｊ縺ｮ菴懈･ｭ: Evidence蜿門ｾ励→UnlockTopicCommand縺ｮ蜍穂ｽ懃｢ｺ隱・- 繝励Ο繧ｸ繧ｧ繧ｯ繝医↓縺ｯ `MCPForUnity.Runtime.Helpers.ScreenshotUtility` 縺悟ｭ伜惠縺励√せ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ蜿門ｾ励′蜿ｯ閭ｽ

## Focus Area
- `docs/evidence/` 驟堺ｸ・ 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ縺ｮ菫晏ｭ・- `Assets/Scenes/DebugChatScene.unity`: UnlockTopicCommand縺ｮ蜍穂ｽ懃｢ｺ隱・- `Assets/Resources/Yarn/DebugScript.yarn`: 繝医ヴ繝・け隗｣謾ｾ繧ｳ繝槭Φ繝峨・螳溯｡檎｢ｺ隱・- Unity Editor蜀・〒縺ｮ蜍穂ｽ懃｢ｺ隱阪→Evidence蜿門ｾ・
## Forbidden Area
- TopicData繧｢繧ｻ繝・ヨ縺ｮ螟画峩・域里蟄倥・繧｢繧ｻ繝・ヨ繧堤ｶｭ謖・ｼ・- UnlockTopicCommand縺ｮ繝ｭ繧ｸ繝・け螟画峩・亥虚菴懃｢ｺ隱阪・縺ｿ・・- 譁ｰ讖溯・縺ｮ霑ｽ蜉・域､懆ｨｼ縺ｮ縺ｿ・・
## Constraints
- 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ蜿門ｾ・ Unity Editor蜀・〒謇句虚蜿門ｾ励√∪縺溘・ `ScreenshotUtility` 繧剃ｽｿ逕ｨ
- Evidence菫晏ｭ伜・: `docs/evidence/task011_topic_assets.png`
- UnlockTopicCommand遒ｺ隱・ DebugScript.yarn縺ｧ `<<UnlockTopic "debug_topic_01">>` 繧貞ｮ溯｡・- 蜍穂ｽ懃｢ｺ隱・ Console繝ｭ繧ｰ縺ｧ縲卦opic unlocked: debug_topic_01縲阪′陦ｨ遉ｺ縺輔ｌ繧九％縺ｨ繧堤｢ｺ隱・
## DoD (Definition of Done)
- [/] 繝医ヴ繝・け繧｢繧ｻ繝・ヨ縺ｮInspector陦ｨ遉ｺ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕・(Pending Manual Action)
  - [ ] `Assets/Resources/Topics/debug_topic_01.asset` 繧帝∈謚・  - [ ] Inspector繧ｦ繧｣繝ｳ繝峨え縺ｧ繝医ヴ繝・け諠・ｱ繧定｡ｨ遉ｺ
  - [ ] 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧・`docs/evidence/task011_topic_assets.png` 縺ｨ縺励※菫晏ｭ・- [/] UnlockTopicCommand縺ｮ蜍穂ｽ懃｢ｺ隱・(Code Verified / Pending Runtime Check)
  - [ ] `Assets/Scenes/DebugChatScene.unity` 繧帝幕縺・  - [ ] `ScenarioManager` 縺・`DebugScript.yarn` 繧貞ｮ溯｡後☆繧九ｈ縺・↓險ｭ螳・  - [ ] Play繝懊ち繝ｳ縺ｧ螳溯｡後＠縲～<<UnlockTopic "debug_topic_01">>` 繧ｳ繝槭Φ繝峨′豁｣蟶ｸ縺ｫ蜍穂ｽ懊☆繧九％縺ｨ繧堤｢ｺ隱・  - [ ] Console繧ｦ繧｣繝ｳ繝峨え縺ｫ縲卦opic unlocked: debug_topic_01縲阪・繝ｭ繧ｰ縺瑚｡ｨ遉ｺ縺輔ｌ繧九％縺ｨ繧堤｢ｺ隱・  - [ ] 繧ｨ繝ｩ繝ｼ縺檎匱逕溘＠縺ｪ縺・％縺ｨ繧堤｢ｺ隱・(Static Check Passed)
- [/] TASK_011縺ｮStatus繧奪ONE縺ｫ譖ｴ譁ｰ・・vidence蜿門ｾ励→UnlockTopicCommand遒ｺ隱榊ｮ御ｺ・ｾ鯉ｼ・- [x] `docs/inbox/` 縺ｫ繝ｬ繝昴・繝・(`REPORT_TASK_013_TopicDataVerification.md`) 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [x] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ蜿門ｾ玲婿豕・
### 譁ｹ豕・: Unity Editor蜀・〒謇句虚蜿門ｾ暦ｼ域耳螂ｨ・・1. Unity Editor縺ｧ `Assets/Resources/Topics/debug_topic_01.asset` 繧帝∈謚・2. Inspector繧ｦ繧｣繝ｳ繝峨え縺ｧ繝医ヴ繝・け諠・ｱ繧定｡ｨ遉ｺ
3. Windows: `Win + Shift + S` 縺ｧ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕・4. `docs/evidence/task011_topic_assets.png` 縺ｨ縺励※菫晏ｭ・
### 譁ｹ豕・: ScreenshotUtility繧剃ｽｿ逕ｨ・・nity Editor蜀・〒螳溯｡鯉ｼ・繝励Ο繧ｸ繧ｧ繧ｯ繝医↓縺ｯ `MCPForUnity.Runtime.Helpers.ScreenshotUtility` 縺悟ｭ伜惠縺励∪縺吶′縲√％繧後・Unity Editor蜀・〒螳溯｡後☆繧句ｿ・ｦ√′縺ゅｊ縺ｾ縺吶・繧ｨ繝・ぅ繧ｿ繧ｹ繧ｯ繝ｪ繝励ヨ繧剃ｽ懈・縺励※縲！nspector陦ｨ遉ｺ縺ｮ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧定・蜍募叙蠕励☆繧九％縺ｨ繧ょ庄閭ｽ縺ｧ縺吶・
## UnlockTopicCommand遒ｺ隱肴焔鬆・
1. **繧ｷ繝ｼ繝ｳ縺ｮ貅門ｙ**
   - Unity Editor縺ｧ `Assets/Scenes/DebugChatScene.unity` 繧帝幕縺・   - `ScenarioManager` 繧ｳ繝ｳ繝昴・繝阪Φ繝医′ `DebugScript.yarn` 繧貞盾辣ｧ縺励※縺・ｋ縺薙→繧堤｢ｺ隱・
2. **螳溯｡後→遒ｺ隱・*
   - Play繝懊ち繝ｳ繧呈款縺励※繧ｷ繝ｼ繝ｳ繧貞ｮ溯｡・   - 繧ｷ繝翫Μ繧ｪ縺碁ｲ陦後＠縲～<<UnlockTopic "debug_topic_01">>` 繧ｳ繝槭Φ繝峨′螳溯｡後＆繧後ｋ繧ｿ繧､繝溘Φ繧ｰ縺ｧ莉･荳九ｒ遒ｺ隱・
     - Console繧ｦ繧｣繝ｳ繝峨え縺ｫ `Topic unlocked: debug_topic_01` 縺ｮ繝ｭ繧ｰ縺瑚｡ｨ遉ｺ縺輔ｌ繧・     - 繧ｨ繝ｩ繝ｼ縺檎匱逕溘＠縺ｪ縺・     - 繝医ヴ繝・け縺梧ｭ｣蟶ｸ縺ｫ隱ｭ縺ｿ霎ｼ縺ｾ繧後ｋ・・esources.Load縺梧・蜉溘☆繧具ｼ・
3. **繝ｭ繧ｰ遒ｺ隱・*
   - Console繧ｦ繧｣繝ｳ繝峨え縺ｧ莉･荳九・繝ｭ繧ｰ繧堤｢ｺ隱・
     ```
     Topic unlocked: debug_topic_01
     ```
   - 繧ｨ繝ｩ繝ｼ繝ｭ繧ｰ縺瑚｡ｨ遉ｺ縺輔ｌ縺ｪ縺・％縺ｨ繧堤｢ｺ隱・
## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- Evidence蜿門ｾ励・謇句虚縺ｧ陦後≧蠢・ｦ√′縺ゅｊ縺ｾ縺呻ｼ・nity Editor蜀・〒縺ｮ謫堺ｽ懊′蠢・ｦ・ｼ・- UnlockTopicCommand遒ｺ隱榊ｾ後ゝASK_011縺ｮStatus繧奪ONE縺ｫ譖ｴ譁ｰ縺吶ｋ縺薙→
