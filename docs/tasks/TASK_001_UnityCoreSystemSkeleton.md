# Task: Unity Core System Skeleton Implementation
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T07:00:00Z
Report: docs/reports/REPORT_TASK_001_UnityCoreSystemSkeleton.md

## Objective
Unityプロジェクト「Project_FoundPhone」�EコアシスチE��のスケルトンコード（クラス定義、主要メソチE��、変数の定義�E�を作�Eする。中身のロジチE��はTODOコメントで構いません、E
実裁E��象�E�E1. `TopicData.cs` (ScriptableObject) & `SynthesisRecipe.cs`
2. `ChatController.cs` (UI制御の基盤)
3. `ScenarioManager.cs` (Yarn連携とカスタムコマンド登録)

## Context
- プロジェクトタイチE Unity 6 (or 2022 LTS) のホラー・チャチE��ノ�Eルゲーム
- アーキチE��チャ: MVCパターン
- 忁E��パチE��ージ: Yarn Spinner, DOTween Pro, TextMeshPro
- 参�EドキュメンチE `最初�Eプロンプト`�E��Eロジェクトルート！E
## Focus Area
- `Assets/Scripts/Data/` 配丁E TopicData.cs, SynthesisRecipe.cs
- `Assets/Scripts/UI/` 配丁E ChatController.cs
- `Assets/Scripts/Core/` 配丁E ScenarioManager.cs
- Unity C# コーチE��ング規紁E��EascalCase, camelCase, #region使用�E�E- SOLID原則に基づく設訁E- スケルトンコード�Eみ�E�ロジチE��はTODOコメント！E
## Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定�E変更
- パッケージの追加�E�Earn Spinner, DOTween, TextMeshProは既に前提�E�E- ロジチE��の完�E実裁E��スケルトンコード�Eみ�E�E- PrefabやSceneの作�E
- チE��トコード�E作�E�E�後続タスクへ刁E���E�E
## Constraints
- チE��チE 主要パスのみ�E�網羁E��スト�E後続タスクへ刁E���E�E- フォールバック: 新規追加禁止
- チE��レクトリ構造: 持E��されたパスに従う�E�Essets/Scripts/Data/, Assets/Scripts/UI/, Assets/Scripts/Core/�E�E- コードスタイル: Unity C# ベスト�EラクチE��スに従う
- 命名規則: 変数名�E m_VariableName, 定数は c_ConstantName, 静的は s_StaticName

## DoD
- [x] TopicData.cs が作�EされてぁE���E�EcriptableObject、ID, Icon, Title, Description�E�E- [x] SynthesisRecipe.cs が作�EされてぁE���E�EcriptableObject、Topic A + Topic B = Topic C�E�E- [x] ChatController.cs が作�EされてぁE���E�EcrollRect, VerticalLayoutGroup, ContentSizeFitter使用、Typing Indicator, Auto Scroll�E�E- [x] ScenarioManager.cs が作�EされてぁE���E�Earn Spinner DialogueRunnerラチE�E、カスタムコマンドハンドラ�E�E- [x] 全てのクラスがSOLID原則に基づぁE��設計されてぁE��
- [x] 主要メソチE��と変数が定義されてぁE���E�ロジチE��はTODOコメント！E- [x] docs/inbox/ にレポ�Eト！EEPORT_TASK_001_UnityCoreSystemSkeleton.md�E�が作�EされてぁE��
- [x] 本チケチE��の Report 欁E��レポ�Eトパスが追記されてぁE��

## 実裁E��細

### 1. TopicData.cs & SynthesisRecipe.cs
- **場所**: `Assets/Scripts/Data/`
- **TopicData**: 
  - ScriptableObjectを継承
  - フィールチE string topicID, Sprite icon, string title, string description
  - CreateAssetMenu属性でエチE��タから作�E可能に
- **SynthesisRecipe**:
  - ScriptableObjectを継承
  - フィールチE TopicData ingredientA, TopicData ingredientB, TopicData result
  - CreateAssetMenu属性でエチE��タから作�E可能に

### 2. ChatController.cs
- **場所**: `Assets/Scripts/UI/`
- MonoBehaviourを継承
- フィールチE ScrollRect scrollRect, VerticalLayoutGroup layoutGroup, GameObject messageBubblePrefab, GameObject typingIndicator
- メソチE��: AddMessage(string charID, string text), ShowTypingIndicator(bool show), AutoScroll()
- Auto Scroll: ユーザーが過去ログを見てぁE��場合�E強制スクロールしなぁE
### 3. ScenarioManager.cs
- **場所**: `Assets/Scripts/Core/`
- MonoBehaviourを継承
- フィールチE DialogueRunner dialogueRunner, ChatController chatController
- カスタムコマンドハンドラ:
  - MessageCommand(string charID, string text)
  - ImageCommand(string charID, string imageID)
  - StartWaitCommand(int seconds)
  - UnlockTopicCommand(string topicID)
  - GlitchCommand(int level)

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想宁E- BLOCKED の場合�E、事宁E根拠/次手（候補）を本斁E��追記し、Report に docs/inbox/REPORT_...md を忁E��設宁E- AssetsチE��レクトリが存在しなぁE��合�E、ディレクトリ構造を作�Eしてから実裁E��めE
