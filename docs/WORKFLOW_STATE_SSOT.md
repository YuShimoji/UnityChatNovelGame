# Workflow State SSOT

Last Updated: 2026-03-01T16:21:46+09:00
Owner: Orchestrator
Scope: UnityChatNovelGame + shared-workflows integration

## Rules

- This file is the single source of truth for current execution state.
- If this file is stale or empty, update this file before proposing new tasks.
- Do not create additional tasks while `Next Action` is unresolved.
- Measurement tasks must be tracked as two layers:
  - Layer A (AI-completable: instrumentation/setup/docs)
  - Layer B (human-run or remote-run verification)

## Current Phase

- Phase: Worker Execution
- Gate: SG-1 closed / MG-1 closed / LG-1 entry slice in progress

## Active Task Set

- TASK_047: DONE
  - PlayMode / Build evidence is recorded under `docs/evidence/TASK_047/`
- TASK_052: COMPLETED
  - TASK_047 closure completed on 2026-02-28
- TASK_MVP_04: COMPLETED
  - `docs/evidence/MVP_FINAL_VERIFICATION_20260301_033904.md`
- TASK_027: COMPLETED
  - `docs/evidence/TASK_027/FULL_PLAYTHROUGH_RESULTS_20260301_034145.md`
- TASK_053: COMPLETED
  - SG-1 report integration closed on 2026-03-01
- TASK_025: COMPLETED
  - After measurement: `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_20260301_051433.md`
  - Verdict: `IMPROVED` (`-14 KB/frame`, `22 -> 8 KB/frame`)
- TASK_054: COMPLETED
  - Verification automation hardening / missing script source attribution and cleanup are recorded
- TASK_055: COMPLETED
  - Generated artifacts were normalized into an orchestrator-owned commit-ready boundary
- Legacy task normalization: COMPLETED
  - `TASK_001`, `TASK_010`, `TASK_011`, `TASK_013`, `TASK_029`, `TASK_044`, `TASK_046`
  - `TASK_MVP_02`, `TASK_MVP_03`, `TASK_MVP_04`
- TASK_056: IN_PROGRESS
  - LG-1 entry slice
  - Layer A: completed
  - Layer B: waiting for first remote GitHub Actions green run

## Blocker Registry

- Current blocker: none (delivery-stopping)
- Residual observation:
  - unrelated local modifications remain in:
    - `Assets/Font/NotoSansJP-Regular SDF.asset`
    - `Assets/Scripts/Tests/PlayMode/MVPScreenshotEvidencePlayModeTests.cs`
  - one transient log remains locked by another process:
    - `docs/logs/unity_automation_task027_20260301.log`
  - `TASK_056` Layer B depends on remote workflow execution after a future push

## Next Action

- Single Entry: `TASK_056` の Layer B を除く準備を固定した状態で、LG-1 の次候補を QA slice として切るべきかを audit ベースで確定する
