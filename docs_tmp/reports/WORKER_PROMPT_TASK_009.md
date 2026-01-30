# Worker Prompt: TASK_009_DeductionBoard

あなたは Worker Agent です。以下の指示に従い、タスクを遂行してください。
Orchestrator によって定義された境界(Focus/Forbidden)を遵守することが求められます。

## 参照情報
- **チケット**: `docs/tasks/TASK_009_DeductionBoard.md` (必読)
- **SSOT**: `docs/Windsurf_AI_Collab_Rules_latest.md`
- **HANDOVER**: `docs/HANDOVER.md`
- **Context**: `AI_CONTEXT.md`
- **関連実装**: `Assets/Scripts/Core/ScenarioManager.cs` (UnlockTopicCommand)
- **データモデル**: `Assets/Scripts/Data/TopicData.cs`

## ミッション
**DeductionBoard (推論ボード) 実装**

- `DeductionBoard.cs` を新規作成し、トピックの追加・表示・管理機能を実装する。
- `TopicCard.cs` を新規作成し、TopicDataの視覚的表示を担当する。
- 対応するPrefab (`DeductionBoard.prefab`, `TopicCard.prefab`) を作成する。
- `ScenarioManager.UnlockTopicCommand` から呼び出せるようにする。

## 境界 (Boundaries)

### Focus Area(変更許可)
- `Assets/Scripts/UI/DeductionBoard.cs` (新規)
- `Assets/Scripts/UI/TopicCard.cs` (新規)
- `Assets/Prefabs/UI/DeductionBoard.prefab` (新規)
- `Assets/Prefabs/UI/TopicCard.prefab` (新規)
- `Assets/Scripts/Core/ScenarioManager.cs` (UnlockTopicCommand連携のみ)

### Forbidden Area(変更禁止)
- `Assets/Scripts/UI/ChatController.cs`
- `Assets/Scripts/Core/` の他のファイルへの大幅変更
- 既存Prefabの破壊的変更

## Definition of Done (DoD)
- [ ] `DeductionBoard.cs` が実装され、トピックの追加・表示ができる
- [ ] `TopicCard.cs` が実装され、TopicDataの情報を表示できる
- [ ] `DeductionBoard.prefab` と `TopicCard.prefab` が作成されている
- [ ] `ScenarioManager.UnlockTopicCommand` からトピック追加が呼び出せる
- [ ] Unity Editorで動作確認が完了している
- [ ] `docs/reports/REPORT_TASK_009_DeductionBoard.md` にレポートが作成されている

## 停止条件 (Stop & Report)
- TopicData ScriptableObjectの構造変更が必要になった場合
- 既存のScenarioManagerロジックとの競合が発生した場合

## 納品物
- 新規作成されたコード(DeductionBoard.cs, TopicCard.cs)
- 新規Prefab
- `docs/reports/REPORT_TASK_009_DeductionBoard.md`
