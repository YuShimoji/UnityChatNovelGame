# Workflow State SSOT

Last Updated: 2026-03-01T16:29:51+09:00
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
- Gate: SG-1 closed / MG-1 closed / LG-1 entry slices in progress

## Active Task Set

- TASK_047: DONE
- TASK_052: COMPLETED
- TASK_MVP_04: COMPLETED
- TASK_027: COMPLETED
- TASK_053: COMPLETED
- TASK_025: COMPLETED
- TASK_054: COMPLETED
- TASK_055: COMPLETED
- Legacy task normalization: COMPLETED
- TASK_056: IN_PROGRESS
  - CI baseline
  - Layer A: completed
  - Layer B: waiting for first remote `repo-guards` green run
- TASK_057: IN_PROGRESS
  - QA CharacterDatabase EditMode coverage
  - Layer A: completed
  - Layer B: waiting for first `unity-editmode-tests` pass
- TASK_058: IN_PROGRESS
  - Remote Unity EditMode CI path
  - Layer A: completed
  - Layer B: waiting for first remote run with Unity credentials configured

## Blocker Registry

- Current blocker: none (delivery-stopping)
- Residual observation:
  - unrelated local modifications remain in:
    - `Assets/Font/NotoSansJP-Regular SDF.asset`
    - `Assets/Scripts/Tests/PlayMode/MVPScreenshotEvidencePlayModeTests.cs`
  - one transient log remains locked by another process:
    - `docs/logs/unity_automation_task027_20260301.log`
  - `TASK_056`, `TASK_057`, and `TASK_058` Layer B depend on remote execution
  - `TASK_058` requires one of the following secret sets:
    - `UNITY_LICENSE` + `UNITY_EMAIL` + `UNITY_PASSWORD`
    - or `UNITY_SERIAL` + `UNITY_EMAIL` + `UNITY_PASSWORD`

## Next Action

- Single Entry: keep the local boundary clean and observe the first remote `repo-guards` plus `unity-editmode-tests` runs needed to close Layer B for `TASK_056` / `TASK_057` / `TASK_058`
