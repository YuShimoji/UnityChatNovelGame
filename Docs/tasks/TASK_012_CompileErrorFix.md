# Task: コンパイルエラー修正（TASK_010, TASK_011関連）

Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-17T04:30:00+09:00
Report: docs/inbox/REPORT_TASK_012_CompileErrorFix.md

## Objective
TASK_010 と TASK_011 の実装後に発生したコンパイルエラーと NullReferenceException を修正する。

実装対象：
1. **ScenarioManager.cs の修正**: `ChatController` のメソッド参照エラーを修正
2. **DebugSceneBuilder.cs の修正**: NullReferenceException を修正（130行目、90行目）

## Context
- TASK_010 (MetaEffectController) と TASK_011 (TopicScriptableObjects) の実装後にコンパイルエラーが発生
- エラー内容:
  - `CS1061`: `ChatController` のメソッド（`AddMessage`, `ShowTypingIndicator`）が見つからない（ScenarioManager.cs）
  - `NullReferenceException`: `DebugSceneBuilder.cs` の130行目と90行目で発生
- 参照ドキュメント: `docs/inbox/REPORT_TASK_010_MetaEffectController.md`, `docs/inbox/REPORT_TASK_011_TopicScriptableObjects.md`

## Focus Area
- `Assets/Scripts/Core/ScenarioManager.cs` の修正
- `Assets/Scripts/Debug/Editor/DebugSceneBuilder.cs` の修正
- Unity のコンパイル順序の問題を回避するための完全修飾名の使用
- Null チェックの追加による NullReferenceException の防止

## Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更
- パッケージの追加・削除
- 他のスクリプトファイルの変更（修正対象以外）

## Constraints
- テスト: コンパイルエラーと NullReferenceException が解消されることを確認
- フォールバック: 既存のコード構造を維持し、最小限の修正に留める
- Unityバージョン: Unity 6 (or 2022 LTS) に対応
- コーディング規約: Unity C# コーディング規約に準拠

## DoD
- [x] ScenarioManager.cs のコンパイルエラーが解消されている
  - [x] `ChatController` の型を完全修飾名（`ProjectFoundPhone.UI.ChatController`）で明示的に指定
  - [x] `FindFirstObjectByType<ChatController>()` の呼び出しを修正
- [x] DebugSceneBuilder.cs の NullReferenceException が解消されている
  - [x] 130行目: `dialogueRunner` と `soRunner` の null チェックを追加
  - [x] 90行目: `chatController`、`soChat`、すべての `FindProperty()` の結果の null チェックを追加
  - [x] `soRunner.ApplyModifiedProperties()` を追加
- [x] 全てのコンパイルエラーが解消されている
- [x] 全ての NullReferenceException が解消されている
- [x] 警告が発生していない
- [x] `docs/inbox/` にレポート（REPORT_TASK_012_CompileErrorFix.md）が作成されている
- [x] 本チケットの Report 欄にレポートパスが追記されている

## 実装詳細

### 1. ScenarioManager.cs の修正

#### 問題
```csharp
private ChatController m_ChatController;
m_ChatController = FindFirstObjectByType<ChatController>();
```
- エラー: `CS1061: 'ChatController' does not contain a definition for 'AddMessage'`
- 原因: Unity のコンパイル順序の問題により、`ChatController` クラスが正しく解決されていない可能性

#### 修正方法
`ChatController` の型を完全修飾名（`ProjectFoundPhone.UI.ChatController`）で明示的に指定

```csharp
private ProjectFoundPhone.UI.ChatController m_ChatController;
m_ChatController = FindFirstObjectByType<ProjectFoundPhone.UI.ChatController>();
```

#### 確認事項
- [x] コンパイルエラーが解消されている
- [x] 型安全性が保たれている

### 2. DebugSceneBuilder.cs の修正

#### 問題1: 130行目の NullReferenceException
```csharp
SerializedObject soRunner = new SerializedObject(dialogueRunner);
soRunner.FindProperty("m_StartNode").stringValue = "Start";
```
- エラー: `NullReferenceException: Object reference not set to an instance of an object`
- 原因: `dialogueRunner` が null の場合や、`SerializedObject` の作成に失敗した場合の null チェック不足

#### 修正方法1
`dialogueRunner` と `soRunner` の null チェックを追加

```csharp
if (dialogueRunner == null)
{
    UnityEngine.Debug.LogError("DialogueRunner not found in scene.");
    return;
}

SerializedObject soRunner = new SerializedObject(dialogueRunner);
if (soRunner == null)
{
    UnityEngine.Debug.LogError("Failed to create SerializedObject for DialogueRunner.");
    return;
}

soRunner.FindProperty("m_StartNode").stringValue = "Start";
soRunner.ApplyModifiedProperties();
```

#### 問題2: 90行目の NullReferenceException
```csharp
SerializedProperty scrollRectProp = soChat.FindProperty("m_ScrollRect");
scrollRectProp.objectReferenceValue = scrollRect;
```
- エラー: `NullReferenceException: Object reference not set to an instance of an object`
- 原因: `FindProperty()` が null を返した場合に、その結果に対して `objectReferenceValue` を設定しようとして発生

#### 修正方法2
すべての `FindProperty()` の結果を null チェック

```csharp
if (chatController == null)
{
    UnityEngine.Debug.LogError("ChatController not found in scene.");
    return;
}

SerializedObject soChat = new SerializedObject(chatController);
if (soChat == null)
{
    UnityEngine.Debug.LogError("Failed to create SerializedObject for ChatController.");
    return;
}

SerializedProperty scrollRectProp = soChat.FindProperty("m_ScrollRect");
if (scrollRectProp != null)
{
    scrollRectProp.objectReferenceValue = scrollRect;
}
else
{
    UnityEngine.Debug.LogError("Failed to find property 'm_ScrollRect' in ChatController.");
}
```

#### 確認事項
- [x] NullReferenceException が解消されている
- [x] エラーハンドリングが適切に実装されている

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- Unity のコンパイル順序の問題を回避するため、完全修飾名を使用することが重要
- Null チェックを追加することで、実行時エラーを防止できる
