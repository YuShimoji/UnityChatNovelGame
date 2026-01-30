# Task: Performance Baseline Measurement

Status: OPEN
Tier: 2 (Performance)
Branch: feature/perf-baseline
Owner: Worker
Created: 2026-01-30T21:00:00+09:00

## Objective
アプリケーションの基本パフォーマンス（FPS, Memory, Init Time）を計測し、ベースラインを確立する。
将来の最適化（Job System/Burst）の効果を測定するための基準値となる。

## Context
- **Current State**: Core System is fully implemented.
- **Goal**: Measure performance BEFORE optimization.

## Focus Area
- **Tools**: Unity Profiler, Frame Timing Manager.
- **Scenes**: `DebugChatScene` (Auto-play scenario).

## Steps
1. Create a `PerformanceMonitor` script (or use `ProfilerRecorder`).
2. Run `DebugChatScene` in a build (Development Build).
3. Record:
   - Average FPS.
   - GC Alloc per frame (Check for spikes).
   - Heap Memory usage.
4. Save report to `docs/reports/REPORT_TASK_022_PerformanceBaseline.md`.

## DoD (Definition of Done)
- [x] `PerformanceMonitor` script created/used.
- [ ] Baseline Report created.
- [ ] Report includes FPS, Memory, GC Alloc data.
