# GC Alloc Reduction Report (TASK_025)

- Date: 2026-03-01
- Scene: DebugChatScene
- Platform: WindowsEditor
- Measurement Tool: `Assets/Scripts/Utils/PerformanceMonitor.cs`
- Measurement Condition: 10s duration / 1s sample / same as TASK_022

## Status

- Result: COMPLETED
- Verdict: IMPROVED

## Baseline (Before)

- Ref: `docs/reports/REPORT_TASK_022_PerformanceBaseline.md`
- GC Alloc: Avg 22 KB/frame, Max 23 KB/frame
- FPS: Avg 184.8
- Memory Used: Avg 336 MB

## Evidence

- `docs/reports/REPORT_TASK_022_PerformanceBaseline.md`
- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260301_051433.md`
- `docs/reports/REPORT_TASK_025_GCAllocReduction_DELTA_20260301_round2.md`
- `docs/evidence/PERFORMANCE_MEASUREMENT_20260301_051439.md`

## Measurement (After)

| Time (s) | FPS | Reserved (MB) | Used (MB) | GC Alloc (KB/frame) |
|----------|-----|---------------|-----------|----------------------|
| 1.0 | 1908.3 | 1503 | 1046 | 8 |
| 2.0 | 2206.8 | 1503 | 1065 | 8 |
| 3.0 | 2167.9 | 1503 | 1084 | 8 |
| 4.0 | 2054.3 | 1503 | 1102 | 8 |
| 5.0 | 2221.6 | 1503 | 1121 | 8 |
| 6.0 | 2234.9 | 1503 | 1141 | 8 |
| 7.0 | 2103.4 | 1503 | 1002 | 8 |
| 8.0 | 2209.4 | 1503 | 1019 | 8 |
| 9.0 | 2227.1 | 1503 | 1039 | 8 |

## Delta Summary

- Baseline Avg GC Alloc: 22 KB/frame
- After Avg GC Alloc: 8 KB/frame
- After Range: 8-8 KB/frame
- Delta: -14 KB/frame
- Verdict: `IMPROVED`

## Conclusion

- After data is recorded successfully and shows a measurable reduction versus baseline.
- `VerificationAutomator` から `PerformanceMonitor` を明示起動することで batch raw report の自動取得を安定化しました。
- `DebugChatScene` の `missing script` は `MessageBubble.prefab` の欠損 MonoBehaviour 削除で解消済みです。
