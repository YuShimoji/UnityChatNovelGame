# Task: ChatController NullReferenceException Fix

Status: DONE
Tier: 2
Branch: feat/fix-chatcontroller-error
Owner: Worker
Created: 2026-01-17T06:30:00+09:00
Report: docs/reports/REPORT_TASK_014_FixChatControllerError.md

## Objective
DebugChatScene縺ｧ逋ｺ逕溘＠縺ｦ縺・ｋChatController縺ｮNullReferenceException繧剃ｿｮ豁｣縺吶ｋ縲・TextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController縺瑚ｪ､縺｣縺ｦ繧｢繧ｿ繝・メ縺輔ｌ縺ｦ縺・ｋ蝠城｡後ｒ隗｣豎ｺ縺励∵ｭ｣縺励＞ProjectFoundPhone.UI.ChatController繧剃ｽｿ逕ｨ縺吶ｋ繧医≧縺ｫ菫ｮ豁｣縺吶ｋ縲・
螳溯｣・ｯｾ雎｡・・1. **DebugChatScene縺ｮ菫ｮ豁｣**: TextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController繧貞炎髯､縺励∵ｭ｣縺励＞ChatController繧定ｨｭ螳・2. **DebugSceneBuilder縺ｮ菫ｮ豁｣**: 繧ｻ繝・ヨ繧｢繝・・譎ゅ↓豁｣縺励＞ChatController繧偵い繧ｿ繝・メ縺吶ｋ繧医≧縺ｫ菫ｮ豁｣

## Context
- DebugChatScene縺ｧ莉･荳九・繧ｨ繝ｩ繝ｼ縺檎匱逕・
  - `ScenarioManager: ChatController not found. Some features may not work.`
  - `NullReferenceException: Object reference not set to an instance of an object ChatController.OnEnable () (at Assets/TextMesh Pro/Examples & Extras/Scripts/ChatController.cs:16)`
- 蜴溷屏: DebugChatScene縺ｮChatRoot縺ｫ縲ゝextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController・・Assets/TextMesh Pro/Examples & Extras/Scripts/ChatController.cs`・峨′隱､縺｣縺ｦ繧｢繧ｿ繝・メ縺輔ｌ縺ｦ縺・ｋ
- 豁｣縺励￥縺ｯ縲～ProjectFoundPhone.UI.ChatController`・・Assets/Scripts/UI/ChatController.cs`・峨ｒ菴ｿ逕ｨ縺吶ｋ蠢・ｦ√′縺ゅｋ
- TextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController縺ｯ縲～ChatInputField`縲～ChatDisplayOutput`縲～ChatScrollbar`縺系ull縺ｮ迥ｶ諷九〒OnEnable縺悟他縺ｰ繧後¨ullReferenceException縺檎匱逕・
## Focus Area
- `Assets/Scenes/DebugChatScene.unity`: ChatController繧ｳ繝ｳ繝昴・繝阪Φ繝医・菫ｮ豁｣
- `Assets/Scripts/Debug/Editor/DebugSceneBuilder.cs`: 繧ｻ繝・ヨ繧｢繝・・譎ゅ↓豁｣縺励＞ChatController繧偵い繧ｿ繝・メ縺吶ｋ繧医≧縺ｫ菫ｮ豁｣

