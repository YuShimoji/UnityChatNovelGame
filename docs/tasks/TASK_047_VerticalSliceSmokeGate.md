# Task: Vertical Slice Smoke Gate

Status: IN_PROGRESS
Tier: 2 (Verification)
Branch: feature/task-047-vs-smoke-gate
Owner: Worker
Created: 2026-02-11
Updated: 2026-02-11
Report: docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md

## Objective

縦切り導線の破綻を早期検知するため、最小構成のスモークゲート（PlayMode + 手動チェック）を整備する。

## Milestone

- MG-1: Vertical Slice Completion

## Focus Area

- Assets/Scripts/Tests
- docs/evidence
- docs/reports

## Forbidden Area

- 本体機能の仕様拡張
- パフォーマンス最適化の深掘り
- 演出仕様の追加決定

## Constraints

- 網羅性より導線保護を優先する
- 過剰な検証拡張は行わない
- 証跡は `docs/evidence` に保存する

## DoD

- [x] 縦切り導線のスモークチェック項目が定義されている
- [x] 最低1本のPlayModeスモークが実行可能
- [x] 手動チェックリストがレポートに整備されている
- [x] 失敗時の記録方法（ログ/スクリーンショット）が明記されている
- [ ] テスト実行結果（PlayMode/Build）が記録される

## Test Plan

- テスト対象:
  - タイトル -> チャット進行 -> 分岐 -> 待機 -> セーブ/ロード
- テスト種別:
  - PlayMode スモーク（最低1本）
  - 手動チェックリスト
  - ビルド検証（最低1ターゲット）
- 期待結果:
  - 導線破綻を再現可能な形で検出できる
  - 失敗時の証跡（ログ/スクリーンショット）が保存される

## Stop Conditions

- スモーク設計が機能仕様を越えて過剰に広がる
- 既存テストとの衝突でメンテナンス不能になる
- Forbidden Area への変更が必要になる
