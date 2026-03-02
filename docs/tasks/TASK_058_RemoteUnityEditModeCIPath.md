# TASK_058_RemoteUnityEditModeCIPath

Status: IN_PROGRESS

## Objective
Create a remote-safe Unity EditMode CI path so Layer B for `TASK_057` can be closed without relying on manual local test execution.

## Context
- `TASK_056` established repository guards but does not execute Unity tests.
- `TASK_057` added `CharacterDatabase` EditMode coverage, but execution evidence is still missing.
- A dedicated workflow is needed so the next remote run can produce repeatable test results and artifacts.

## Focus Area
- `.github/workflows/unity-editmode-tests.yml`
- `Assets/Scripts/Tests/CoreLogicTests.cs`
- `docs/WORKFLOW_STATE_SSOT.md`
- `docs/HANDOVER.md`
- `docs/MILESTONE_PLAN.md`

## Layer A
- [x] Add a GitHub Actions workflow for Unity EditMode execution.
- [x] Detect whether supported Unity secret sets are configured and emit an explicit skip reason when they are not.
- [x] Record the secret contract and remote close condition in task/report/SSOT artifacts.

## Layer B
- [ ] Observe the first successful remote `unity-editmode-tests` run.

## Definition of Done
- [x] `.github/workflows/unity-editmode-tests.yml` exists and is wired to push / pull_request / workflow_dispatch.
- [x] The workflow distinguishes between "ready to run" and "credentials missing".
- [x] The workflow uploads EditMode artifacts on remote execution.
- [ ] The repository has one passing remote EditMode run recorded.

## Milestone
- LG-1: Production readiness

## Stop Conditions
- Repository secrets required by Unity licensing are not available.
- Remote GitHub Actions execution cannot be observed from the local environment.

## Notes
- Supported secret sets:
  - `UNITY_LICENSE` + `UNITY_EMAIL` + `UNITY_PASSWORD`
  - `UNITY_SERIAL` + `UNITY_EMAIL` + `UNITY_PASSWORD`
