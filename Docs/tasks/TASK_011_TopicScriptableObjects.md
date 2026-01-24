# Task: Topic ScriptableObjects Creation

Status: IN_PROGRESS
Tier: 2
Branch: feat/topic-scriptableobjects
Owner: Worker
Created: 2026-01-17T02:00:00+09:00
Report: docs/inbox/REPORT_TASK_011_TopicScriptableObjects.md 

## Objective
`TopicData` ScriptableObject のインスタンス（アセット）を作成する。
`UnlockTopicCommand` や `DeductionBoard` で使用するトピックデータを準備する。

## Context
- `TopicData.cs` (ScriptableObject定義) は実装済み（TASK_001）
- `UnlockTopicCommand` は `Resources.Load<TopicData>($"Topics/{topicID}")` で読み込む想定
- `DeductionBoard` (TASK_008) でトピックを表示するために、実際のアセットが必要
- プロジェクトの初期シナリオで使用するトピックを定義する必要がある

## Focus Area
- `Assets/Resources/Topics/` 配下: TopicData アセットの作成
- 初期トピックの定義（例: "Strange Signal", "Missing Person", "Found Phone" 等）
- 各トピックの `State` (Hidden, Unlocked, Solved) の初期値設定
- `SynthesisRecipe` との関連付け（合成レシピの定義）

## Forbidden Area
- `TopicData.cs` の定義変更（既存の構造を維持）
- 過度なトピック数の作成（初期シナリオに必要な最小限のみ）
- 既存のアセットの破壊的変更

## Constraints
- テスト: 主要パスのみ（Resources.Load での読み込み確認）
- フォールバック: 新規追加禁止（既存の TopicData 構造に準拠）
- Unity Editor での手動作成（ScriptableObject の CreateAssetMenu を使用）

## DoD (Definition of Done)
- [x] `Assets/Resources/Topics/` ディレクトリが存在する（4つのアセットが存在するため確認済み）
- [x] 初期シナリオで使用するトピックアセットが3つ以上作成されている（4つ存在）
- [x] 各トピックアセットが `Resources.Load<TopicData>($"Topics/{topicID}")` で読み込める（テスト成功: 4 succeeded, 0 failed）
- [/] `UnlockTopicCommand` でトピックを解放できる（Code Verified / Pending Runtime Check）
- [ ] `DeductionBoard` (TASK_008完了後) でトピックが表示できる
- [ ] **Evidence**: トピックアセットの Inspector 表示スクリーンショット (Pending Manual Action)
- [x] `docs/inbox/` にレポート (`REPORT_TASK_013_TopicDataVerification.md`) が作成されている
- [x] 本チケットの Report 欄にレポートパスが追記されている

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- トピックの内容はプロジェクトの仕様書（`Docs/Core Specification`）に基づいて定義すること
- 将来的にシナリオが拡張されることを考慮し、トピックIDの命名規則を統一すること
