# Task: DebugSceneBuilder Reflection Error Fix

Status: DONE
Tier: 2
Branch: feat/fix-debug-scene-builder-reflection
Owner: Worker
Created: 2026-01-17T07:00:00+09:00
Report: docs/inbox/REPORT_TASK_015_FixDebugSceneBuilderReflection.md

## Objective
DebugSceneBuilderのセットアップ時に発生しているリフレクションエラーを修正する。
ChatControllerのフィールド（m_ScrollRect, m_LayoutGroup, m_MessageBubblePrefab, m_TypingIndicator）がリフレクションで見つからない問題と、DialogueRunnerの'startAutomatically'プロパティが見つからない問題を解決する。

実装対象：
1. **DebugSceneBuilder.csの修正**: ChatControllerのリフレクション処理を修正し、正しいフィールドを取得できるようにする
2. **DialogueRunnerのプロパティ設定修正**: 'startAutomatically'プロパティの設定方法を修正する

## Context
- DebugSceneBuilder実行時に以下のエラーが発生:
  - `DebugSceneBuilder: Failed to find m_ScrollRect field via reflection. Available fields: ChatInputField (TMP_InputField), ChatDisplayOutput (TMP_Text), ChatScrollbar (Scrollbar)`
  - `DebugSceneBuilder: Failed to find m_LayoutGroup field via reflection. Available fields: ChatInputField (TMP_InputField), ChatDisplayOutput (TMP_Text), ChatScrollbar (Scrollbar)`
  - `DebugSceneBuilder: Failed to find m_MessageBubblePrefab field via reflection. Available fields: ChatInputField (TMP_InputField), ChatDisplayOutput (TMP_Text), ChatScrollbar (Scrollbar)`
  - `DebugSceneBuilder: Failed to find m_TypingIndicator field via reflection. Available fields: ChatInputField (TMP_InputField), ChatDisplayOutput (TMP_Text), ChatScrollbar (Scrollbar)`
  - `DialogueRunner: 'startAutomatically' property not found. Dialogue will not start automatically.`
- 原因: DebugSceneBuilderが取得しているChatControllerが、TextMesh ProのサンプルChatController（`Assets/TextMesh Pro/Examples & Extras/Scripts/ChatController.cs`）になっている可能性がある
- 正しくは、`ProjectFoundPhone.UI.ChatController`（`Assets/Scripts/UI/ChatController.cs`を使用する必要がある
- ChatController.csには、m_ScrollRect, m_LayoutGroup, m_MessageBubblePrefab, m_TypingIndicatorのフィールドが存在する（[SerializeField] privateフィールド）

## Focus Area
- `Assets/Scripts/Debug/Editor/DebugSceneBuilder.cs`: リフレクション処理の修正
  - ChatControllerの取得方法を確認・修正
  - 正しい`ProjectFoundPhone.UI.ChatController`を取得していることを確認
  - リフレクションのBindingFlagsを確認・修正
  - DialogueRunnerのプロパティ設定方法を修正

## Forbidden Area
- ChatController.csのロジック変更（リフレクション処理の修正のみ）
- DialogueRunnerの内部実装変更（プロパティ設定方法の修正のみ）
- 新機能の追加（エラー修正のみ）

## Constraints
- DebugSceneBuilderは、Tools > FoundPhone > Setup Debug Sceneで実行可能である必要がある
- ChatControllerのフィールドは、[SerializeField] privateフィールドとして定義されているため、リフレクションでアクセス可能である必要がある
- DialogueRunnerの'startAutomatically'プロパティは、Yarn Spinnerのバージョンによって異なる可能性がある

## DoD (Definition of Done)
- [ ] DebugSceneBuilderが正しい`ProjectFoundPhone.UI.ChatController`を取得していることを確認
- [ ] ChatControllerのフィールド（m_ScrollRect, m_LayoutGroup, m_MessageBubblePrefab, m_TypingIndicator）がリフレクションで正しく取得できることを確認
- [ ] リフレクション処理でフィールドが正しく設定されることを確認
- [ ] DialogueRunnerの'startAutomatically'プロパティが正しく設定されることを確認（または、プロパティが存在しない場合は適切な代替手段を実装）
- [ ] Tools > FoundPhone > Setup Debug Sceneを実行して、エラーが発生しないことを確認（Unityエディタでの確認が必要）
- [ ] Consoleログで「Failed to find ... field via reflection」のエラーが表示されないことを確認（Unityエディタでの確認が必要）
- [ ] Consoleログで「'startAutomatically' property not found」の警告が表示されないことを確認（Unityエディタでの確認が必要）
- [ ] `docs/inbox/` にレポート (`REPORT_TASK_015_FixDebugSceneBuilderReflection.md`) が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 修正手順

### 1. ChatControllerの取得確認
- `chatRoot.AddComponent<ChatController>()`で追加したChatControllerが、正しい`ProjectFoundPhone.UI.ChatController`であることを確認
- `chatController.GetType()`の結果が`ProjectFoundPhone.UI.ChatController`であることを確認
- もしTextMesh ProのサンプルChatControllerが取得されている場合は、コンポーネントの削除・追加処理を修正

### 2. リフレクション処理の修正
- BindingFlagsを確認: `BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public`が正しいか確認
- フィールド名の確認: `m_ScrollRect`, `m_LayoutGroup`, `m_MessageBubblePrefab`, `m_TypingIndicator`が正しいか確認
- フィールドの取得方法を修正: `GetField()`の代わりに、`GetFields()`で全フィールドを取得し、名前で検索する方法を検討
- SerializeField属性の確認: フィールドに`[SerializeField]`属性が付いていることを確認

### 3. DialogueRunnerのプロパティ設定修正
- Yarn Spinnerのバージョンを確認
- 'startAutomatically'プロパティが存在するか確認
- 存在しない場合は、代替手段（例: `StartDialogue()`メソッドの呼び出し）を実装
- SerializedObjectを使用したプロパティ設定方法を確認・修正

### 4. 動作確認
- Tools > FoundPhone > Setup Debug Sceneを実行
- Consoleログでエラーが発生しないことを確認
- シーンを保存
- Playボタンで実行し、正常に動作することを確認

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- リフレクション処理は、Unityエディタでのみ実行されるため、`#if UNITY_EDITOR`ディレクティブは不要（既にEditorスクリプト内）
- ChatControllerのフィールドは、[SerializeField] privateフィールドとして定義されているため、リフレクションでアクセス可能である必要がある
- DialogueRunnerのプロパティ名は、Yarn Spinnerのバージョンによって異なる可能性があるため、バージョンに応じた対応が必要
