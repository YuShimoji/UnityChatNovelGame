# Workflow State SSOT

Last Updated: 2026-02-27
Owner: Orchestrator
Scope: UnityChatNovelGame + shared-workflows integration

## Rules

- This file is the single source of truth for current execution state.
- If this file is stale or empty, update this file before proposing new tasks.
- Do not create additional tasks while `Next Action` is unresolved.
- Measurement tasks must be tracked as two layers:
  - Layer A (AI-completable: instrumentation/setup/docs)
  - Layer B (human-run: Unity manual measurement and evidence capture)

## Current Phase

- Phase: Worker Execution
- Gate: Verification Gate split active (A/B)

## Active Task Set

- TASK_027: IN_PROGRESS
  - Layer A: COMPLETED
  - Layer B: IN_PROGRESS (UI/bubble gate passed, full route evidence pending)
- TASK_053: IN_PROGRESS
  - Dependency: TASK_027 Layer B completion
- TASK_025: IN_PROGRESS
  - After measurement: UPDATED (`REPORT_TASK_022_PerformanceBaseline_RAW_20260227_020005.md` / `...030649.md`)

## Blocker Registry

- Previous blocker: `MessageBubble` missing-script runtime failure
- Current blocker: none (technical)
- Remaining gap: manual full-route evidence not yet captured

## Next Action

- Single Entry: Run TASK_027 Layer B full route and record dated evidence (`FULL_PLAYTHROUGH_RESULTS_*`, `Log_*`, `Capture_*`), then close TASK_053.
