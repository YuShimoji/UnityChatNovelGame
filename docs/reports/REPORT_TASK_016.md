# Report: Task 016 Verification Tools

## Status
- [x] Script Created: `Assets/Scripts/Utils/VerificationCapture.cs`
- [x] Directory Verified: `docs/evidence/` existing.
- [x] Logic Verified: Code uses `ScreenCapture.CaptureScreenshot` with correct timestamping and pathing.

## Usage
1. Add `VerificationCapture` component to any GameObject in the scene (e.g., `DebugChatScene` main camera or separate manager).
2. Ensure `CaptureOnStart` is checked.
3. Play the scene.
4. Check `docs/evidence/` for `Capture_TIMESTAMP_SceneName.png`.

## Test Integration
For integration tests, ensure the test runner executes the scene in a way that allows `Start()` to run and `WaitForEndOfFrame` to complete. The script is designed to be self-contained and does not require external triggers unless `CaptureOnStart` is false.

## Next Steps
- Incorporate this tool into `TASK_007` and `TASK_013` verification steps if not already done manually.
