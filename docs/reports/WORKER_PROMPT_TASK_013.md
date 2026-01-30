# Worker Prompt: TASK_011 + TASK_013 Topic System Verification

## Request
縺ゅ↑縺溘・ Unity 繧ｯ繝ｩ繧､繧｢繝ｳ繝医お繝ｳ繧ｸ繝九い縺ｨ縺励※縲・*Topic System (ScriptableObjects, UnlockTopicCommand)** 縺ｮ蜍穂ｽ懈､懆ｨｼ繧定｡後▲縺ｦ縺上□縺輔＞縲・TASK_011 (Asset菴懈・) 縺ｨ TASK_013 (讀懆ｨｼ) 縺後窪vidence 蠕・■縲阪〒蛛懈ｭ｢縺励※縺・∪縺吶・
## Context
- **Current State**: 繧｢繧ｻ繝・ヨ菴懈・貂医∩縲ゅさ繝ｼ繝牙ｮ溯｣・ｸ医∩縲１hase 3 謌ｦ逡･縺ｫ繧医ｊ縲祁erification First縲阪′豎ｺ螳壹・- **Goal**: 繧｢繧ｻ繝・ヨ縺ｮ Inspector 陦ｨ遉ｺ縺ｨ縲√さ繝槭Φ繝牙ｮ溯｡後Ο繧ｰ縺ｮ**繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧呈署蜃ｺ縺吶ｋ縺薙→**縲・- **Blocking**: 縺薙ｌ縺悟ｮ御ｺ・＠縺ｪ縺・剞繧翫．eduction Board (TASK_008/009 -> Renamed to 016/017 for conflict resolution) 縺ｮ螳溯｣・ｒ蜀埼幕縺ｧ縺阪↑縺・・
## Focus Area
1. **Assets**: `Assets/Resources/Topics/*.asset`
2. **Command**: `UnlockTopicCommand.cs` (蜍穂ｽ懃｢ｺ隱阪・縺ｿ)
3. **Action**: 
   - Inspector 縺ｧ縺ｮ逶ｮ隕也｢ｺ隱・   - DebugChatScene 縺ｧ縺ｮ `<<UnlockTopic>>` 繧ｳ繝槭Φ繝牙ｮ溯｡檎｢ｺ隱・
## Tasks (Step-by-Step)
1. **Topic Asset Inspection**:
   - `Assets/Resources/Topics/` 莉･荳九・驕ｩ蠖薙↑繧｢繧ｻ繝・ヨ (萓・ `debug_topic_01.asset`) 繧帝∈謚槭・   - Inspector 縺ｫ繝励Ο繝代ユ繧｣ (ID, Title, Description, Icon) 縺瑚｡ｨ遉ｺ縺輔ｌ縺ｦ縺・ｋ縺狗｢ｺ隱阪・   - **Evidence 蜿門ｾ・(Automated)**:
     - `DebugChatScene` 縺ｫ `VerificationCapture` 繧ｳ繝ｳ繝昴・繝阪Φ繝医′縺ゅｋ縺薙→繧堤｢ｺ隱阪・     - PlayMode 縺ｧ螳溯｡後＠縲～docs/evidence/` 縺ｫ逕ｻ蜒上′逕滓・縺輔ｌ繧九％縺ｨ繧堤｢ｺ隱阪・     - (Optional) `Topic unlocked` 繝ｭ繧ｰ縺檎｢ｺ隱阪〒縺阪ｋ繧ｿ繧､繝溘Φ繧ｰ縺ｧ `VerificationCapture.TriggerCapture()` 繧貞他縺ｶ繧医≧縺ｫ隱ｿ謨ｴ縺励※繧ゅｈ縺・′縲∝渕譛ｬ縺ｯ襍ｷ蜍墓凾繧ｭ繝｣繝励メ繝｣縺ｧ繧ｷ繝ｼ繝ｳ縺悟虚菴懊＠縺ｦ縺・ｋ險ｼ譏弱′縺ゅｌ縺ｰ繧医＠縺ｨ縺吶ｋ・医Ο繧ｰ縺ｯ繝・く繧ｹ繝井ｿ晏ｭ俶ｩ溯・縺後≠繧後・縺昴■繧峨ｒ遒ｺ隱搾ｼ峨・     - 逕滓・縺輔ｌ縺溽判蜒擾ｼ医♀繧医・繝ｭ繧ｰ繝輔ぃ繧､繝ｫ・峨ｒ邏榊刀迚ｩ縺ｨ縺吶ｋ縲・
2. **Command Execution**:
   - `Assets/Scenes/DebugChatScene.unity` 繧・Play 縺吶ｋ (TASK_007 縺ｨ蜷後§繧ｷ繝ｼ繝ｳ縺ｧ蜿ｯ)縲・   - `DebugScript.yarn` 蜀・〒 `<<UnlockTopic "debug_topic_01">>` 縺悟他縺ｰ繧後◆譎ゅ，onsole 縺ｫ `Topic unlocked: debug_topic_01` (縺ｾ縺溘・鬘樔ｼｼ縺ｮ謌仙粥繝ｭ繧ｰ) 縺悟・繧九°遒ｺ隱阪・   - 繧ｨ繝ｩ繝ｼ縺悟・縺ｦ縺・↑縺・％縺ｨ繧堤｢ｺ隱阪・   - **Evidence 蜿門ｾ・(Automated)**:
     - 繧ｳ繝ｳ繧ｽ繝ｼ繝ｫ繝ｭ繧ｰ縺ｯ `VerificationCapture` 縺ｮ `CaptureLogs` 讖溯・縺ｫ繧医ｊ `docs/evidence/Log_*.txt` 縺ｫ菫晏ｭ倥＆繧後ｋ縲・     - 縺薙・繝ｭ繧ｰ繝輔ぃ繧､繝ｫ縺ｫ `Topic unlocked` 縺ｮ險倩ｿｰ縺後≠繧九％縺ｨ繧堤｢ｺ隱阪☆繧九・     - 逕滓・縺輔ｌ縺溘Ο繧ｰ繝輔ぃ繧､繝ｫ繧定ｨｼ諡縺ｨ縺励※謗｡逕ｨ縺吶ｋ縲・
3. **Report**:
   - `docs/tasks/TASK_011_TopicScriptableObjects.md` 縺ｮ DoD 譖ｴ譁ｰ縲・   - `docs/tasks/TASK_013_TopicDataVerification.md` 縺ｮ DoD 譖ｴ譁ｰ縲・   - 繝ｬ繝昴・繝・`docs/inbox/REPORT_TASK_013_TopicDataVerification.md` 繧剃ｽ懈・ (TASK_011 縺ｮ螳御ｺ・ｱ蜻翫ｂ蜈ｼ縺ｭ繧・縲・
## Forbidden Area
- `TopicData.cs` 縺ｮ讒矩螟画峩
- 譁ｰ隕上い繧ｻ繝・ヨ縺ｮ螟ｧ驥丈ｽ懈・

## Output
- `docs/evidence/task011_topic_inspector.png`
- `docs/evidence/task013_unlock_log.png`
- `docs/inbox/REPORT_TASK_013_TopicDataVerification.md`
