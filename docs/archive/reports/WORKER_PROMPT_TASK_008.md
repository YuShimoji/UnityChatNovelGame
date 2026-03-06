# Worker Prompt: TASK_008_ChatUI_Integration

## Status
- Current Status: IN_PROGRESS
- Goal: Complete implementation and provide Evidence.

## 参照
- チケット: `docs/tasks/TASK_008_ChatUI_Integration.md`
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md`
- HANDOVER: `docs/HANDOVER.md`
- Verification Tool: `Assets/Scripts/Utils/VerificationCapture.cs` (Usage: Run Scene -> Auto Capture)

## 境界
- **Focus Area**: 
  - `Assets/Scripts/Core/ScenarioManager.cs` (Data Loading)
  - `Assets/Scripts/UI/ChatController.cs` (UI Binding)
  - `Assets/Resources/Data/` (Test Data)
- **Forbidden Area**: 
  - `Assets/Scripts/Core/CoreLifecycle.cs` (Do not modify init order)

## Context
- Task 007 (Visuals) is DONE. Task 013 (Topics) is DONE.
- Task 008 logic is reportedly "RESOLVED" but lacks Evidence.
- **New Requirement**: Use `VerificationCapture` component to automatically take a screenshot during runtime.

## Instructions
1. **Analyze**: Check `ChatController.cs` and `ScenarioManager.cs`. Ensure they can load a basic scenario (JSON/ScriptableObject).
2. **Implement/Fix**: If data loading is missing, implement it.
3. **Verify**:
   - Add `VerificationCapture` to the `DebugChatScene` (or `ChatDevScene`).
   - Run the scene in Unity Editor.
   - Confirm `docs/evidence/auto_capture_xxxx.png` (or similar) is generated.
4. **Report**:
   - Create `docs/inbox/REPORT_TASK_008_ChatUI_Integration.md`.
   - Include the path to the evidence.

## DoD
- [ ] Chat Scenario Loads from Data.
- [ ] User can click options (if applicable) or proceed.
- [ ] Evidence file exists in `docs/evidence/`.
- [ ] Report created.

## 納品先
- `docs/inbox/REPORT_TASK_008_ChatUI_Integration.md`
