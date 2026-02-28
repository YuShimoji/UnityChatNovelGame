# Workflow State SSOT

Last Updated: 2026-02-28
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

- TASK_047: DONE
  - PlayMode / Build 証跡は `docs/evidence/TASK_047/` で検証済み
- TASK_052: COMPLETED
  - `TASK_047` の未達DoDクローズを 2026-02-28 に再確認
- TASK_027: IN_PROGRESS
  - Layer A: COMPLETED
  - Layer B: IN_PROGRESS (full route evidence pending only)
- TASK_053: IN_PROGRESS
  - Remaining: `TASK_027` Layer B + `docs/AI_CONTEXT_MVP.md` 最終反映
- TASK_025: IN_PROGRESS
  - After measurement: RECORDED
  - Verdict: no measurable GC reduction yet

## Blocker Registry

- Previous blocker: `MessageBubble` missing-script runtime failure
- Current blocker: none (technical)
- Remaining gap: consolidated manual verification block not yet executed

## Next Action

- Single Entry: Execute the minimum manual verification block for `TASK_027/TASK_053` and save dated evidence (`FULL_PLAYTHROUGH_RESULTS_*`, `Log_*`, `Capture_*`) in `docs/evidence/TASK_027/`, then update `docs/AI_CONTEXT_MVP.md`.
