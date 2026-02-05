# Task 029: Fix Assembly Definition Conflicts

## Goal
Resolve compilation errors caused by duplicate Assembly Definitions, git conflicts in `ProjectFoundPhone.Tests.asmdef`, and lingering metadata for missing modular asmdefs. Restore the project to a stable monolithic assembly structure.

## Context
The project failed to compile with errors:
- "Folder 'Assets/Scripts/' contains multiple assembly definition files"
- "JSON parse error" in `ProjectFoundPhone.Tests.asmdef`
- Missing .asmdef references for Core, UI, etc.

## Proposed Changes
1.  **Resolve Conflict**: Fix `Assets/Scripts/Tests/ProjectFoundPhone.Tests.asmdef` to remove conflict markers and reference `ProjectFoundPhone`.
2.  **Remove Duplicate**: Delete `Assets/Scripts/ProjectFoundPhone.Runtime.asmdef` (and `.meta`).
3.  **Cleanup**: Remove tracking for missing modular asmdef meta files (`Core`, `UI`, etc.) using `git rm`.
4.  **Verify**: Ensure no compilation errors remain.

## Definition of Done
- [ ] No compilation errors in Unity Console.
- [ ] `ProjectFoundPhone.Tests.asmdef` is valid JSON.
- [ ] Only `ProjectFoundPhone.asmdef` exists in `Assets/Scripts`.
- [ ] Git status is clean (no missing/deleted files showing as confusing states).
