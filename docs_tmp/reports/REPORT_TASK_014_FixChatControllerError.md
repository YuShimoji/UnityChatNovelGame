# Report: TASK_014 ChatController NullReferenceException Fix

**作成日時**: 2026-01-17T06:45:00+09:00  
**タスク**: ChatController NullReferenceException Fix  
**ステータス**: COMPLETED  
**実行者**: AI Agent (Worker)

## 概要

DebugChatSceneで発生していたChatControllerのNullReferenceExceptionを修正しました。原因は、TextMesh ProのサンプルChatControllerが誤ってアタッチされていたことです。正しい`ProjectFoundPhone.UI.ChatController`を使用するように修正しました。

## 修正内容

### 1. DebugChatScene.unity の修正

#### 問題
- ChatRootのGameObjectに、TextMesh ProのサンプルChatController（GUID: `53d91f98a2664f5cb9af11de72ac54ec`）が誤ってアタッチされていた
- 正しい`ProjectFoundPhone.UI.ChatController`（GUID: `b687e66afed7f80428bce853284df875`）がアタッチされていなかった
- TextMesh ProのサンプルChatControllerは、`ChatInputField`、`ChatDisplayOutput`、`ChatScrollbar`がnullの状態でOnEnableが呼ばれ、NullReferenceExceptionが発生

#### 修正内容
1. **TextMesh ProのサンプルChatControllerの削除**
   - ChatRootのGameObject（fileID: 1937668978）のコンポーネントリストから、TextMesh ProのサンプルChatController（fileID: 1937668980）を削除
   - 該当するMonoBehaviourセクション（1937668980）を削除

2. **正しいChatControllerの追加**
   - ChatRootのGameObjectに、正しい`ProjectFoundPhone.UI.ChatController`コンポーネントを追加（fileID: 1937668982）
   - ChatRootのm_Componentリストに1937668982を追加

3. **参照の設定**
   - `m_ScrollRect`: ScrollRectコンポーネント（fileID: 1937668981）を参照
   - `m_LayoutGroup`: VerticalLayoutGroupコンポーネント（fileID: 110588022）を参照
   - `m_MessageBubblePrefab`: MessageBubble.prefab（GUID: `26b7b210afd94b68a720bb0a31a0b517`）を参照
   - `m_TypingIndicator`: 現在は{fileID: 0}（DebugSceneBuilder実行時に自動設定される）
   - `m_AutoScrollThreshold`: 0.1を設定
   - `m_EditorClassIdentifier`: `Assembly-CSharp::ProjectFoundPhone.UI.ChatController`を設定

#### 修正箇所
- `Assets/Scenes/DebugChatScene.unity`:
  - ChatRootのm_Componentリスト（687-690行目）: 1937668980を1937668982に変更
  - TextMesh ProのサンプルChatController（718-732行目）: 削除
  - 正しいChatController（728-734行目）: 追加

### 2. DebugSceneBuilder.cs の確認

#### 確認結果
- DebugSceneBuilder.csは既に正しく実装されていました
- `SetupScene()`メソッド（85行目）で、正しい`ProjectFoundPhone.UI.ChatController`を追加している
- 既存のChatRootを削除する際（44行目）、`DestroyImmediate(chatRoot)`により、すべてのコンポーネント（TextMesh ProのサンプルChatControllerを含む）が確実に削除される
- 必要な参照（ScrollRect、LayoutGroup、Prefabs等）を正しく設定している

#### 修正不要の理由
- 既存のChatRootを完全に削除してから新しいChatRootを作成するため、TextMesh ProのサンプルChatControllerが残る可能性はない
- 正しい`ProjectFoundPhone.UI.ChatController`を追加し、必要な参照を設定している

## エラー詳細

### NullReferenceException（修正前）

```
NullReferenceException: Object reference not set to an instance of an object
ChatController.OnEnable () (at Assets/TextMesh Pro/Examples & Extras/Scripts/ChatController.cs:16)
```

**原因**: TextMesh ProのサンプルChatControllerのOnEnable()メソッドで、`ChatInputField.onSubmit.AddListener(AddToChatOutput)`を呼び出しているが、`ChatInputField`がnullのため発生

### 警告メッセージ（修正前）

```
ScenarioManager: ChatController not found. Some features may not work.
```

**原因**: ScenarioManagerが`ProjectFoundPhone.UI.ChatController`を探しているが、TextMesh ProのサンプルChatControllerしか見つからなかったため

## 検証結果

### 修正後の状態
- ✅ DebugChatSceneのChatRootからTextMesh ProのサンプルChatControllerを削除
- ✅ DebugChatSceneのChatRootに正しいProjectFoundPhone.UI.ChatControllerをアタッチ
- ✅ 正しいChatControllerの参照（ScrollRect、LayoutGroup、MessageBubblePrefab）を設定
- ✅ DebugSceneBuilder.csは既に正しく実装されていることを確認

### 残作業（Unityエディタでの確認が必要）
- ⚠️ Tools > FoundPhone > Setup Debug Sceneを実行して、正しくセットアップされることを確認
- ⚠️ Playボタンで実行し、NullReferenceExceptionが発生しないことを確認
- ⚠️ Consoleログで「ChatController not found」の警告が表示されないことを確認
- ⚠️ TypingIndicatorの参照を設定（DebugSceneBuilder実行時に自動設定される）

## 技術的詳細

### GUID情報
- **TextMesh ProのサンプルChatController**: `53d91f98a2664f5cb9af11de72ac54ec`
- **正しいChatController**: `b687e66afed7f80428bce853284df875`
- **MessageBubble.prefab**: `26b7b210afd94b68a720bb0a31a0b517`
- **TypingIndicator.prefab**: `0fd4467a6a384bee9a3bdcb1e4557e38`

### FileID情報
- **ChatRoot GameObject**: 1937668978
- **ChatRoot RectTransform**: 1937668979
- **ScrollRect**: 1937668981
- **正しいChatController**: 1937668982
- **Viewport RectTransform**: 1350469799
- **Content RectTransform**: 110588020
- **VerticalLayoutGroup**: 110588022

## 影響範囲

### 修正されたファイル
- `Assets/Scenes/DebugChatScene.unity`: ChatControllerコンポーネントの修正

### 影響を受けないファイル
- `Assets/Scripts/Debug/Editor/DebugSceneBuilder.cs`: 既に正しく実装されているため修正不要
- `Assets/Scripts/UI/ChatController.cs`: ロジック変更なし
- `Assets/TextMesh Pro/Examples & Extras/Scripts/ChatController.cs`: サンプルスクリプトは削除せず維持

## 今後の対応

1. UnityエディタでDebugChatSceneを開き、修正が正しく反映されていることを確認
2. Tools > FoundPhone > Setup Debug Sceneを実行し、正しくセットアップされることを確認
3. Playボタンで実行し、NullReferenceExceptionが発生しないことを確認
4. Consoleログで「ChatController not found」の警告が表示されないことを確認
5. TypingIndicatorの参照が正しく設定されていることを確認（DebugSceneBuilder実行時に自動設定される）

## 備考

- TextMesh Proのサンプルスクリプトは、Examples & Extrasフォルダ内に残しておく（削除しない）
- DebugSceneBuilderの修正により、今後セットアップする際に正しいChatControllerが自動的にアタッチされる
- シーンファイル内でPrefabの参照を直接設定するのは複雑なため、TypingIndicatorの参照はDebugSceneBuilder実行時に自動設定される
