# Task: DeductionBoard (推論ボード) 実装
Status: OPEN
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-16T13:55:00Z
Report: docs/reports/REPORT_TASK_009_DeductionBoard.md

## Objective
プレイヤーが獲得したトピック(TopicData)を表示・管理する「推論ボード(DeductionBoard)」UIを実装する。
ScenarioManagerのUnlockTopicCommandから呼び出され、トピックの追加・表示を行う。

## Context
- **前工程**: TASK_002でUnlockTopicCommandのスケルトンが実装済み(Debug.Logで代替中)
- **依存**: TopicData ScriptableObject (既に定義済み)
- **ゴール**: トピックの視覚的管理と、将来のトピック合成(SynthesisRecipe)の基盤構築

## Focus Area
- `Assets/Scripts/UI/DeductionBoard.cs` (新規)
- `Assets/Scripts/UI/TopicCard.cs` (新規、表示用コンポーネント)
- `Assets/Prefabs/UI/DeductionBoard.prefab` (新規)
- `Assets/Prefabs/UI/TopicCard.prefab` (新規)
- `Assets/Scripts/Core/ScenarioManager.cs` (UnlockTopicCommand連携部分のみ)

## Forbidden Area
- `Assets/Scripts/UI/ChatController.cs` への変更
- `Assets/Scripts/Core/` の他のファイルへの大幅変更
- 既存のPrefabの破壊的変更

## Constraints
- TopicDataのScriptableObjectを使用すること
- UIはCanvas上に配置可能な構造にすること
- シンプルなリスト表示から開始(グリッドレイアウトは後続タスク)

## Steps
1. DeductionBoard.cs の基本構造を実装(トピック追加/削除/表示)
2. TopicCard.cs を実装(TopicDataの表示用)
3. Prefabを作成(ScrollView + GridLayout)
4. ScenarioManager.UnlockTopicCommandと連携
5. テストシーンで動作確認

## DoD (Definition of Done)
- [ ] `DeductionBoard.cs` が実装され、トピックの追加・表示ができる
- [ ] `TopicCard.cs` が実装され、TopicDataの情報(名前、アイコン等)を表示できる
- [ ] `DeductionBoard.prefab` と `TopicCard.prefab` が作成されている
- [ ] `ScenarioManager.UnlockTopicCommand` からトピック追加が呼び出せる
- [ ] Unity Editorで動作確認が完了している
- [ ] `docs/reports/REPORT_TASK_009_DeductionBoard.md` にレポートが作成されている

## 停止条件
- TopicData ScriptableObjectの構造変更が必要になった場合
- 既存のScenarioManagerロジックとの競合が発生した場合

## Notes
- TASK_008 (ChatUI Integration) と並行実行可能
- 将来的にはトピック合成(SynthesisRecipe)機能と連携予定
