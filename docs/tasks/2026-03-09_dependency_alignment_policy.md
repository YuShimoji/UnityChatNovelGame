# Dependency Alignment Policy (Draft, 2026-03-09)

## Objective
Reduce package-related editor instability by enforcing a simple manifest/lock consistency policy.

## Current Risk Signals
- `manifest.json` and `packages-lock.json` do not fully align for some packages.
- Mixed transitive versions around test framework and AI-related stacks.
- Duplicate Roslyn/shared assemblies detected when AI Assistant and YarnSpinner editor tooling coexist.

## Policy Rules
1. Treat `Packages/manifest.json` as the source of truth for direct dependencies.
2. Require `Packages/packages-lock.json` to match direct package versions after update.
3. For each dependency update PR/task, run:
   - `powershell -ExecutionPolicy Bypass -File scripts/check_unity_dependency_alignment.ps1`
4. If risk flags include duplicate Roslyn/shared assemblies:
   - Avoid simultaneous upgrade of `com.unity.ai.*` and `dev.yarnspinner.unity` in the same step.
   - Validate editor startup and script reload times before proceeding.
5. Keep package changes and gameplay feature changes in separate commits/tasks.

## Definition of Done
- Alignment check script reports no direct mismatch.
- No new duplicate assembly warning increase versus baseline log.
- One clean editor startup + one content authoring run complete without new import drift.
