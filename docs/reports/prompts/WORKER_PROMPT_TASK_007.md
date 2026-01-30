# Worker Prompt: TASK_007_Verification

## 蜿ら・
- 繝√こ繝・ヨ: docs/tasks/TASK_007_Verification.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- 繝励Ο繧ｸ繧ｧ繧ｯ繝井ｻ墓ｧ・ 譛蛻昴・繝励Ο繝ｳ繝励ヨ・医・繝ｭ繧ｸ繧ｧ繧ｯ繝医Ν繝ｼ繝茨ｼ・
## 蠅・阜

### Focus Area
- `Assets/Scenes/` 驟堺ｸ・ DebugChatScene 縺ｮ菴懈・
- `Assets/Resources/Yarn/` 驟堺ｸ・ DebugScript 縺ｮ菴懈・
- `ChatController` 縺ｨ `ScenarioManager` 縺ｮ騾｣謳ｺ遒ｺ隱・(螳滓ｩ溷虚菴・
- **險ｼ諡菴懈・**: 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ縺ｾ縺溘・蜍慕判縺ｮ謦ｮ蠖ｱ縺ｨ `docs/evidence/` 縺ｸ縺ｮ菫晏ｭ・
### Forbidden Area
- 譌｢蟄倥・ `MainScene` 繧・・繝ｭ繝繧ｯ繧ｷ繝ｧ繝ｳ繧ｳ繝ｼ繝峨・遐ｴ螢顔噪螟画峩
- Core System 縺ｮ繝ｭ繧ｸ繝・け螟画峩・医ヰ繧ｰ菫ｮ豁｣縺ｯ蜿ｯ縺縺後∵悽雉ｪ逧・､画峩縺ｯ荳榊庄・・- 譁ｰ讖溯・縺ｮ霑ｽ蜉・・eduction Board 遲会ｼ・
## Tier / Branch
- Tier: 3・域､懆ｨｼ繝ｻ菫ｮ豁｣・・- Branch: feat/verify-core-system

## DoD
- [ ] `Assets/Scenes/DebugChatScene.unity` 縺御ｽ懈・縺輔ｌ縲∝・逕溷庄閭ｽ縺ｧ縺ゅｋ
- [ ] `Assets/Resources/Yarn/DebugScript.yarn` 縺御ｽ懈・縺輔ｌ縲∽ｻ･荳九・讖溯・繧貞性繧薙〒縺・ｋ
    - [ ] `<<Message>>` (蟾ｦ蜿ｳ蜷ｹ縺榊・縺・
    - [ ] `<<Image>>`
    - [ ] `<<StartWait>>` (繧ｿ繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ)
    - [ ] `<<UnlockTopic>>` (繝ｭ繧ｰ蜃ｺ蜉帷｢ｺ隱・
    - [ ] `<<Glitch>>` (繝ｭ繧ｰ蜃ｺ蜉帷｢ｺ隱・
- [ ] Unity Editor 荳翫〒繧ｨ繝ｩ繝ｼ縺ｪ縺上す繝翫Μ繧ｪ縺梧怙蠕後∪縺ｧ騾ｲ陦後☆繧・- [ ] **Evidence (蠢・・**:
    - [ ] `docs/evidence/task007_chat_ui.png` (繝√Ε繝・ヨ蜍穂ｽ應ｸｭ繧ｹ繧ｯ繧ｷ繝ｧ)
    - [ ] `docs/evidence/task007_console_logs.png` (繧ｳ繝槭Φ繝峨Ο繧ｰ蜃ｺ蜉帙せ繧ｯ繧ｷ繝ｧ)
    - [ ] (莉ｻ諢・ `docs/evidence/task007_demo.mp4`
- [ ] docs/inbox/ 縺ｫ繝ｬ繝昴・繝茨ｼ・EPORT_TASK_007_Verification.md・峨′菴懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## 蛛懈ｭ｢譚｡莉ｶ
- Unity Editor 縺瑚ｵｷ蜍輔〒縺阪↑縺・- 譌｢蟄倥・繧ｳ繝ｳ繝代う繝ｫ繧ｨ繝ｩ繝ｼ縺悟・逋ｺ縺励∬ｧ｣豸井ｸ崎・
- 莉墓ｧ倥・莉ｮ螳壹′ 3 縺､莉･荳雁ｿ・ｦ・- 萓晏ｭ倩ｿｽ蜉/譖ｴ譁ｰ縲∫ｴ螢顔噪Git謫堺ｽ懊；itHubAutoApprove荳肴・縺ｧ縺ｮ push 縺悟ｿ・ｦ・- SSOT荳崎ｶｳ繧・`ensure-ssot.js` 縺ｧ隗｣豎ｺ縺ｧ縺阪↑縺・
蛛懈ｭ｢譎ゅ・莉･荳九ｒ螳滓命・・1. 繝√こ繝・ヨ縺ｮStatus繧達LOCKED縺ｫ譖ｴ譁ｰ
2. 莠句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ繝√こ繝・ヨ譛ｬ譁・↓霑ｽ險・3. docs/inbox/REPORT_TASK_007_Verification.md 繧剃ｽ懈・縺励∝●豁｢逅・罰繧定ｨ倬鹸
4. 繝√こ繝・ヨ縺ｮReport谺・↓繝ｬ繝昴・繝医ヱ繧ｹ繧定ｿｽ險・
## 邏榊刀蜈・- docs/inbox/REPORT_TASK_007_Verification.md

## 螳溯｣・ヲ繝ｳ繝・- `DebugChatScene` 縺ｫ縺ｯ `ChatController` 縺ｨ `ScenarioManager` 縺ｮ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ繧帝・鄂ｮ縺励！nspector縺ｧ驕ｩ蛻・↓蜿ら・繧定ｨｭ螳壹＠縺ｦ縺上□縺輔＞縲・- `TopicData` 繧・`SynthesisRecipe` 縺ｯ繝・せ繝育畑縺ｮ繝繝溘・繝・・繧ｿ (`Assets/Scripts/Data/Test/` 遲・ 繧剃ｸ譎ら噪縺ｫ菴懈・縺励※繧よｧ九＞縺ｾ縺帙ｓ縲・- Evidence 逕ｨ縺ｮ繝・ぅ繝ｬ繧ｯ繝医Μ `docs/evidence/` 縺悟ｭ伜惠縺励↑縺・ｴ蜷医・菴懈・縺励※縺上□縺輔＞縲・
