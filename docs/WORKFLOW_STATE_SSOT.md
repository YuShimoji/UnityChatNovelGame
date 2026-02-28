# Workflow State SSOT

Last Updated: 2026-03-01
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
- Gate: SG-1 closed / MG-1 performance follow-up

## Active Task Set

- TASK_047: DONE
  - PlayMode / Build 証跡は `docs/evidence/TASK_047/` で検証済み
- TASK_052: COMPLETED
  - `TASK_047` の未達DoDクローズを 2026-02-28 に再確認済み
- TASK_MVP_04: COMPLETED
  - `docs/evidence/MVP_FINAL_VERIFICATION_20260301_015705.md` で 60秒以内 / 分岐A-B / rapid input を確認済み
- TASK_027: COMPLETED
  - `docs/evidence/TASK_027/FULL_PLAYTHROUGH_RESULTS_20260301_015520.md` で通し導線を自動検証済み
- TASK_053: COMPLETED
  - SG-1 クローズに必要な統合更新を 2026-03-01 に反映済み
- TASK_025: IN_PROGRESS
  - After measurement: RECORDED
  - Verdict: `NO_MEASURABLE_REDUCTION` (`+0.22 KB/frame` vs baseline)

## Blocker Registry

- Current blocker: none (delivery-stopping)
- Residual observation: batch evidence capture still emits raw `ReadPixels...` noise in Unity logs
- Residual observation: `DebugChatScene` load log includes one `The referenced script (Unknown) on this Behaviour is missing!` line

## Next Action

- Single Entry: `TASK_025` の Layer A を開始し、GC Alloc 上位発生源の特定と次の改善タスク切り出しを行う。
