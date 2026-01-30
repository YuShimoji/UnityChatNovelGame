# Task: Performance Baseline

Status: OPEN
Tier: 3
Branch: feature/perf-baseline
Owner: Worker
Created: 2026-01-30T11:15:00+09:00
Report: docs/reports/REPORT_TASK_043_performance_baseline.md

## Objective
繧｢繝励Μ繧ｱ繝ｼ繧ｷ繝ｧ繝ｳ縺ｮ蜍穂ｽ懆ｲ闕ｷ・医Ο繝ｼ繝画凾髢薙：PS縲√Γ繝｢繝ｪ菴ｿ逕ｨ驥擾ｼ峨ｒ險域ｸｬ縺励√・繝ｼ繧ｹ繝ｩ繧､繝ｳ縺ｨ縺励※險倬鹸縺吶ｋ縲・
蟆・擂逧・↑譛驕ｩ蛹悶・蜉ｹ譫懈ｸｬ螳壹↓菴ｿ逕ｨ縺吶ｋ縲・

## Context
- **Current State**: 繧ｳ繧｢讖溯・縺ｮ螳溯｣・′螳御ｺ・＠縲∝虚菴懷庄閭ｽ縺ｪ迥ｶ諷九・
- **Goal**: 荳ｻ隕√Γ繝医Μ繧ｯ繧ｹ繧貞ｮ夐㍼蛹悶☆繧九・
- **Tooling**: Unity ProfilerRecorder API 縺ｾ縺溘・ System.Diagnostics 繧剃ｽｿ逕ｨ縲・

## Focus Area
- `Assets/Scripts/Debug/PerformanceBenchmark.cs` (譁ｰ隕丈ｽ懈・)
- `docs/reports/PERFORMANCE_BASELINE.md` (險域ｸｬ邨先棡)

## Forbidden Area
- 譌｢蟄倥・繧ｲ繝ｼ繝繝ｭ繧ｸ繝・け縺ｮ螟画峩・郁ｨ域ｸｬ逕ｨ繧ｹ繧ｯ繝ｪ繝励ヨ縺ｮ霑ｽ蜉縺ｮ縺ｿ・・

## Steps
1. `Assets/Scripts/Debug/PerformanceBenchmark.cs` 繧剃ｽ懈・縺吶ｋ縲・
   - `Using UnityEngine.Profiling`
   - Start譎ゅ↓譎る俣險域ｸｬ縲ゞpdate縺ｧFPS險域ｸｬ縲＾nDestroy遲峨〒繝ｬ繝昴・繝亥・蜉帙・
2. `DebugChatScene` 縺ｫ `PerformanceBenchmark` 繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ霑ｽ蜉縺吶ｋ・井ｸ譎ら噪縲√∪縺溘・Prefabs蛹厄ｼ峨・
3. 繧ｷ繝ｼ繝ｳ繧貞ｮ溯｡後＠縲∬・蜍慕噪縺ｾ縺溘・繧ｭ繝ｼ蜈･蜉帙〒險域ｸｬ繧堤ｵゆｺ・・繝ｭ繧ｰ菫晏ｭ倥＆縺帙ｋ縲・
4. 逕滓・縺輔ｌ縺溘Ο繧ｰ繧・`docs/reports/` 縺ｫ驟咲ｽｮ縺吶ｋ縲・

## DoD (Definition of Done)
- [ ] `PerformanceBenchmark.cs` 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ
- [ ] 險域ｸｬ縺悟ｮ溯｡後＆繧後∽ｻ･荳九・鬆・岼繧貞性繧繝ｬ繝昴・繝医′菴懈・縺輔ｌ縺ｦ縺・ｋ:
  - [ ] Boot Time (Scene Load ~ Ready)
  - [ ] Average FPS (10s run)
  - [ ] Max Memory Warning (if any)
- [ ] `docs/reports/PERFORMANCE_BASELINE_YYYYMMDD.md` 縺悟ｭ伜惠縺吶ｋ
- [ ] Report 菴懈・ (`docs/reports/REPORT_TASK_043_performance_baseline.md`)
