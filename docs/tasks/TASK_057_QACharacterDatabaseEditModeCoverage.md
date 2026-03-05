# TASK_057_QACharacterDatabaseEditModeCoverage

Status: IN_PROGRESS

## Objective
Add meaningful EditMode coverage for `CharacterDatabase` so the QA side of LG-1 advances without manual local verification.

## Context
- `AUDIT_03_TESTING` treated `CharacterDatabase / CharacterProfile` as weakly covered.
- The existing tests mainly covered fallback behavior.
- The missing part was the injected-profile path that represents the intended runtime lookup behavior.

## Focus Area
- `Assets/Scripts/Tests/CoreLogicTests.cs`
- `Assets/Scripts/Data/CharacterDatabase.cs`
- `Assets/Scripts/Data/CharacterProfile.cs`

## Layer A
- [x] Reassess the current `CharacterDatabase` coverage.
- [x] Add injected-profile EditMode tests.
- [x] Record the QA slice in task/report/SSOT artifacts.

## Layer B
- [ ] Observe the first passing `unity-editmode-tests` remote run.

## Definition of Done
- [x] `GetProfile` / `IsPlayer` / `GetThemeColor` / `GetDisplayName` injected-profile paths are covered by tests.
- [x] Fallback tests and injected-profile tests are both present.
- [ ] A passing EditMode execution result is captured through `TASK_058`.

## Milestone
- LG-1: Production readiness

## Stop Conditions
- Unity Test Runner cannot be executed locally without adding more manual steps than the current slice allows.
- Remote Unity execution remains unavailable.
