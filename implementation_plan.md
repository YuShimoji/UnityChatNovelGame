# Implementation Plan - Deduction Board & Task Cleanup

## Goal Description
重複している Deduction Board 関連タスク (TASK_016, TASK_017) を統合し、単一の明確な実装タスク (TASK_018) を定義する。
その後、Deduction Board の UI およびロジック実装を行い、`UnlockTopicCommand` との連携を完了させる。

## User Review Required
- **Task Consolidation**: TASK_016 と TASK_017 は統合され、Close されます。
- **New Task**: TASK_018_DeductionBoard_Implementation が新たに SSOT となります。

## Proposed Changes

### 1. Task Management
#### [NEW] [TASK_018_DeductionBoard_Implementation.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/tasks/TASK_018_DeductionBoard_Implementation.md)
- TASK_016 と TASK_017 の要件をマージ。
- **Scope**:
  - `DeductionBoard.cs`: トピック管理、UI制御
  - `TopicCard.cs`: トピック個別表示
  - Prefabs: `DeductionBoard.prefab`, `TopicCard.prefab`
  - Integration: `ScenarioManager` からのイベント受信 (`UnlockTopic`)

#### [MODIFY] [TASK_016_DeductionBoard_Conflict.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/tasks/TASK_016_DeductionBoard_Conflict.md)
- Status: **CLOSED (Merged to TASK_018)**

#### [MODIFY] [TASK_017_DeductionBoard_Conflict.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/tasks/TASK_017_DeductionBoard_Conflict.md)
- Status: **CLOSED (Merged to TASK_018)**

### 2. Implementation (TASK_018)
#### [NEW] Assets/Scripts/UI/DeductionBoard.cs
- `MonoBehaviour`
- Singleton pattern (or reference injection)
- Methods: `AddTopic(TopicData)`, `HasTopic(string id)`
- UI Elements: `ScrollView`, `Content` (Grid Layout)

#### [NEW] Assets/Scripts/UI/TopicCard.cs
- `MonoBehaviour`
- Methods: `Setup(TopicData)`
- UI Elements: `Image (Icon)`, `TextMeshProUGUI (Title)`

#### [MODIFY] Assets/Scripts/Core/ScenarioManager.cs
- `UnlockTopicCommand` 内で `DeductionBoard.Instance.AddTopic()` を呼び出す処理を正式実装（現在は Debug.Log のみの場合あり）。

## Verification Plan

### Automated Tests
- **EditMode Test**: `TopicCard` の生成とデータ割り当てのテスト（Prefab依存のため PlayMode が適切かも）。
- **PlayMode Test**: `DeductionBoard` にトピックを追加し、UI要素が増えることを確認。

### Manual Verification
1. **Scene**: `Assets/Scenes/DebugChatScene.unity`
2. **Action**:
   - Play シーンを実行。
   - `<<UnlockTopic>>` コマンドが実行されるまで進める。
3. **Expectation**:
   - 画面上（または専用のオーバーレイ）に Deduction Board が表示され（またはボタンで開閉）、新しいトピックカードが追加されていること。
