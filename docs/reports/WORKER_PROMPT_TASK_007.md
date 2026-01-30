# Worker Prompt: TASK_007 Core System Verification

## Request
縺ゅ↑縺溘・ Unity 繧ｯ繝ｩ繧､繧｢繝ｳ繝医お繝ｳ繧ｸ繝九い縺ｨ縺励※縲・*Core System (ChatController, ScenarioManager)** 縺ｮ蜍穂ｽ懈､懆ｨｼ・亥・隧ｦ陦鯉ｼ峨ｒ陦後▲縺ｦ縺上□縺輔＞縲ょ燕蝗槭・讀懆ｨｼ縺ｧ縺ｯ Evidence 縺梧署蜃ｺ縺輔ｌ縺壹√ち繧ｹ繧ｯ縺悟ｮ御ｺ・〒縺阪∪縺帙ｓ縺ｧ縺励◆縲・

## Context
- **Current State**: 繧ｳ繝ｼ繝牙ｮ溯｣・ｮ御ｺ・１hase 1.75 騾夐℃貂医∩縺縺後√ち繧ｹ繧ｯ縺ｯ OPEN 迥ｶ諷九・
- **Goal**: `DebugChatScene` 繧貞ｮ滄圀縺ｫ蜍輔°縺励・*繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧呈署蜃ｺ縺吶ｋ縺薙→**縲・
- **Blocking**: 縺薙ｌ縺悟ｮ御ｺ・＠縺ｪ縺・剞繧翫∵ｬ｡縺ｮ讖溯・螳溯｣・ｼ・eduction Board・峨↓逹謇九〒縺阪↑縺・ｼ・erification First 謌ｦ逡･・峨・

## Focus Area
1. **Scene**: `Assets/Scenes/DebugChatScene.unity` (縺ｪ縺代ｌ縺ｰ菴懈・/Setup)
2. **Script**: `Assets/Resources/Yarn/DebugScript.yarn` (蜈ｨ讖溯・繝・せ繝育畑)
3. **Action**: Unity Editor 縺ｧ Play 縺励∽ｻ･荳九・蜍穂ｽ懊ｒ遒ｺ隱阪☆繧・
   - 繝｡繝・そ繝ｼ繧ｸ陦ｨ遉ｺ (Left/Right)
   - 逕ｻ蜒剰｡ｨ遉ｺ (`<<Image>>`)
   - 蠕・ｩ・(`<<StartWait>>`)
   - 繝ｭ繧ｰ蜃ｺ蜉・(`<<UnlockTopic>>`, `<<Glitch>>`)

## Tasks (Step-by-Step)
1. `Assets/Scenes/DebugChatScene.unity` 繧帝幕縺・(蟄伜惠縺励↑縺・ｴ蜷医・ `Tools > FoundPhone > Setup Debug Scene` 繧定ｩｦ縺吶°縲∵焔蜍穂ｽ懈・)縲・
2. `ScenarioManager` 縺ｫ `Assets/Resources/Yarn/DebugScript.yarn` 繧偵い繧ｿ繝・メ縺吶ｋ縲・
3. Unity Editor 縺ｧ Play 縺吶ｋ縲・
4. 繧ｷ繝翫Μ繧ｪ繧呈怙蠕後∪縺ｧ騾ｲ繧√ｋ縲・
5. **Evidence 蜿門ｾ・(蠢・・**:
   - `Assets/Scripts/Utils/VerificationCapture.cs` 繧偵す繝ｼ繝ｳ蜀・・GameObject・井ｾ・ Camera・峨↓繧｢繧ｿ繝・メ縺吶ｋ縲・
   - `CaptureOnStart` 繧・true 縺ｫ險ｭ螳壹・
   - PlayMode 繧貞ｮ溯｡後＠縲～docs/evidence/` 縺ｫ `Capture_...png` 縺檎函謌舌＆繧後ｋ縺薙→繧堤｢ｺ隱阪☆繧九・
   - 逕滓・縺輔ｌ縺溽判蜒上ｒ險ｼ諡縺ｨ縺励※謗｡逕ｨ縺吶ｋ縲・
6. `docs/tasks/TASK_007_Verification.md` 縺ｮ DoD 繝√ぉ繝・け繝懊ャ繧ｯ繧ｹ繧貞沂繧√ｋ縲・
7. 繝ｬ繝昴・繝・`docs/inbox/REPORT_TASK_007_Verification.md` 繧剃ｽ懈・縺吶ｋ (Evidence 繝代せ繧呈・險・縲・

## Forbidden Area
- 繝励Ο繝繧ｯ繧ｷ繝ｧ繝ｳ繧ｳ繝ｼ繝・(`ChatController.cs` 遲・ 縺ｮ繝ｭ繧ｸ繝・け螟画峩
- 譁ｰ讖溯・縺ｮ霑ｽ蜉

## Output
- `docs/evidence/task007_chat_ui.png`
- `docs/evidence/task007_console.png`
- `docs/inbox/REPORT_TASK_007_Verification.md`
