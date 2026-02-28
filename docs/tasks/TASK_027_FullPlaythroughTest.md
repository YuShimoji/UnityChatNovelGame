# Task: Full Playthrough Test

Status: IN_PROGRESS
Tier: 2 (Verification)
Branch: main
Owner: Worker
Created: 2026-02-02T04:00:00+09:00
Updated: 2026-02-28T21:36:21+09:00
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

## Current Status (2026-02-28)

- 以前の blocker（Missing Script / bubble不可視）は解消済み。
- 2026-02-27 時点でチャットバブル視認性とランタイム安定性は再確認済み。
- 残件は Layer B のフル導線実測と、日付付き証跡の保存のみ。

## Steps

1. `Assets/Scenes/DebugChatScene.unity` を開き、Console をクリアしてから PlayMode を開始。
2. `Chat UI -> Topic unlock -> Deduction -> Synthesis -> Effect/End` を 1 回通しで実行する。
3. `docs/evidence/TASK_027/` に以下を保存する。
   - `FULL_PLAYTHROUGH_RESULTS_YYYYMMDD.md`
   - `Log_YYYYMMDD.txt`
   - `Capture_01_start.png`
   - `Capture_02_topic.png`
   - `Capture_03_synthesis_or_end.png`
4. 成功/失敗にかかわらず、最後に到達したステップと Console 状態を `REPORT_TASK_027_FullPlaythroughTest.md` に反映する。

## DoD (Definition of Done)

- [ ] Full Playthrough が完了している。
- [ ] `docs/evidence/TASK_027/` に日付付き結果ファイルと 3 枚のキャプチャが保存されている。
- [ ] 発見された問題が Issue/Task として起票されている（または問題なしと記録）。
- [x] レポートが `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md` に作成されている。
