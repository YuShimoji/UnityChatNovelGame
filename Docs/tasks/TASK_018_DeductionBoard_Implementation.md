# Task: Deduction Board Implementation

Status: OPEN
Tier: 2
Branch: feat/deduction-board
Owner: Worker
Created: 2026-01-26T02:40:00+09:00
Report: docs/reports/REPORT_TASK_018_DeductionBoard_Implementation.md

## Objective
推理ボード（Deduction Board）の基本システムを実装する。
`TopicData` (TASK_011) を管理・表示し、シナリオ進行 (`UnlockTopicCommand`) と連動してトピックが追加される仕組みを構築する。
以前の重複タスク (TASK_016, TASK_017) の統合版。

## Context
- **TopicData**: 既に実装済み (TASK_001/011)。ScriptableObject。
- **ScenarioManager**: `UnlockTopicCommand` が実装済みだが、現在はログ出力のみ。ここからボードへの通知を行う。
- **Goal**: プレイヤーが獲得した手がかりを可視化し、将来的な「推理（合成）」機能の基盤を作ること。

## Focus Area
- `Assets/Scripts/UI/DeductionBoard.cs`: ボード管理クラス (Singleton or Manager-managed)
- `Assets/Scripts/UI/TopicCard.cs`: 個別のトピック表示用コンポーネント
- `Assets/Prefabs/UI/DeductionBoard.prefab`: ボード全体のPrefab
- `Assets/Prefabs/UI/TopicCard.prefab`: トピックカードのPrefab
- Integration: `ScenarioManager.cs` の `UnlockTopicCommand`

## Forbidden Area
- `ChatController.cs` への変更（チャットUIとは独立させる）
- 見た目の過度な作り込み（機能疎通優先）

## Constraints
- **Data Source**: `TopicData` ScriptableObject (Resources/Topics/)
- **UI Architecture**: Canvas 上にオーバーレイ、または別パネルとして実装。
- **Layout**: `ScrollView` + `GridLayoutGroup` (または同等の整列機能) を使用。

## DoD (Definition of Done)
- [ ] `DeductionBoard.cs` が実装されている
  - [ ] `AddTopic(TopicData data)` メソッドを持つ
  - [ ] 既に持っているトピックの重複追加を防ぐ
- [ ] `TopicCard.cs` が実装されている
  - [ ] `Setup(TopicData data)` でアイコンとタイトルを表示できる
- [ ] Prefab が作成されている (`DeductionBoard.prefab`, `TopicCard.prefab`)
- [ ] `ScenarioManager` の `UnlockTopicCommand` から `DeductionBoard.AddTopic` が呼ばれる
- [ ] **Verification**:
  - [ ] `DebugChatScene` で `<<UnlockTopic>>` コマンド実行時にボードにカードが追加されることを確認
- [ ] Report 作成 (`docs/reports/REPORT_TASK_018_DeductionBoard_Implementation.md`)

## Notes
- TASK_016, TASK_017 は本タスクに統合されたため Close 済み。
- 将来的にドラッグ＆ドロップによる合成機能が入るため、各カードは個別のGameObjectとして操作可能にしておくこと。
