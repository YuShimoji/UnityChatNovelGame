# Worker Prompt: TASK_017_FixEditorCompilation

## 参照
- チケット: `docs/tasks/TASK_017_FixEditorCompilation.md`
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md`

## 境界
- **Focus Area**: `Assets/Scripts/Editor/DeductionBoardSetup.cs`
- **Forbidden Area**: `Assets/Scripts/Debug/Editor/DebugSceneBuilder.cs` (Do not rename the namespace, just fix the usage site)

## Context
- `ProjectFoundPhone.Debug` namespace exists in `DebugSceneBuilder.cs`.
- This shadows `UnityEngine.Debug` inside `ProjectFoundPhone` namespace scope.
- `DeductionBoardSetup.cs` fails to compile.

## Instructions
1. Open `Assets/Scripts/Editor/DeductionBoardSetup.cs`.
2. Replace all instances of `Debug.Log`, `Debug.LogWarning`, `Debug.LogError` with `UnityEngine.Debug.Log`, etc.
3. Verify that the file saves and (if possible) check logs to ensure no CS0234 errors remain.
4. Create `docs/inbox/REPORT_TASK_017_FixEditorCompilation.md`.

## DoD
- [ ] `DeductionBoardSetup.cs` uses `UnityEngine.Debug`.
- [ ] Report created.

## 納品先
- `docs/inbox/REPORT_TASK_017_FixEditorCompilation.md`
