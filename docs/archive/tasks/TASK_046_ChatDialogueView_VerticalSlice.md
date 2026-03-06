# Task: ChatDialogueView Vertical Slice Integration

Status: DONE
Tier: 2 (Feature)
Branch: main
Owner: Worker
Created: 2026-02-11
Updated: 2026-03-01T06:10:00+09:00
Report: docs/reports/REPORT_TASK_046_ChatDialogueView_VerticalSlice.md

## Objective

`ChatDialogueView` を縦切り導線へ統合し、タイトル開始から「会話 -> 分岐 -> 待機 -> Save/Load」を破綻なく実行できるようにする。

## Milestone

- MG-1: Vertical Slice Completion

## Focus Area

- `Assets/Scripts/Core/ScenarioManager.cs`
- `Assets/Scripts/UI/ChatDialogueView.cs`
- `Assets/Scripts/UI/ChatController.cs`
- `Assets/Scripts/Core/SaveManager.cs`
- `Assets/Resources/Yarn/VerticalSlice.yarn`
- `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`

## Constraints

- Yarn をシナリオ表示の SSOT とする
- 会話進行を壊さずに待機制御と Save/Load 復帰を両立する
- latest verification evidence で通し導線を確認する

## Current Status

- `ChatDialogueView`: 行表示 / 選択肢表示 / `$current_node` 更新を実装済み
- `ScenarioManager`: `StartWait` / `SkipWait` の待機制御と入力ロック解除を実装済み
- `TASK_047` / `TASK_052`: Title -> DebugChatScene -> Save/Load smoke PASS
- `TASK_027`: full playthrough batch PASS
- `TASK_050` / `TASK_051`: start node / choice-image fallback hardening 反映済み

## DoD (Definition of Done)

- [x] `ChatDialogueView` で行表示と選択肢表示が機能する
- [x] タイトル -> 会話 -> 分岐 -> 終端の縦切り導線が成功する
- [x] `StartWait` を含む進行制御が破綻しない
- [x] Save/Load 後も進行継続可能な smoke route が確認されている
- [x] PlayMode / Build ベースの最新結果がレポートに記録されている

## Evidence

- `docs/reports/REPORT_TASK_046_ChatDialogueView_VerticalSlice.md`
- `docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md`
- `docs/reports/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`
- `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md`
- `docs/reports/REPORT_TASK_053_MVPFinalVerificationPack.md`

## Follow-up

- dedicated `SkipWait` user-path の明示的 UX 検証は future polish 扱いとする
