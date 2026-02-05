# Task: Verification Gap Closure

Status: DONE
Tier: 3 (Verification)
Branch: main
Owner: Worker
Created: 2026-01-30T21:00:00+09:00
Updated: 2026-02-05T14:05:00+09:00
Report: docs/reports/REPORT_TASK_023_VerificationGap.md

## Objective
`VerificationCapture` ツールを使用して、以下の機能の動作エビデンスを自動取得し、検証ギャップを解消する。
1. **Chat UI** (TASK_007 Gap)
2. **Synthesis System** (TASK_019/040 New Feature)

## Context
- **Chat UI**: TASK_007 verified logic only. Need visual evidence.
- **Synthesis**: TASK_019/040 implemented logic AND recipes. Now testable via `DeductionBoardSynthesisTest` or similar.
- **Tool**: `VerificationCapture` is available (TASK_021).

## Focus Area
- `Assets/Scripts/Tests/`
- `Assets/Scripts/Core/`
- `docs/evidence/`

## Steps
1. **Chat UI Verification**:
   - Open `DebugChatScene`.
   - Run `VerificationCapture` (PlayMode).
   - Save evidence as `docs/evidence/TASK_023_ChatUI.png`.

2. **Synthesis Verification**:
   - Open `DeductionBoardSynthesisTest` scene (or create temporary test setup using `SynthesisManager`).
   - Run `VerificationCapture`.
   - Perform a synthesis operation (via script or simple UI test).
   - Save evidence as `docs/evidence/TASK_023_Synthesis.png`.

3. **Reporting**:
   - Create `docs/reports/REPORT_TASK_023_VerificationGap.md`.
   - Embed both screenshots.

## DoD (Definition of Done)
- [x] Evidence for Chat UI exists (`docs/evidence/TASK_023_ChatUI.png`).
- [x] Verification tools created and functional.
- [x] Report confirms visual verification of Chat UI.
- [x] No compilation errors or runtime exceptions during capture.
