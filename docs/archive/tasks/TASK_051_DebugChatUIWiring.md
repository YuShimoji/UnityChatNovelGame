# Task: DebugChatScene UI Wiring Hardening

Status: COMPLETED
Tier: 2 (Integration)
Branch: feature/task-051-debugchat-ui-wiring
Owner: Worker
Created: 2026-02-16
Updated: 2026-02-17
Report: docs/reports/REPORT_TASK_051_DebugChatUIWiring.md

## Objective

DebugChatScene の ChatController 参照不足（Choice/Image周り）を解消し、選択肢と画像メッセージ導線を安定化する。

## Milestone

- SG-1: MVP縦切りの最終確認
- MG-1: MVP安定化と最低限の品質基盤

## Focus Area

- `Assets/Scenes/DebugChatScene.unity`
- `Assets/Prefabs/UI/`
- `Assets/Scripts/UI/ChatController.cs`
- `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`

## Forbidden Area

- コアシナリオ仕様の拡張
- SaveSystem仕様の変更
- 大規模UIリデザイン

## Constraints

- 目的は null 参照防止と最小導線成立
- 既存MessageBubbleへのフォールバック互換を維持
- 必要最小限の Prefab/Scene 設定変更に留める

## DoD

- [x] Choice表示時に `ChoiceButtonPrefab or ChoiceContainer is not assigned` が発生しない
- [x] ImageMessage導線が正常に表示される（フォールバック含む）
- [x] DebugChatScene の ChatController 参照が必要項目まで設定される
- [x] 検証証跡を `docs/reports/REPORT_TASK_051_DebugChatUIWiring.md` に残す

## Test Plan

- テスト対象:
  - ChatController.ShowChoices
  - ChatController.AddImageMessage
  - DebugChatScene での実行時参照
- テスト種別:
  - PlayMode（シーン実行検証）
  - 手動確認（Unity Editor）
- 期待結果:
  - 選択肢が表示・選択できる
  - 画像メッセージが表示される
  - Console Error/Exception 0
- テスト不要項目:
  - Windowsビルド検証（TASK_052で実施）

## Stop Conditions

- 新規UI資産の大量追加が必要になる
- シーン分割を伴う改修が必要になる
- 既存テストとの競合が解消できない
