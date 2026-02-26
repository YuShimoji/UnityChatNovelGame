# GC Alloc Reduction Report (TASK_025)

- Date: 2026-02-26
- Scene: DebugChatScene
- Platform: WindowsEditor
- Measurement Tool: `Assets/Scripts/Utils/PerformanceMonitor.cs`
- Measurement Condition: 10s duration / 1s sample / same as TASK_022

## Status

- Result: IN_PROGRESS (after measurement updated)

## Baseline (Before)

- Ref: `docs/reports/REPORT_TASK_022_PerformanceBaseline.md`
- GC Alloc: Avg 22 KB/frame, Max 23 KB/frame
- FPS: Avg 184.8
- Memory Used: Avg 336 MB

## Evidence

- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260226_224612.md`
- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260226_222221.md`
- `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260226_222235.md`

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

## Conclusion

- Current after data is recorded successfully and comparable in range to baseline.
- Final close still requires TASK_027 full-playthrough closure and consolidated verdict in TASK_053.
