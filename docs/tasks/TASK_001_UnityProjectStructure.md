# Task: Unity 繝励Ο繧ｸ繧ｧ繧ｯ繝域ｧ矩縺ｮ謨ｴ逅・

Status: OPEN
Tier: 1
Branch: main
Owner: Worker-1
Created: 2026-01-08T13:55:40Z
Report: 

## Objective
- Unity 繝励Ο繧ｸ繧ｧ繧ｯ繝医・繝・ぅ繝ｬ繧ｯ繝医Μ讒矩繧剃ｻ墓ｧ俶嶌縺ｫ蝓ｺ縺･縺・※謨ｴ逅・＠縲・幕逋ｺ迺ｰ蠅・・蝓ｺ逶､繧呈ｧ狗ｯ峨☆繧・
- MVC繝代ち繝ｼ繝ｳ縺ｫ貅匁侠縺励◆繝輔か繝ｫ繝讒区・繧剃ｽ懈・縺吶ｋ
- 蠢・医ヱ繝・こ繝ｼ繧ｸ・・arn Spinner, DOTween Pro, TextMeshPro・峨・蟆主・貅門ｙ繧定｡後≧
- 繧ｳ繧｢繧ｷ繧ｹ繝・Β螳溯｣・・蜑肴署譚｡莉ｶ繧呈紛縺医ｋ

## Context
- 繝励Ο繧ｸ繧ｧ繧ｯ繝医・ Unity 2022.3 LTS 莉･髯阪ｒ菴ｿ逕ｨ
- 繝√Ε繝・ヨ繝弱・繝ｫ繧ｲ繝ｼ繝・・ost Phone邉ｻ・峨・髢狗匱繧帝幕蟋九☆繧・
- 莉墓ｧ俶嶌・・docs/Core Specification`, `docs/Core System莉墓ｧ俶嶌`・峨↓蝓ｺ縺･縺・◆讒矩縺悟ｿ・ｦ・
- 迴ｾ蝨ｨ縲ゞnity 繝励Ο繧ｸ繧ｧ繧ｯ繝医・繝・ぅ繝ｬ繧ｯ繝医Μ讒矩縺梧悴謨ｴ蛯吶・迥ｶ諷・

## Focus Area
- `Assets/Scripts/` 驟堺ｸ九・繝・ぅ繝ｬ繧ｯ繝医Μ讒矩菴懈・:
  - `Core/` (GameManager, SaveManager, TimeManager遲・
  - `UI/` (ChatController, DeductionBoard, NotificationManager遲・
  - `Data/` (ScriptableObjects螳夂ｾｩ)
  - `Logic/` (Minigames, ExplorationThreads遲・
- `Assets/Resources/` 驟堺ｸ九・繝・ぅ繝ｬ繧ｯ繝医Μ讒矩菴懈・:
  - `Yarn/` (繧ｷ繝翫Μ繧ｪ繝輔ぃ繧､繝ｫ)
  - `Topics/` (Topic ScriptableObjects)
- `Assets/Prefabs/` 驟堺ｸ九・讒矩菴懈・・・I Prefabs逕ｨ・・
- `Assets/Sprites/` 驟堺ｸ九・讒矩菴懈・・・I邏譚千畑・・
- Unity 繝励Ο繧ｸ繧ｧ繧ｯ繝医・蝓ｺ譛ｬ險ｭ螳夲ｼ医Ξ繧､繝､繝ｼ縲√ち繧ｰ縲√す繝ｼ繝ｳ讒区・遲会ｼ・

## Forbidden Area
- 譌｢蟄倥・Unity繝励Ο繧ｸ繧ｧ繧ｯ繝医ヵ繧｡繧､繝ｫ・亥ｭ伜惠縺吶ｋ蝣ｴ蜷茨ｼ峨・遐ｴ螢顔噪螟画峩
- 繧ｳ繧｢繧ｷ繧ｹ繝・Β縺ｮ螳溯｣・ｼ域悽繧ｿ繧ｹ繧ｯ縺ｯ讒矩謨ｴ逅・・縺ｿ・・
- 繧｢繧ｻ繝・ヨ縺ｮ螳溷宛菴懶ｼ域ｧ矩縺ｨ繝励Ξ繝ｼ繧ｹ繝帙Ν繝繝ｼ縺ｮ縺ｿ・・

## Constraints
- 繝・せ繝・ 荳ｻ隕√ヱ繧ｹ縺ｮ縺ｿ・医ョ繧｣繝ｬ繧ｯ繝医Μ讒矩縺ｮ遒ｺ隱阪ゞnity Editor縺ｧ縺ｮ陦ｨ遉ｺ遒ｺ隱搾ｼ・
- 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: 譁ｰ隕剰ｿｽ蜉遖∵ｭ｢・域里蟄倥・讒矩縺後≠繧句ｴ蜷医・遒ｺ隱阪＠縺ｦ縺九ｉ隱ｿ謨ｴ・・
- Unity 2022.3 LTS 莉･髯阪・繝舌・繧ｸ繝ｧ繝ｳ縺ｫ蟇ｾ蠢・
- MVC繝代ち繝ｼ繝ｳ縺ｫ貅匁侠縺励◆讒矩繧堤ｶｭ謖√☆繧九％縺ｨ

## DoD
- [ ] `Assets/Scripts/` 驟堺ｸ九↓ Core/, UI/, Data/, Logic/ 繝・ぅ繝ｬ繧ｯ繝医Μ縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] `Assets/Resources/` 驟堺ｸ九↓ Yarn/, Topics/ 繝・ぅ繝ｬ繧ｯ繝医Μ縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] `Assets/Prefabs/`, `Assets/Sprites/` 縺ｪ縺ｩ縺ｮ蝓ｺ譛ｬ繝・ぅ繝ｬ繧ｯ繝医Μ縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] 蜷・ョ繧｣繝ｬ繧ｯ繝医Μ縺ｫ `.gitkeep` 縺ｾ縺溘・驕ｩ蛻・↑繝励Ξ繝ｼ繧ｹ繝帙Ν繝繝ｼ繝輔ぃ繧､繝ｫ縺碁・鄂ｮ縺輔ｌ縺ｦ縺・ｋ
- [ ] Unity Editor縺ｧ繝励Ο繧ｸ繧ｧ繧ｯ繝域ｧ矩縺梧ｭ｣縺励￥陦ｨ遉ｺ縺輔ｌ繧九％縺ｨ繧堤｢ｺ隱・
- [ ] 莉墓ｧ俶嶌縺ｫ險倩ｼ峨＆繧後◆讒矩隕∽ｻｶ繧呈ｺ縺溘＠縺ｦ縺・ｋ縺薙→繧堤｢ｺ隱・
- [ ] `docs/inbox/` 縺ｫ繝ｬ繝昴・繝茨ｼ・EPORT_TASK_001_*.md・峨′菴懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・
- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・
- Unity 繝励Ο繧ｸ繧ｧ繧ｯ繝医′譌｢縺ｫ蟄伜惠縺吶ｋ蝣ｴ蜷医・縲∵里蟄俶ｧ矩繧堤｢ｺ隱阪＠縺ｦ縺九ｉ隱ｿ謨ｴ縺吶ｋ縺薙→
- 蠢・医ヱ繝・こ繝ｼ繧ｸ縺ｮ蟆主・縺ｯ蛻･繧ｿ繧ｹ繧ｯ縺ｨ縺吶ｋ蜿ｯ閭ｽ諤ｧ縺後≠繧具ｼ域悽繧ｿ繧ｹ繧ｯ縺ｧ縺ｯ讒矩縺ｮ縺ｿ・・
