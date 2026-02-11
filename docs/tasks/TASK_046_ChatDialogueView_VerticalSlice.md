# Task: ChatDialogueView Vertical Slice Integration

Status: IN_PROGRESS
Tier: 2 (Feature)
Branch: main
Owner: Worker
Created: 2026-02-11
Updated: 2026-02-11
Report: docs/reports/REPORT_TASK_046_ChatDialogueView_VerticalSlice.md

## Objective

`ChatDialogueView` 繧堤ｸｦ蛻・ｊ蟆守ｷ壹∈豁｣蠑乗磁邯壹＠縲√ち繧､繝医Ν髢句ｧ九°繧我ｼ夊ｩｱ騾ｲ陦後・蛻・ｲ舌・蠕・ｩ溘′騾壹ｋ迥ｶ諷九ｒ菴懊ｋ縲・
## Milestone

- MG-1: Vertical Slice Completion

## Focus Area

- Assets/Scripts/Core
- Assets/Scripts/UI
- Assets/Resources/Yarn
- Assets/Scenes

## Forbidden Area

- 謗｢邏｢繧ｹ繝ｬ繝・ラ縺ｮ繝ｪ繧ｽ繝ｼ繧ｹ莉墓ｧ倡｢ｺ螳夲ｼ・BD鬆伜沺・・- 繝・・繝樔ｾ晏ｭ俶ｼ泌・縺ｮ螟ｧ隕乗ｨ｡霑ｽ蜉
- Addressables遘ｻ陦・
## Constraints

- Yarn 繧偵す繝翫Μ繧ｪ騾ｲ陦後・豁｣隕丞ｰ守ｷ壹→縺励※謇ｱ縺・- 迚ｩ隱櫁・蜍慕函謌舌Ο繧ｸ繝・け縺ｯ蟆主・縺励↑縺・- 譌｢蟄倥そ繝ｼ繝匁ｩ溯・繧貞｣翫＆縺ｪ縺・ｼ亥ｾ梧婿莠呈鋤繧堤ｶｭ謖・ｼ・
## DoD

- [ ] `ChatDialogueView` 縺ｧ陦瑚｡ｨ遉ｺ縺ｨ驕ｸ謚櫁い陦ｨ遉ｺ縺梧ｩ溯・縺吶ｋ
- [ ] 繧ｿ繧､繝医Ν -> 莨夊ｩｱ -> 蛻・ｲ・-> 蠕・ｩ・縺ｮ騾壹＠蟆守ｷ壹′謌千ｫ九☆繧・- [ ] StartWait/SkipWait 縺ｮ騾ｲ陦悟宛蠕｡縺檎ｴ邯ｻ縺励↑縺・- [ ] 繧ｻ繝ｼ繝・繝ｭ繝ｼ繝峨ｒ謖溘ｓ縺ｧ繧るｲ陦後′邯咏ｶ壹〒縺阪ｋ
- [ ] 螳溯｡檎ｵ先棡縺後Ξ繝昴・繝医↓險倬鹸縺輔ｌ縺ｦ縺・ｋ

## Test Plan

- 繝・せ繝亥ｯｾ雎｡:
  - ChatDialogueView 騾｣謳ｺ
  - Yarn騾ｲ陦鯉ｼ郁｡・驕ｸ謚櫁い/蠕・ｩ滂ｼ・  - Save/Load 蠕ｩ蟶ｰ
- 繝・せ繝育ｨｮ蛻･:
  - EditMode・亥ｿ・ｦ√↑繝ｭ繧ｸ繝・け蜊倅ｽ難ｼ・  - PlayMode・育ｸｦ蛻・ｊ騾壹＠・・  - 繝薙Ν繝画､懆ｨｼ・医お繝ｩ繝ｼ縺ｪ縺暦ｼ・- 譛溷ｾ・ｵ先棡:
  - 荳ｻ隕∝ｰ守ｷ壹〒萓句､悶・騾ｲ陦悟●豁｢縺檎匱逕溘＠縺ｪ縺・  - 繝・せ繝医′蜈ｨ縺ｦ謌仙粥縺励√ン繝ｫ繝峨お繝ｩ繝ｼ縺後↑縺・
## Stop Conditions

- 譌｢蟄・`ScenarioManager` 縺ｨ縺ｮ謨ｴ蜷医′蜿悶ｌ縺夊ｨｭ險亥､画峩縺悟､ｧ縺阪￥縺ｪ繧・- Yarn雉・肇蛛ｴ縺ｮ菫ｮ豁｣縺縺代〒縺ｯ騾ｲ陦御ｸ榊・蜷医′隗｣豸医〒縺阪↑縺・
## Update (2026-02-11)
- ChatDialogueView: line delay cancellation + option cancel cleanup.
- ScenarioManager: StartWait cancel/SkipWait command, wait cancel cleanup, input unlock.
- Yarn variables: use $current_node and $has_topic_* for save/restore.
- Pending: EditMode/PlayMode/Build test execution and results logging.



