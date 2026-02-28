# GC Alloc Reduction Report (TASK_025)

- Date: 2026-03-01
- Scene: DebugChatScene
- Platform: WindowsEditor
- Measurement Tool: `Assets/Scripts/Utils/PerformanceMonitor.cs`
- Measurement Condition: 10s duration / 1s sample / same as TASK_022

## Status

- Result: IN_PROGRESS
- Verdict: NO_MEASURABLE_REDUCTION

## Baseline (Before)

- Ref: `docs/reports/REPORT_TASK_022_PerformanceBaseline.md`
- GC Alloc: Avg 22 KB/frame, Max 23 KB/frame
- FPS: Avg 184.8
- Memory Used: Avg 336 MB

## Evidence

- `docs/reports/REPORT_TASK_022_PerformanceBaseline.md`
- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260226_224612.md`
- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260226_222221.md`
- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260226_222235.md`
- `docs/reports/REPORT_TASK_025_GCAllocReduction_DELTA_20260301.md`

## Measurement (After)

| Time (s) | FPS | Reserved (MB) | Used (MB) | GC Alloc (KB/frame) |
|----------|-----|---------------|-----------|----------------------|
| 1.0 | 170.6 | 1807 | 1225 | 22 |
| 2.0 | 161.7 | 1807 | 1227 | 22 |
| 3.0 | 172.6 | 1807 | 1230 | 23 |
| 4.0 | 199.9 | 1807 | 1233 | 22 |
| 5.0 | 196.0 | 1807 | 1237 | 22 |
| 6.0 | 221.7 | 1807 | 1242 | 22 |
| 7.0 | 231.3 | 1807 | 1247 | 23 |
| 8.0 | 218.6 | 1807 | 1252 | 22 |
| 9.0 | 222.1 | 1807 | 1257 | 22 |

## Delta Summary

- Baseline Avg GC Alloc: 22 KB/frame
- After Avg GC Alloc: 22.22 KB/frame
- After Range: 22-23 KB/frame
- Delta: +0.22 KB/frame
- Verdict: `NO_MEASURABLE_REDUCTION`

## Conclusion

- After data is recorded successfully and remains directly comparable to baseline
- GC Alloc は同一帯域に留まり、現時点では低減を主張できない
- TASK_025 は `IN_PROGRESS` を維持し、次のステップは alloc source attribution と追加改善施策の切り出しとする
