# Report: TASK_051 DebugChatScene UI Wiring Hardening

**Date**: 2026-02-16
**Status**: IN_PROGRESS

## Summary
- `ChatController.AddImageMessage` で生成した `imageBubble` を明示的に `SetActive(true)` する修正を追加。
- 参照未設定時のランタイム補完ロジック（Choice/Image/Input）が有効な前提で、画像バブルが非表示のまま残る不具合を抑止。
- DoD を機械検証するため、PlayMode テスト `DebugChatScene_ChoiceAndImageFallback_AreUsable` を追加。

## Changed Files
- `Assets/Scripts/UI/ChatController.cs`
- `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`

## Verification
- Static Check: PASS
  - `imageBubble.SetActive(true)` 追加を確認
  - 新規 PlayMode テストメソッド追加を確認
- Runtime Check: BLOCKED
  - 実行日時: 2026-02-16
  - Unity batchmode 実行時に `It looks like another Unity instance is running with this project open.` が発生
  - 同一プロジェクトが別Unityで開かれており、CLI検証不可
  - 影響: Choice/Image の実行時挙動を未確認

## DoD Status
- [ ] Choice未設定エラー未発生の実行証跡
- [ ] ImageMessage導線表示の実行証跡
- [ ] DebugChatScene 参照設定の実行証跡
- [x] レポート作成

## Next Steps
1. UnityChatNovelGame を開いている Editor を閉じる。
2. PlayMode テスト `VerticalSliceSmokeGatePlayModeTests` を実行し、`DebugChatScene_ChoiceAndImageFallback_AreUsable` の結果を確認する。
3. 必要に応じて `docs/evidence/TASK_051/` にスクリーンショット/ログを保存し、DoD を更新する。
