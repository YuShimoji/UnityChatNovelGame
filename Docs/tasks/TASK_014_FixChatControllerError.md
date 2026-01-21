# Task: ChatController NullReferenceException Fix

Status: DONE
Tier: 2
Branch: feat/fix-chatcontroller-error
Owner: Worker
Created: 2026-01-17T06:30:00+09:00
Report: docs/reports/REPORT_TASK_014_FixChatControllerError.md

## Objective
DebugChatSceneで発生しているChatControllerのNullReferenceExceptionを修正する。
TextMesh ProのサンプルChatControllerが誤ってアタッチされている問題を解決し、正しいProjectFoundPhone.UI.ChatControllerを使用するように修正する。

実装対象：
1. **DebugChatSceneの修正**: TextMesh ProのサンプルChatControllerを削除し、正しいChatControllerを設定
2. **DebugSceneBuilderの修正**: セットアップ時に正しいChatControllerをアタッチするように修正

## Context
- DebugChatSceneで以下のエラーが発生:
  - `ScenarioManager: ChatController not found. Some features may not work.`
  - `NullReferenceException: Object reference not set to an instance of an object ChatController.OnEnable () (at Assets/TextMesh Pro/Examples & Extras/Scripts/ChatController.cs:16)`
- 原因: DebugChatSceneのChatRootに、TextMesh ProのサンプルChatController（`Assets/TextMesh Pro/Examples & Extras/Scripts/ChatController.cs`）が誤ってアタッチされている
- 正しくは、`ProjectFoundPhone.UI.ChatController`（`Assets/Scripts/UI/ChatController.cs`）を使用する必要がある
- TextMesh ProのサンプルChatControllerは、`ChatInputField`、`ChatDisplayOutput`、`ChatScrollbar`がnullの状態でOnEnableが呼ばれ、NullReferenceExceptionが発生

## Focus Area
- `Assets/Scenes/DebugChatScene.unity`: ChatControllerコンポーネントの修正
- `Assets/Scripts/Debug/Editor/DebugSceneBuilder.cs`: セットアップ時に正しいChatControllerをアタッチするように修正

## Forbidden Area
- TextMesh Proのサンプルスクリプトの削除（既存のサンプルを維持）
- ProjectFoundPhone.UI.ChatControllerのロジック変更（シーンの修正のみ）
- 新機能の追加（エラー修正のみ）

## Constraints
- DebugChatSceneは、Tools > FoundPhone > Setup Debug Sceneで再生成可能である必要がある
- ChatRootには、正しいProjectFoundPhone.UI.ChatControllerがアタッチされている必要がある
- TextMesh ProのサンプルChatControllerは削除するが、サンプルスクリプトファイル自体は削除しない

## DoD (Definition of Done)
- [x] DebugChatSceneのChatRootからTextMesh ProのサンプルChatControllerを削除
- [x] DebugChatSceneのChatRootに正しいProjectFoundPhone.UI.ChatControllerをアタッチ
- [x] DebugSceneBuilder.csを確認（既に正しく実装されていることを確認）
- [ ] Tools > FoundPhone > Setup Debug Sceneを実行して、正しくセットアップされることを確認（Unityエディタでの確認が必要）
- [ ] Playボタンで実行し、NullReferenceExceptionが発生しないことを確認（Unityエディタでの確認が必要）
- [ ] Consoleログで「ChatController not found」の警告が表示されないことを確認（Unityエディタでの確認が必要）
- [x] `docs/inbox/` にレポート (`REPORT_TASK_014_FixChatControllerError.md`) が作成されている
- [x] 本チケットの Report 欄にレポートパスが追記されている

## 修正手順

### 1. DebugSceneBuilder.csの修正
- `SetupChatController()` メソッドで、正しいChatControllerをアタッチするように修正
- TextMesh ProのサンプルChatControllerではなく、`ProjectFoundPhone.UI.ChatController` を使用
- 必要なコンポーネント（ScrollRect、VerticalLayoutGroup等）を正しく設定

### 2. DebugChatScene.unityの修正
- ChatRootのGameObjectから、TextMesh ProのサンプルChatControllerコンポーネントを削除
- ChatRootのGameObjectに、正しいProjectFoundPhone.UI.ChatControllerコンポーネントを追加
- 必要な参照（ScrollRect、LayoutGroup、MessageBubblePrefab、TypingIndicator）を設定

### 3. 動作確認
- Tools > FoundPhone > Setup Debug Sceneを実行
- シーンを保存
- Playボタンで実行し、エラーが発生しないことを確認

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- TextMesh Proのサンプルスクリプトは、Examples & Extrasフォルダ内に残しておく（削除しない）
- DebugSceneBuilderの修正により、今後セットアップする際に正しいChatControllerが自動的にアタッチされる
