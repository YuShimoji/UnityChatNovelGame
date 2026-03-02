# Task: Full Playthrough Test

Status: COMPLETED
Tier: 2 (Verification)
Branch: main
Owner: Worker
Created: 2026-02-02T04:00:00+09:00
Updated: 2026-03-01T02:05:15+09:00
Report: docs/reports/REPORT_TASK_027_FullPlaythroughTest.md

## Objective

DebugChatScene の通し導線（Chat UI -> Topic -> Deduction -> Synthesis -> Effect/End）を自動検証し、統合動作を証跡付きで確定する。

## Context

- Core System: 100% 実装済み
- Chat UI: 表示不全を解消し、バブル表示を再確認済み
- Deduction Board: 100% 実装済み
- Synthesis System: 100% 実装済み
- Visual Effects: 100% 実装済み
- 2026-03-01 に Unity batch automation で通し証跡を回収済み

## Verification Gate (Layer Split)

### Layer A (AI-completable)

- 実行ログ・証跡ファイルの整理
- レポート更新とブロッカー明文化

### Layer B (verification run)

- `VerificationMenu.RunVerticalSliceFullPlaythroughBatch` で `DebugChatScene` を通し実行
- `docs/evidence/TASK_027/` に日付付き結果ファイルとキャプチャを保存

## Current Status (2026-03-01)

- `RunVerticalSliceFullPlaythroughBatch` が exit code 0 で完了した
- `docs/evidence/TASK_027/FULL_PLAYTHROUGH_RESULTS_20260301_015520.md` に SUCCESS を記録した
- Topic unlock / synthesis / end marker まで到達し、Unexpected Error Count は 0
- raw Unity log に batch capture 由来のノイズと `missing script` 行が残るため、次サイクルの改善対象として切り分けた

## Steps

1. `ProjectFoundPhone.EditorTools.VerificationMenu.RunVerticalSliceFullPlaythroughBatch` を batchmode で実行した
2. `docs/evidence/TASK_027/` に以下を保存した
   - `FULL_PLAYTHROUGH_RESULTS_20260301_015520.md`
   - `Log_20260301_015520.txt`
   - `Capture_01_start.png`
   - `Capture_02_topic.png`
   - `Capture_03_synthesis_or_end.png`
3. `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md` に観測結果と residual observations を反映した

## DoD (Definition of Done)

- [x] Full Playthrough が完了している
- [x] `docs/evidence/TASK_027/` に日付付き結果ファイルと 3 枚のキャプチャが保存されている
- [x] 発見された問題が Issue/Task として起票されている、または follow-up として記録されている
- [x] レポートが `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md` に更新されている
