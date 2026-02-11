# Task: ChatDialogueView Vertical Slice Integration

Status: OPEN
Tier: 2 (Feature)
Branch: feature/task-046-chatdialogueview-vs
Owner: Worker
Created: 2026-02-11
Updated: 2026-02-11
Report: docs/reports/REPORT_TASK_046_ChatDialogueView_VerticalSlice.md

## Objective

`ChatDialogueView` を縦切り導線へ正式接続し、タイトル開始から会話進行・分岐・待機が通る状態を作る。

## Milestone

- MG-1: Vertical Slice Completion

## Focus Area

- Assets/Scripts/Core
- Assets/Scripts/UI
- Assets/Resources/Yarn
- Assets/Scenes

## Forbidden Area

- 探索スレッドのリソース仕様確定（TBD領域）
- テーマ依存演出の大規模追加
- Addressables移行

## Constraints

- Yarn をシナリオ進行の正規導線として扱う
- 物語自動生成ロジックは導入しない
- 既存セーブ機能を壊さない（後方互換を維持）

## DoD

- [ ] `ChatDialogueView` で行表示と選択肢表示が機能する
- [ ] タイトル -> 会話 -> 分岐 -> 待機 の通し導線が成立する
- [ ] StartWait/SkipWait の進行制御が破綻しない
- [ ] セーブ/ロードを挟んでも進行が継続できる
- [ ] 実行結果がレポートに記録されている

## Test Plan

- テスト対象:
  - ChatDialogueView 連携
  - Yarn進行（行/選択肢/待機）
  - Save/Load 復帰
- テスト種別:
  - EditMode（必要なロジック単体）
  - PlayMode（縦切り通し）
  - ビルド検証（エラーなし）
- 期待結果:
  - 主要導線で例外・進行停止が発生しない
  - テストが全て成功し、ビルドエラーがない

## Stop Conditions

- 既存 `ScenarioManager` との整合が取れず設計変更が大きくなる
- Yarn資産側の修正だけでは進行不具合が解消できない
