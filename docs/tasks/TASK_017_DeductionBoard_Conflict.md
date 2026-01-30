# Task: DeductionBoard (謗ｨ隲悶・繝ｼ繝・ 螳溯｣・Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-16T13:55:00Z
Report: docs/reports/REPORT_TASK_009_DeductionBoard.md

## Objective
繝励Ξ繧､繝､繝ｼ縺檎佐蠕励＠縺溘ヨ繝斐ャ繧ｯ(TopicData)繧定｡ｨ遉ｺ繝ｻ邂｡逅・☆繧九梧耳隲悶・繝ｼ繝・DeductionBoard)縲攻I繧貞ｮ溯｣・☆繧九・ScenarioManager縺ｮUnlockTopicCommand縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ縲√ヨ繝斐ャ繧ｯ縺ｮ霑ｽ蜉繝ｻ陦ｨ遉ｺ繧定｡後≧縲・
## Context
- **蜑榊ｷ･遞・*: TASK_002縺ｧUnlockTopicCommand縺ｮ繧ｹ繧ｱ繝ｫ繝医Φ縺悟ｮ溯｣・ｸ医∩(Debug.Log縺ｧ莉｣譖ｿ荳ｭ)
- **萓晏ｭ・*: TopicData ScriptableObject (譌｢縺ｫ螳夂ｾｩ貂医∩)
- **繧ｴ繝ｼ繝ｫ**: 繝医ヴ繝・け縺ｮ隕冶ｦ夂噪邂｡逅・→縲∝ｰ・擂縺ｮ繝医ヴ繝・け蜷域・(SynthesisRecipe)縺ｮ蝓ｺ逶､讒狗ｯ・
## Focus Area
- `Assets/Scripts/UI/DeductionBoard.cs` (譁ｰ隕・
- `Assets/Scripts/UI/TopicCard.cs` (譁ｰ隕上∬｡ｨ遉ｺ逕ｨ繧ｳ繝ｳ繝昴・繝阪Φ繝・
- `Assets/Prefabs/UI/DeductionBoard.prefab` (譁ｰ隕・
- `Assets/Prefabs/UI/TopicCard.prefab` (譁ｰ隕・
- `Assets/Scripts/Core/ScenarioManager.cs` (UnlockTopicCommand騾｣謳ｺ驛ｨ蛻・・縺ｿ)

## Forbidden Area
- `Assets/Scripts/UI/ChatController.cs` 縺ｸ縺ｮ螟画峩
- `Assets/Scripts/Core/` 縺ｮ莉悶・繝輔ぃ繧､繝ｫ縺ｸ縺ｮ螟ｧ蟷・､画峩
- 譌｢蟄倥・Prefab縺ｮ遐ｴ螢顔噪螟画峩

## Constraints
- TopicData縺ｮScriptableObject繧剃ｽｿ逕ｨ縺吶ｋ縺薙→
- UI縺ｯCanvas荳翫↓驟咲ｽｮ蜿ｯ閭ｽ縺ｪ讒矩縺ｫ縺吶ｋ縺薙→
- 繧ｷ繝ｳ繝励Ν縺ｪ繝ｪ繧ｹ繝郁｡ｨ遉ｺ縺九ｉ髢句ｧ・繧ｰ繝ｪ繝・ラ繝ｬ繧､繧｢繧ｦ繝医・蠕檎ｶ壹ち繧ｹ繧ｯ)

## Steps
1. DeductionBoard.cs 縺ｮ蝓ｺ譛ｬ讒矩繧貞ｮ溯｣・繝医ヴ繝・け霑ｽ蜉/蜑企勁/陦ｨ遉ｺ)
2. TopicCard.cs 繧貞ｮ溯｣・TopicData縺ｮ陦ｨ遉ｺ逕ｨ)
3. Prefab繧剃ｽ懈・(ScrollView + GridLayout)
4. ScenarioManager.UnlockTopicCommand縺ｨ騾｣謳ｺ
5. 繝・せ繝医す繝ｼ繝ｳ縺ｧ蜍穂ｽ懃｢ｺ隱・
## DoD (Definition of Done)
- [ ] `DeductionBoard.cs` 縺悟ｮ溯｣・＆繧後√ヨ繝斐ャ繧ｯ縺ｮ霑ｽ蜉繝ｻ陦ｨ遉ｺ縺後〒縺阪ｋ
- [ ] `TopicCard.cs` 縺悟ｮ溯｣・＆繧後ゝopicData縺ｮ諠・ｱ(蜷榊燕縲√い繧､繧ｳ繝ｳ遲・繧定｡ｨ遉ｺ縺ｧ縺阪ｋ
- [ ] `DeductionBoard.prefab` 縺ｨ `TopicCard.prefab` 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] `ScenarioManager.UnlockTopicCommand` 縺九ｉ繝医ヴ繝・け霑ｽ蜉縺悟他縺ｳ蜃ｺ縺帙ｋ
- [ ] Unity Editor縺ｧ蜍穂ｽ懃｢ｺ隱阪′螳御ｺ・＠縺ｦ縺・ｋ
- [ ] `docs/reports/REPORT_TASK_009_DeductionBoard.md` 縺ｫ繝ｬ繝昴・繝医′菴懈・縺輔ｌ縺ｦ縺・ｋ

## 蛛懈ｭ｢譚｡莉ｶ
- TopicData ScriptableObject縺ｮ讒矩螟画峩縺悟ｿ・ｦ√↓縺ｪ縺｣縺溷ｴ蜷・- 譌｢蟄倥・ScenarioManager繝ｭ繧ｸ繝・け縺ｨ縺ｮ遶ｶ蜷医′逋ｺ逕溘＠縺溷ｴ蜷・
## Notes
- TASK_008 (ChatUI Integration) 縺ｨ荳ｦ陦悟ｮ溯｡悟庄閭ｽ
- 蟆・擂逧・↓縺ｯ繝医ヴ繝・け蜷域・(SynthesisRecipe)讖溯・縺ｨ騾｣謳ｺ莠亥ｮ・
