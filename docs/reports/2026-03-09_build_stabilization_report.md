# Build Stabilization Report (2026-03-09)

## Scope
- Project: UnityChatNovelGame
- Goal: Stabilize Content Authoring / MVP batch workflow before feature expansion.

## High-priority Findings
1. Asset import version drift was observed.
   - Source: `Logs/AssetImportWorker1.log`
   - Symptom: `Build asset version error` for `Assets/Scripts/Data/ChatUIConfig.cs`
2. Duplicate managed assemblies are loaded from multiple packages.
   - Source: `Logs/AssetImportWorker1.log`
   - Symptom: Duplicate `Microsoft.CodeAnalysis*`, `System.Reflection.Metadata`, `System.Collections.Immutable`, `System.Runtime.CompilerServices.Unsafe`
   - Likely packages: `com.unity.ai.assistant` and `dev.yarnspinner.unity`
3. Long domain reload and repeated assertions in worker logs.
   - Source: `Logs/AssetImportWorker1.log`
   - Symptom: heavy reload time + repeated `Assertion failed on expression: 'pred(*previous, *i)'`

## Reproduction Baseline
- Unity version in logs: `6000.3.6f1`
- Batch worker command includes `-noUpm`
- Import error was observed during script reimport wave.

## Immediate Mitigation Playbook
1. Close Unity Editor completely.
2. Backup and remove `Library/` only when import DB drift persists.
3. Reopen project once without `-noUpm` (for package graph normalization).
4. Trigger `Reimport All` for `Assets/Scripts/Data/ChatUIConfig.cs` first, then small batches.
5. If duplicate assembly warnings persist, temporarily disable one conflicting editor package path and re-check logs.

## Guardrails for Next Runs
- Do not run content-authoring batch setup immediately after package changes.
- After package update, do one clean editor launch and wait for script compilation to settle.
- Keep log snapshots per run under `docs/reports/` when instability appears.

## Proposed Follow-up Tasks
- Add dependency alignment check to pre-flight (`scripts/check_unity_dependency_alignment.ps1`).
- Define package compatibility policy for AI assistant + YarnSpinner coexistence.
- Track whether `-noUpm` is required for current automation; avoid it for baseline validation runs.
