# Worker Prompt: TASK_009_DeductionBoard

縺ゅ↑縺溘・ Worker Agent 縺ｧ縺吶ゆｻ･荳九・謖・､ｺ縺ｫ蠕薙＞縲√ち繧ｹ繧ｯ繧帝≠陦後＠縺ｦ縺上□縺輔＞縲・
Orchestrator 縺ｫ繧医▲縺ｦ螳夂ｾｩ縺輔ｌ縺溷｢・阜(Focus/Forbidden)繧帝・螳医☆繧九％縺ｨ縺梧ｱゅａ繧峨ｌ縺ｾ縺吶・

## 蜿ら・諠・ｱ
- **繝√こ繝・ヨ**: `docs/tasks/TASK_009_DeductionBoard.md` (蠢・ｪｭ)
- **SSOT**: `docs/Windsurf_AI_Collab_Rules_latest.md`
- **HANDOVER**: `docs/HANDOVER.md`
- **Context**: `AI_CONTEXT.md`
- **髢｢騾｣螳溯｣・*: `Assets/Scripts/Core/ScenarioManager.cs` (UnlockTopicCommand)
- **繝・・繧ｿ繝｢繝・Ν**: `Assets/Scripts/Data/TopicData.cs`

## 繝溘ャ繧ｷ繝ｧ繝ｳ
**DeductionBoard (謗ｨ隲悶・繝ｼ繝・ 螳溯｣・*

- `DeductionBoard.cs` 繧呈眠隕丈ｽ懈・縺励√ヨ繝斐ャ繧ｯ縺ｮ霑ｽ蜉繝ｻ陦ｨ遉ｺ繝ｻ邂｡逅・ｩ溯・繧貞ｮ溯｣・☆繧九・
- `TopicCard.cs` 繧呈眠隕丈ｽ懈・縺励ゝopicData縺ｮ隕冶ｦ夂噪陦ｨ遉ｺ繧呈球蠖薙☆繧九・
- 蟇ｾ蠢懊☆繧輝refab (`DeductionBoard.prefab`, `TopicCard.prefab`) 繧剃ｽ懈・縺吶ｋ縲・
- `ScenarioManager.UnlockTopicCommand` 縺九ｉ蜻ｼ縺ｳ蜃ｺ縺帙ｋ繧医≧縺ｫ縺吶ｋ縲・

## 蠅・阜 (Boundaries)

### Focus Area(螟画峩險ｱ蜿ｯ)
- `Assets/Scripts/UI/DeductionBoard.cs` (譁ｰ隕・
- `Assets/Scripts/UI/TopicCard.cs` (譁ｰ隕・
- `Assets/Prefabs/UI/DeductionBoard.prefab` (譁ｰ隕・
- `Assets/Prefabs/UI/TopicCard.prefab` (譁ｰ隕・
- `Assets/Scripts/Core/ScenarioManager.cs` (UnlockTopicCommand騾｣謳ｺ縺ｮ縺ｿ)

### Forbidden Area(螟画峩遖∵ｭ｢)
- `Assets/Scripts/UI/ChatController.cs`
- `Assets/Scripts/Core/` 縺ｮ莉悶・繝輔ぃ繧､繝ｫ縺ｸ縺ｮ螟ｧ蟷・､画峩
- 譌｢蟄榔refab縺ｮ遐ｴ螢顔噪螟画峩

## Definition of Done (DoD)
- [ ] `DeductionBoard.cs` 縺悟ｮ溯｣・＆繧後√ヨ繝斐ャ繧ｯ縺ｮ霑ｽ蜉繝ｻ陦ｨ遉ｺ縺後〒縺阪ｋ
- [ ] `TopicCard.cs` 縺悟ｮ溯｣・＆繧後ゝopicData縺ｮ諠・ｱ繧定｡ｨ遉ｺ縺ｧ縺阪ｋ
- [ ] `DeductionBoard.prefab` 縺ｨ `TopicCard.prefab` 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] `ScenarioManager.UnlockTopicCommand` 縺九ｉ繝医ヴ繝・け霑ｽ蜉縺悟他縺ｳ蜃ｺ縺帙ｋ
- [ ] Unity Editor縺ｧ蜍穂ｽ懃｢ｺ隱阪′螳御ｺ・＠縺ｦ縺・ｋ
- [ ] `docs/reports/REPORT_TASK_009_DeductionBoard.md` 縺ｫ繝ｬ繝昴・繝医′菴懈・縺輔ｌ縺ｦ縺・ｋ

## 蛛懈ｭ｢譚｡莉ｶ (Stop & Report)
- TopicData ScriptableObject縺ｮ讒矩螟画峩縺悟ｿ・ｦ√↓縺ｪ縺｣縺溷ｴ蜷・
- 譌｢蟄倥・ScenarioManager繝ｭ繧ｸ繝・け縺ｨ縺ｮ遶ｶ蜷医′逋ｺ逕溘＠縺溷ｴ蜷・

## 邏榊刀迚ｩ
- 譁ｰ隕丈ｽ懈・縺輔ｌ縺溘さ繝ｼ繝・DeductionBoard.cs, TopicCard.cs)
- 譁ｰ隕襲refab
- `docs/reports/REPORT_TASK_009_DeductionBoard.md`
