# Worker Prompt for TASK_043: Performance Benchmark

## 萓晞ｼ蜀・ｮｹ
莉･荳九・謇矩・↓蠕薙＞縲√い繝励Μ繧ｱ繝ｼ繧ｷ繝ｧ繝ｳ縺ｮ繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ繝吶・繧ｹ繝ｩ繧､繝ｳ・医Ο繝ｼ繝画凾髢薙：PS縲√Γ繝｢繝ｪ・峨ｒ險域ｸｬ縺励※縺上□縺輔＞縲・

## 繧ｳ繝ｳ繝・く繧ｹ繝・
- 繝励Ο繧ｸ繧ｧ繧ｯ繝・ UnityChatNovelGame
- 逶ｮ逧・ 譛驕ｩ蛹門燕縺ｮ迴ｾ迥ｶ・・aseline・峨ｒ謨ｰ蛟､蛹悶☆繧九・
- 繧ｿ繝ｼ繧ｲ繝・ヨ: `DebugChatScene`

## 謇矩・
1. `Assets/Scripts/Debug/PerformanceBenchmark.cs` 繧剃ｽ懈・縺吶ｋ縲・
   - `Using UnityEngine.Profiling;` 繧剃ｽｿ逕ｨ縲・
   - `Awake` 縺ｧ `Stopwatch` 繧帝幕蟋九・
   - `Start` 縺ｧ繝ｭ繝ｼ繝画凾髢薙ｒ險域ｸｬ(`Stopwatch.ElapsedMilliseconds`)縲・
   - `Update` 縺ｧ FPS 繧定ｨ域ｸｬ (Time.deltaTime 縺ｮ蟷ｳ蝮・縲・
   - `CapturedMetrics` 繧ｯ繝ｩ繧ｹ (struct) 縺ｫ繝・・繧ｿ繧剃ｿ晄戟縲・
2. Unity Editor 縺ｧ `DebugChatScene` 繧帝幕縺阪∫ｩｺ縺ｮ GameObject 縺ｫ `PerformanceBenchmark` 繧偵い繧ｿ繝・メ縺吶ｋ縲・
3. Play 繝｢繝ｼ繝峨ｒ螳溯｡後＠縲∫ｴ・0遘帝俣蠕・ｩ滂ｼ医∪縺溘・閾ｪ蜍慕ｵゆｺ・ｼ峨＆縺帙※險域ｸｬ縺吶ｋ縲・
4. 險域ｸｬ邨先棡繧・Markdown 蠖｢蠑上〒 `docs/reports/PERFORMANCE_BASELINE_{YYYYMMDD}.md` 縺ｫ蜃ｺ蜉帙☆繧九・
   - 鬆・岼: Date, BootTime(ms), AvgFPS, MemoryUsed(MB).

## 遖∵ｭ｢莠矩・
- 譌｢蟄倥・ `ChatController` 繧・`ScenarioManager` 繧貞､画峩縺励↑縺・・
- 險域ｸｬ逕ｨ繧ｪ繝悶ず繧ｧ繧ｯ繝医ｒ Commit 縺ｫ蜷ｫ繧√↑縺・ｼ・cript縺ｮ縺ｿ Commit 縺吶ｋ縲？ierarchy螟画峩縺ｯ Revert 縺吶ｋ縲√∪縺溘・蟆ら畑繧ｷ繝ｼ繝ｳ繧剃ｽ懊ｋ・峨・
  - 謗ｨ螂ｨ: 險域ｸｬ逕ｨ繧ｷ繝ｼ繝ｳ `Assets/Scenes/DebugPerformance.unity` 繧呈眠隕丈ｽ懈・縺励※繧ゅｈ縺・・

## 謠仙・迚ｩ
- `Assets/Scripts/Debug/PerformanceBenchmark.cs`
- `docs/reports/PERFORMANCE_BASELINE_*.md`
- `docs/reports/REPORT_TASK_043_performance_baseline.md` (Worker Report)
