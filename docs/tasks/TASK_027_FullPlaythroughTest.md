# Task: Full Playthrough Test

Status: IN_PROGRESS
Tier: 2 (Verification)
Branch: main
Owner: Worker
Created: 2026-02-02T04:00:00+09:00
Updated: 2026-02-27T03:15:00+09:00
Report: docs/reports/REPORT_TASK_027_FullPlaythroughTest.md

## Objective

DebugChatScene の通し導線（Chat UI -> Topic -> Deduction -> Synthesis -> Effect）を手動実測で確認し、統合動作を証跡付きで確定する。

## Context

- Core System: 100% 実装済み
- Chat UI: 表示不全を解消し、バブル表示を再確認済み
- Deduction Board: 100% 実装済み
- Synthesis System: 100% 実装済み
- Visual Effects: 100% 実装済み
- End-to-End の通し証跡は未完了。

## Verification Gate (Layer Split)

### Layer A (AI-completable)

- 実行ログ・証跡ファイルの整理
- レポート更新とブロッカー明文化

### Layer B (manual)

- Unity で `Assets/Scenes/DebugChatScene.unity` を通し実行
- フルプレイ結果の記録と保存

## Current Status (2026-02-27)

- 以前の blocker（Missing Script / bubble不可視）は解消。
- 現在は Layer B のフル導線実測を残す段階。

## Steps

1. DebugChatScene を PlayMode で実行。
2. Chat UI -> Topic -> Deduction -> Synthesis -> End まで通し確認。
3. 各機能の動作を記録（スクリーンショット/ログ）。
4. 問題点・改善点をレポートに反映。

## DoD (Definition of Done)

- [ ] Full Playthrough が完了している。
- [ ] 発見された問題が Issue/Task として起票されている（または問題なしと記録）。
- [x] レポートが `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md` に作成されている。
