# Worker Prompt: TASK_022_PerformanceBaseline

## 参照
- チケット: docs/tasks/TASK_022_PerformanceBaseline.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md (or AI_CONTEXT.md if rules missing)
- HANDOVER: docs/HANDOVER.md

## 境界
- Focus Area: DebugChatScene, Scripts/Utils
- Forbidden Area: Core Logic (Non-intrusive measurement)

## DoD
- [ ] `PerformanceMonitor` script created/used.
- [ ] Baseline Report created (docs/reports/REPORT_TASK_022_PerformanceBaseline.md).
- [ ] Report includes FPS, Memory, GC Alloc data.

## 停止条件
- Unity Build Failure
- Tools missing

## 納品先
- docs/inbox/REPORT_TASK_022_PerformanceBaseline.md
