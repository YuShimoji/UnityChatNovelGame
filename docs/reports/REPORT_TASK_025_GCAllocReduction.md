# GC Alloc Reduction Report (TASK_025)

- **Date**: TBD
- **Scene**: DebugChatScene
- **Platform**: WindowsEditor
- **Measurement Tool**: `Assets/Scripts/Utils/PerformanceMonitor.cs`
- **Measurement Condition**: 10s duration / 1s sample / same as TASK_022

## Status
- **Result**: IN_PROGRESS

## Baseline (Before)
- **Ref**: `docs/reports/REPORT_TASK_022_PerformanceBaseline.md`
- **GC Alloc**: Avg 22 KB/frame, Max 23 KB/frame
- **FPS**: Avg 184.8
- **Memory Used**: Avg 336 MB

## Evidence (Profiler)
- **Top Alloc Sites (1-3)**
  - 1) `PlayerLoop > UpdateScene > Update.ScriptRunDelayed` (GC Alloc: 11.8 KB)
  - 2) TBD
  - 3) TBD

- **Screenshots / Captures**
  - `docs/evidence/TASK_025_3.png`

- **Limitations**
  - This capture shows non-zero `GC Alloc` in CPU Hierarchy, but detailed call stack / metadata was not available in the Profiler UI at capture time.
  - Further narrowing to specific user scripts (e.g., `ChatController`, text generation, DOTween) requires a capture where call stacks or deeper hierarchy attribution is available.

## Changes

### Change 1: Reduce per-call allocations in ChatController AutoScroll
- **File**: `Assets/Scripts/UI/ChatController.cs`
- **Intent**:
  - Avoid per-call allocations by removing `DOVirtual.DelayedCall(..., () => { ... })` lambda allocation.
  - Avoid scheduling duplicates by using `Invoke(nameof(...))` + guard flag.
  - Avoid stacking scroll tweens by killing previous tween before creating new one.
- **Notes**:
  - Behavior should remain identical (no UX/spec changes).

### Change 2: Reduce allocations in choice button onClick handlers
- **File**: `Assets/Scripts/UI/ChatController.cs`
- **Intent**:
  - Avoid lambda capturing for each option button.
  - Use per-button handler component to store index & callback.

## Measurement (After)

| Time (s) | FPS | Reserved (MB) | Used (MB) | GC Alloc (KB/frame) |
|----------|-----|---------------|----------|--------------------|
| 1.0 | TBD | TBD | TBD | TBD |
| 2.0 | TBD | TBD | TBD | TBD |
| 3.0 | TBD | TBD | TBD | TBD |
| 4.0 | TBD | TBD | TBD | TBD |
| 5.0 | TBD | TBD | TBD | TBD |
| 6.0 | TBD | TBD | TBD | TBD |
| 7.0 | TBD | TBD | TBD | TBD |
| 8.0 | TBD | TBD | TBD | TBD |
| 9.0 | TBD | TBD | TBD | TBD |

## Conclusion
- **GC Alloc**: TBD (target: < 22 KB/frame avg)
- **Next**:
  - Fill in profiler top alloc sites and attach evidence.
  - If GC Alloc is unchanged, inspect string allocations in message generation path and DOTween allocations in UI effects.