## Forbidden Area
- TextMesh Pro縺ｮ繧ｵ繝ｳ繝励Ν繧ｹ繧ｯ繝ｪ繝励ヨ縺ｮ蜑企勁・域里蟄倥・繧ｵ繝ｳ繝励Ν繧堤ｶｭ謖・ｼ・- ProjectFoundPhone.UI.ChatController縺ｮ繝ｭ繧ｸ繝・け螟画峩・医す繝ｼ繝ｳ縺ｮ菫ｮ豁｣縺ｮ縺ｿ・・- 譁ｰ讖溯・縺ｮ霑ｽ蜉・医お繝ｩ繝ｼ菫ｮ豁｣縺ｮ縺ｿ・・
## Constraints
- DebugChatScene縺ｯ縲ゝools > FoundPhone > Setup Debug Scene縺ｧ蜀咲函謌仙庄閭ｽ縺ｧ縺ゅｋ蠢・ｦ√′縺ゅｋ
- ChatRoot縺ｫ縺ｯ縲∵ｭ｣縺励＞ProjectFoundPhone.UI.ChatController縺後い繧ｿ繝・メ縺輔ｌ縺ｦ縺・ｋ蠢・ｦ√′縺ゅｋ
- TextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController縺ｯ蜑企勁縺吶ｋ縺後√し繝ｳ繝励Ν繧ｹ繧ｯ繝ｪ繝励ヨ繝輔ぃ繧､繝ｫ閾ｪ菴薙・蜑企勁縺励↑縺・
## DoD (Definition of Done)
- [x] DebugChatScene縺ｮChatRoot縺九ｉTextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController繧貞炎髯､
- [x] DebugChatScene縺ｮChatRoot縺ｫ豁｣縺励＞ProjectFoundPhone.UI.ChatController繧偵い繧ｿ繝・メ
- [x] DebugSceneBuilder.cs繧堤｢ｺ隱搾ｼ域里縺ｫ豁｣縺励￥螳溯｣・＆繧後※縺・ｋ縺薙→繧堤｢ｺ隱搾ｼ・- [ ] Tools > FoundPhone > Setup Debug Scene繧貞ｮ溯｡後＠縺ｦ縲∵ｭ｣縺励￥繧ｻ繝・ヨ繧｢繝・・縺輔ｌ繧九％縺ｨ繧堤｢ｺ隱搾ｼ・nity繧ｨ繝・ぅ繧ｿ縺ｧ縺ｮ遒ｺ隱阪′蠢・ｦ・ｼ・- [ ] Play繝懊ち繝ｳ縺ｧ螳溯｡後＠縲¨ullReferenceException縺檎匱逕溘＠縺ｪ縺・％縺ｨ繧堤｢ｺ隱搾ｼ・nity繧ｨ繝・ぅ繧ｿ縺ｧ縺ｮ遒ｺ隱阪′蠢・ｦ・ｼ・- [ ] Console繝ｭ繧ｰ縺ｧ縲靴hatController not found縲阪・隴ｦ蜻翫′陦ｨ遉ｺ縺輔ｌ縺ｪ縺・％縺ｨ繧堤｢ｺ隱搾ｼ・nity繧ｨ繝・ぅ繧ｿ縺ｧ縺ｮ遒ｺ隱阪′蠢・ｦ・ｼ・- [x] `docs/inbox/` 縺ｫ繝ｬ繝昴・繝・(`REPORT_TASK_014_FixChatControllerError.md`) 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [x] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## 菫ｮ豁｣謇矩・
### 1. DebugSceneBuilder.cs縺ｮ菫ｮ豁｣
- `SetupChatController()` 繝｡繧ｽ繝・ラ縺ｧ縲∵ｭ｣縺励＞ChatController繧偵い繧ｿ繝・メ縺吶ｋ繧医≧縺ｫ菫ｮ豁｣
- TextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController縺ｧ縺ｯ縺ｪ縺上～ProjectFoundPhone.UI.ChatController` 繧剃ｽｿ逕ｨ
- 蠢・ｦ√↑繧ｳ繝ｳ繝昴・繝阪Φ繝茨ｼ・crollRect縲〃erticalLayoutGroup遲会ｼ峨ｒ豁｣縺励￥險ｭ螳・
### 2. DebugChatScene.unity縺ｮ菫ｮ豁｣
- ChatRoot縺ｮGameObject縺九ｉ縲ゝextMesh Pro縺ｮ繧ｵ繝ｳ繝励ΝChatController繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ蜑企勁
- ChatRoot縺ｮGameObject縺ｫ縲∵ｭ｣縺励＞ProjectFoundPhone.UI.ChatController繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ霑ｽ蜉
- 蠢・ｦ√↑蜿ら・・・crollRect縲´ayoutGroup縲｀essageBubblePrefab縲ゝypingIndicator・峨ｒ險ｭ螳・
### 3. 蜍穂ｽ懃｢ｺ隱・- Tools > FoundPhone > Setup Debug Scene繧貞ｮ溯｡・- 繧ｷ繝ｼ繝ｳ繧剃ｿ晏ｭ・- Play繝懊ち繝ｳ縺ｧ螳溯｡後＠縲√お繝ｩ繝ｼ縺檎匱逕溘＠縺ｪ縺・％縺ｨ繧堤｢ｺ隱・
## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- TextMesh Pro縺ｮ繧ｵ繝ｳ繝励Ν繧ｹ繧ｯ繝ｪ繝励ヨ縺ｯ縲・xamples & Extras繝輔か繝ｫ繝蜀・↓谿九＠縺ｦ縺翫￥・亥炎髯､縺励↑縺・ｼ・- DebugSceneBuilder縺ｮ菫ｮ豁｣縺ｫ繧医ｊ縲∽ｻ雁ｾ後そ繝・ヨ繧｢繝・・縺吶ｋ髫帙↓豁｣縺励＞ChatController縺瑚・蜍慕噪縺ｫ繧｢繧ｿ繝・メ縺輔ｌ繧・
