# Worker Prompt: TASK_018 Deduction Board Implementation

## 🛡️ このプロンプトの役割
あなたは **Worker** です。このプロンプトに従い、指定されたタスクを実装してください。
**Orchestrator** (私) は、あなたの成果物を統合し、品質を保証します。

## 🎯 今回のミッション
**タスク**: [TASK_018_DeductionBoard_Implementation.md](../tasks/TASK_018_DeductionBoard_Implementation.md)
**目的**: 推理ボード (Deduction Board) の基本システムを実装し、シナリオ進行と連動させる。

## 📂 参照ファイル (SSOT)
- **Task Definition**: `docs/tasks/TASK_018_DeductionBoard_Implementation.md` (DoD はここに従う)
- **TopicData**: `Assets/Scripts/Data/TopicData.cs` (既存クラス)
- **ScenarioManager**: `Assets/Scripts/Core/ScenarioManager.cs` (`UnlockTopicCommand` の修正対象)
- **Topic Assets**: `Assets/Resources/Topics/` (テストデータ)

## 🛠️ 実装手順 (Step-by-Step)

### Step 1: UI Script Implementation
1. `Assets/Scripts/UI/DeductionBoard.cs` を作成。
   - Singleton パターン (or Manager reference)
   - `AddTopic(TopicData data)` メソッド
   - 重複チェック機能
2. `Assets/Scripts/UI/TopicCard.cs` を作成。
   - `Setup(TopicData data)` メソッド
   - アイコンとタイトルの表示

### Step 2: Prefab Creation
1. Unity Editor は操作できないため、スクリプトで Prefab 構築用 Editor Script を作るか、あるいは **コードベースで Prefab の構成要件** を定義し、ユーザーに手動作成を依頼する形になる（もしくは、簡単な EditorWindow で自動生成するスクリプトを提供）。
   - 推奨: `DeductionBoard` は Canvas 直下の Panel。`TopicCard` はその中の Content 要素。
2. 今回は **Unity Editor 検証時に Prefab 化する** 前提で、まずはスクリプトを完璧にする。

### Step 3: Integration
1. `Assets/Scripts/Core/ScenarioManager.cs` を修正。
   - `UnlockTopicCommand` メソッド内で、`DeductionBoard.Instance` が存在すれば `AddTopic` を呼ぶように変更。

### Step 4: Verification Setup
1. `Assets/Scripts/Dev/DeductionBoardVerifier.cs` (Editor Script or Runtime Script) を作成。
   - `AddTopic` を直接呼んでカードが増えるかテストできるスクリプト。

## ✅ 完了条件 (DoD)
- [ ] `DeductionBoard.cs` と `TopicCard.cs` がコンパイルエラーなく実装されている。
- [ ] `ScenarioManager` がボードと連携している。
- [ ] テスト用スクリプト (`DeductionBoardVerifier.cs`等) が作成されている。
- [ ] レポート (`docs/reports/REPORT_TASK_018_DeductionBoard_Implementation.md`) を作成。
  - レポートには「Unity Editor での手順」を含める（ユーザーに Prefab 作成を作業依頼するため）。

## ⚠️ 注意事項
- **ChatUI とは独立させる**: ChatController に依存しないこと。
- **データ駆動**: TopicData (ScriptableObject) の内容を正しく表示すること。
- **エラーハンドリング**: `TopicData` が null の場合などを考慮。

## 📝 提出物
実装完了後、以下のフォーマットでレポートを作成し、`docs/inbox/` に保存してください。

```markdown
# Report: TASK_018 Deduction Board Implementation

## Status
IMPLEMENTED (Verification Pending)

## Changes
- Created: Assets/Scripts/UI/DeductionBoard.cs
- Created: Assets/Scripts/UI/TopicCard.cs
- Modified: Assets/Scripts/Core/ScenarioManager.cs
- Created: Assets/Scripts/Dev/DeductionBoardVerifier.cs

## Verification Steps (For User)
1. Hierarchy に Canvas/DeductionBoard を作成し、DeductionBoard コンポーネントをアタッチ。
2. ...
```
