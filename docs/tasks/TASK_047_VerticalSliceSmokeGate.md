# Task: Vertical Slice Smoke Gate

Status: OPEN
Tier: 2 (Verification)
Branch: feature/task-047-vs-smoke-gate
Owner: Worker
Created: 2026-02-11
Updated: 2026-02-11
Report: docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md

## Objective

縦切り導線の破綻を早期検知するため、スモーク検証（自動+手動）を最小構成で整備する。

## Milestone

- MG-1: Vertical Slice Completion

## Focus Area

- Assets/Scripts/Tests
- docs/evidence
- docs/reports

## Forbidden Area

- 本体機能の仕様拡張
- パフォーマンス最適化の深掘り

## Constraints

- 網羅テストではなく「導線破綻検知」を優先する
- 実行コストが高い検証は採用しない
- 証跡保存ルール（docs/evidence）を守る

## DoD

- [ ] 縦切り導線のスモークチェック項目が定義されている
- [ ] 最低1本のPlayModeスモークが実行可能
- [ ] 手動チェックリストがレポートに整備されている
- [ ] 失敗時の記録方法（ログ/スクリーンショット）が明記されている

## Test Plan

- テスト対象:
  - タイトル -> チャット進行 -> 分岐 -> 待機 -> セーブ/ロード
- テスト種別:
  - PlayMode スモーク
  - 手動チェックリスト
  - ビルド検証（最低1ターゲット）
- 期待結果:
  - 導線破綻を再現可能な形で検出できる
  - 証跡が `docs/evidence` とレポートに残る

## Stop Conditions

- スモーク設計が機能仕様を越えて過剰に広がる
- 既存テストとの衝突でメンテナンス不能になる
