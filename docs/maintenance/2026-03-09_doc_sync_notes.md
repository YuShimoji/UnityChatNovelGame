# Doc Maintenance Sync Notes (2026-03-09)

## Why this note exists
Some existing docs appear to contain stale structure examples and partially outdated operational checklists. This note records the minimum sync actions to keep implementation and docs aligned.

## Drift Detected
1. StorySpec README contains legacy repository tree examples (`/src`, `/assets`, `/tools`) that do not reflect the current Unity project layout.
2. WORKFLOW SSOT has a checklist item around `m_AutoStartYarn` that conflicts with current implementation intent:
   - Runtime default in `ScenarioManager` is `false`
   - Setup scripts may explicitly set it `true` for quick preview scenes
3. SSOT references docs folders that may not exist in all branches (`docs/reports`, `docs/tasks`, etc.).

## Canonical Clarification (proposed)
- `ScenarioManager.m_AutoStartYarn`
  - Default: `false` (safe runtime behavior)
  - Setup override: allowed for debug/content-authoring preview scenes only
- Repository structure for docs should reference Unity directories first:
  - `Assets/`, `Packages/`, `ProjectSettings/`, `docs/`, `scripts/`

## Maintenance Actions queued
1. Refresh StorySpec README structure block to current Unity layout.
2. Add one-line clarification in SSOT for `m_AutoStartYarn` default vs setup override.
3. Keep this note until the two source docs are updated directly.
