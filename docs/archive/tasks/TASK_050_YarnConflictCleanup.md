# Task: Yarn Script Conflict Cleanup for Vertical Slice

Status: COMPLETED
Tier: 2 (Stabilization)
Branch: feature/task-050-yarn-conflict-cleanup
Owner: Worker
Created: 2026-02-16
Updated: 2026-02-16
Report: docs/reports/REPORT_TASK_050_YarnConflictCleanup.md

## Objective

Yarn スクリプトのノード重複/変数再宣言を解消し、縦切りシナリオが安定して実行できる状態にする。

## Milestone

- SG-1: MVP縦切りの最終確認
- MG-1: MVP安定化と最低限の品質基盤

## Focus Area

- `Assets/Resources/Yarn/DebugScript.yarn`
- `Assets/Resources/Yarn/VerticalSlice.yarn`
- `Assets/Resources/Yarn/Project.yarnproject`
- `docs/evidence/TASK_047/EditorBatchCheck.log`

## Forbidden Area

- ChatController/ScenarioManager の仕様追加
- 新規Yarn機能追加（演出拡張）
- 物語テーマの大幅改稿

## Constraints

- 修正は「重複回避」と「実行安定化」に限定する
- ノード命名と変数スコープの衝突をなくす
- 既存コマンド（Message/SystemMessage/StartWait等）互換を維持する

## DoD

- [x] `More than one node is named Start` が解消される（静的確認済み）
- [x] `Redeclaration of existing variable` が解消される（静的確認済み）
- [x] Vertical Slice用の開始ノードが一意に定義される（静的確認済み）
- [x] EditMode/PlayMode 実行証跡が取得される
- [x] 修正内容を `docs/reports/REPORT_TASK_050_YarnConflictCleanup.md` に記録する

## Test Plan

- テスト対象:
  - Yarn importer コンパイル
  - DebugChatScene のシナリオ開始
- テスト種別:
  - EditMode（Yarn import/compile確認）
  - PlayMode（開始ノード実行確認）
- 期待結果:
  - Yarn コンパイルエラー 0
  - シナリオ進行が開始される
- テスト不要項目:
  - ビルド成果物検証（TASK_052で実施）

## Stop Conditions

- シナリオ仕様の再設計が必要になる
- 複数シーンでノード名衝突し、影響範囲が広域化する
- Yarn package側不具合が疑われる
