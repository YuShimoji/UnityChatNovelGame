# Task: Full Playthrough Test

Status: OPEN
Tier: 2 (Verification)
Branch: feature/playthrough-test
Owner: Worker
Created: 2026-02-02T04:00:00+09:00
Report: N/A

## Objective
実装済みの全機能（Chat UI、Deduction Board、Synthesis System、Visual Effects）を通しでプレイし、統合動作を検証する。

## Context
- Core System: 100% 実装済み
- Chat UI: 100% 実装済み
- Deduction Board: 100% 実装済み
- Synthesis System: 100% 実装済み
- Visual Effects: 100% 実装済み
- 個別機能は検証済みだが、End-to-End の統合テストは未実施。

## Scope
- DebugChatScene を起点に、シナリオ進行 → Topic取得 → Deduction → Synthesis → Effect発動 の一連フローを確認。
- 不具合や改善点をリストアップ。

## Constraints
- 新機能追加は行わない（検証のみ）。
- 発見した問題は別タスクとして起票。

## Steps
1. DebugChatScene を PlayMode で実行。
2. シナリオを最後まで進行。
3. 各機能の動作を記録（スクリーンショット/ログ）。
4. 問題点・改善点をレポートにまとめる。

## DoD (Definition of Done)
- [ ] Full Playthrough が完了している。
- [ ] 発見された問題が Issue/Task として起票されている（または問題なしと記録）。
- [ ] レポートが `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md` に作成されている。
