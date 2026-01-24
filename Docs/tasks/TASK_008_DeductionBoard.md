# Task: Deduction Board Implementation

Status: OPEN
Tier: 2
Branch: feat/deduction-board
Owner: Worker
Created: 2026-01-17T00:35:00+09:00
Report: docs/inbox/REPORT_TASK_008_DeductionBoard.md

## Objective
推理ボード（Deduction Board）の基本システムを実装する。
プレイヤーが証拠（Topic）を収集し、それらを組み合わせて新しい結論（Synthesis）を導き出すUIとロジックを構築する。

## Context
- `UnlockTopicCommand` によって解放された `TopicData` を可視化する場所が必要。
- `SynthesisRecipe` に基づいてトピックを合成する機能が必要。
- Core System (Task 001-006) は実装済み。

## Focus Area
- `Assets/Scripts/UI/DeductionBoard.cs`: ボードの管理クラス
- `Assets/Prefabs/UI/TopicNode.prefab`: トピックを表すUI要素
- `Assets/Scripts/Core/ScenarioManager.cs` or `Commands`: `UnlockTopic` 時にボードへ通知する仕組み
- Drag & Drop 操作の実装 (Unity `IDragHandler` 等)

## Forbidden Area
- 見た目の過度な作り込み（まずは機能疎通を優先）
- 既存の `ChatController` の破壊

## Constraints
- `TopicData` の `State` (Hidden, Unlocked, Solved) を反映すること
- 合成（Synthesis）成功時に新しいトピックを解放すること

## DoD (Definition of Done)
- [x] `DeductionBoard.cs` が実装されている
- [x] `TopicNode.prefab` が作成されている (Implemented `TopicCard.cs`)
- [x] `UnlockTopic` コマンドを実行すると、ボード上にトピックが表示される
- [x] 2つのトピックを合成して新しいトピックを作成できる（SynthesisRecipeごとの判定）
- [ ] **Evidence**: ボード操作のGIF/動画 または スクリーンショット (Pending User Verification)
- [x] Report 作成
