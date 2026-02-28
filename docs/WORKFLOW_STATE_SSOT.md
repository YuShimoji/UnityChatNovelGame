# Workflow State SSOT

Last Updated: 2026-03-01T05:37:17+09:00
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
- Gate: SG-1 closed / MG-1 closed / residual hygiene normalization in progress

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
  - Verdict: `IMPROVED` (`-14 KB/frame` vs baseline, `22 -> 8 KB/frame`)
- TASK_054: COMPLETED
  - Verification automation hardening / missing script source attribution and cleanup are recorded
- TASK_055: COMPLETED
  - generated artifacts were normalized into an orchestrator-owned commit-ready boundary

## Blocker Registry

- Current blocker: none (delivery-stopping)
- Residual observation:
  - unrelated local modifications remain in:
    - `Assets/Font/NotoSansJP-Regular SDF.asset`
    - `Assets/Scripts/Tests/PlayMode/MVPScreenshotEvidencePlayModeTests.cs`
  - one transient log remains locked by another process:
    - `docs/logs/unity_automation_task027_20260301.log`

## Next Action

- Single Entry: orchestrator-owned verification / report / evidence updates を commit boundary として確定し、その後に `TASK_044` / `TASK_046` の再優先付けへ移る
