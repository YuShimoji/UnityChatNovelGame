# Task: Unity Core System Skeleton Implementation
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T07:00:00Z
Report: docs/reports/REPORT_TASK_001_UnityCoreSystemSkeleton.md

## Objective
Unityプロジェクト「Project_FoundPhone」のコアシステムのスケルトンコード（クラス定義、主要メソッド、変数の定義）を作成する。中身のロジックはTODOコメントで構いません。

実装対象：
1. `TopicData.cs` (ScriptableObject) & `SynthesisRecipe.cs`
2. `ChatController.cs` (UI制御の基盤)
3. `ScenarioManager.cs` (Yarn連携とカスタムコマンド登録)

## Context
- プロジェクトタイプ: Unity 6 (or 2022 LTS) のホラー・チャットノベルゲーム
- アーキテクチャ: MVCパターン
- 必須パッケージ: Yarn Spinner, DOTween Pro, TextMeshPro
- 参照ドキュメント: `最初のプロンプト`（プロジェクトルート）

## Focus Area
- `Assets/Scripts/Data/` 配下: TopicData.cs, SynthesisRecipe.cs
- `Assets/Scripts/UI/` 配下: ChatController.cs
- `Assets/Scripts/Core/` 配下: ScenarioManager.cs
- Unity C# コーディング規約（PascalCase, camelCase, #region使用）
- SOLID原則に基づく設計
- スケルトンコードのみ（ロジックはTODOコメント）

## Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更
- パッケージの追加（Yarn Spinner, DOTween, TextMeshProは既に前提）
- ロジックの完全実装（スケルトンコードのみ）
- PrefabやSceneの作成
- テストコードの作成（後続タスクへ分離）

## Constraints
- テスト: 主要パスのみ（網羅テストは後続タスクへ分離）
- フォールバック: 新規追加禁止
- ディレクトリ構造: 指定されたパスに従う（Assets/Scripts/Data/, Assets/Scripts/UI/, Assets/Scripts/Core/）
- コードスタイル: Unity C# ベストプラクティスに従う
- 命名規則: 変数名は m_VariableName, 定数は c_ConstantName, 静的は s_StaticName

## DoD
- [x] TopicData.cs が作成されている（ScriptableObject、ID, Icon, Title, Description）
- [x] SynthesisRecipe.cs が作成されている（ScriptableObject、Topic A + Topic B = Topic C）
- [x] ChatController.cs が作成されている（ScrollRect, VerticalLayoutGroup, ContentSizeFitter使用、Typing Indicator, Auto Scroll）
- [x] ScenarioManager.cs が作成されている（Yarn Spinner DialogueRunnerラップ、カスタムコマンドハンドラ）
- [x] 全てのクラスがSOLID原則に基づいて設計されている
- [x] 主要メソッドと変数が定義されている（ロジックはTODOコメント）
- [x] docs/inbox/ にレポート（REPORT_TASK_001_UnityCoreSystemSkeleton.md）が作成されている
- [x] 本チケットの Report 欄にレポートパスが追記されている

## 実装詳細

### 1. TopicData.cs & SynthesisRecipe.cs
- **場所**: `Assets/Scripts/Data/`
- **TopicData**: 
  - ScriptableObjectを継承
  - フィールド: string topicID, Sprite icon, string title, string description
  - CreateAssetMenu属性でエディタから作成可能に
- **SynthesisRecipe**:
  - ScriptableObjectを継承
  - フィールド: TopicData ingredientA, TopicData ingredientB, TopicData result
  - CreateAssetMenu属性でエディタから作成可能に

### 2. ChatController.cs
- **場所**: `Assets/Scripts/UI/`
- MonoBehaviourを継承
- フィールド: ScrollRect scrollRect, VerticalLayoutGroup layoutGroup, GameObject messageBubblePrefab, GameObject typingIndicator
- メソッド: AddMessage(string charID, string text), ShowTypingIndicator(bool show), AutoScroll()
- Auto Scroll: ユーザーが過去ログを見ている場合は強制スクロールしない

### 3. ScenarioManager.cs
- **場所**: `Assets/Scripts/Core/`
- MonoBehaviourを継承
- フィールド: DialogueRunner dialogueRunner, ChatController chatController
- カスタムコマンドハンドラ:
  - MessageCommand(string charID, string text)
  - ImageCommand(string charID, string imageID)
  - StartWaitCommand(int seconds)
  - UnlockTopicCommand(string topicID)
  - GlitchCommand(int level)

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- Assetsディレクトリが存在しない場合は、ディレクトリ構造を作成してから実装する
