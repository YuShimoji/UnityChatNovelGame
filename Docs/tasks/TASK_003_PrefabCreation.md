# Task: Chat UI Prefab作成
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T09:15:00Z
Report: Docs/inbox/REPORT_TASK_003_PrefabCreation.md

## 停止理由（解消済み）
- **事実**: Unityエディタが起動していることを確認済み
- **根拠**: ユーザー確認により、Unityエディタが起動しており、コンパイルエラーも解消されている
- **次手**: TASK_003の実装を開始可能 

## Objective
ChatController.csで使用するPrefab（MessageBubble, TypingIndicator）を作成する。Unityエディタ上でPrefabを作成し、必要なコンポーネントを設定する。

実装対象：
1. **MessageBubble Prefab**: メッセージバブルのPrefab
2. **TypingIndicator Prefab**: タイピングインジケーターのPrefab

## Context
- 前タスク（TASK_002）でChatController.csの実装が完了
- ChatControllerは`m_MessageBubblePrefab`と`m_TypingIndicator`のPrefabを必要とする
- 必須パッケージ: TextMeshPro（既に前提）
- 参照ドキュメント: `最初のプロンプト`（プロジェクトルート）、`Docs/inbox/REPORT_TASK_002_LogicImplementation.md`

## Focus Area
- `Assets/Prefabs/UI/` 配下: MessageBubble.prefab, TypingIndicator.prefab
- UnityエディタでのPrefab作成とコンポーネント設定
- TextMeshProを使用したテキスト表示
- 9-Slice設定された背景画像（Sliced Sprite）
- ContentSizeFitterによる高さ自動調整
- 右寄せ/左寄せレイアウト対応

## Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更
- パッケージの追加（TextMeshProは既に前提）
- スクリプトの作成（Prefab作成のみ）
- Sceneの作成（Prefab作成のみ）
- アニメーションの作成（後続タスクへ分離）

## Constraints
- テスト: 主要パスのみ（網羅テストは後続タスクへ分離）
- フォールバック: 新規追加禁止（Prefab作成のみ）
- ディレクトリ構造: `Assets/Prefabs/UI/` を作成してからPrefabを配置
- コードスタイル: Unityエディタの標準的なPrefab作成手順に従う
- 命名規則: MessageBubble.prefab, TypingIndicator.prefab

## DoD
- [ ] MessageBubble.prefab が作成されている
  - [ ] TextMeshProUGUIコンポーネントが設定されている
  - [ ] ContentSizeFitterコンポーネントが設定されている（Vertical Fit: Preferred Size）
  - [ ] 背景Imageコンポーネントが設定されている（Sliced Sprite）
  - [ ] RectTransformの設定（Anchor/Pivotはスクリプトで動的に変更される想定）
- [ ] TypingIndicator.prefab が作成されている
  - [ ] 3点リーダーのアニメーション用コンポーネント（TextMeshProUGUIまたはImage）
  - [ ] アニメーション用のスクリプトまたはDOTween設定（後続タスクで実装予定の場合はプレースホルダー）
- [ ] Prefabが`Assets/Prefabs/UI/`配下に配置されている
- [ ] ChatController.csで参照可能な状態になっている
- [ ] docs/inbox/ にレポート（REPORT_TASK_003_PrefabCreation.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 実装詳細

### 1. MessageBubble Prefab

#### 構成要素
- **GameObject名**: MessageBubble
- **コンポーネント**:
  - `RectTransform`: UI要素の基本コンポーネント
  - `Image`: 背景画像（Sliced Sprite、9-Slice設定）
  - `TextMeshProUGUI`: メッセージテキスト表示
  - `ContentSizeFitter`: 高さ自動調整（Vertical Fit: Preferred Size）

#### 設定詳細
- **RectTransform**:
  - Width: 適切な幅（例: 300-400px）
  - Height: ContentSizeFitterで自動調整
  - Anchor: スクリプトで動的に変更されるため、初期値は任意
  - Pivot: スクリプトで動的に変更されるため、初期値は任意
- **Image (Background)**:
  - Image Type: Sliced
  - Source Image: 9-Slice設定された白い背景画像（後続タスクで着色）
  - Color: 白色（スクリプトで動的に着色）
- **TextMeshProUGUI**:
  - Text: プレースホルダーテキスト（"Message"など）
  - Font: TextMeshProのデフォルトフォントまたはプロジェクトフォント
  - Font Size: 適切なサイズ（例: 14-16px）
  - Alignment: Left（左寄せメッセージ用）、Right（右寄せメッセージ用）はスクリプトで設定
  - Overflow: Vertical Overflow: Overflow
- **ContentSizeFitter**:
  - Horizontal Fit: Unconstrained
  - Vertical Fit: Preferred Size

#### レイアウト
- TextMeshProUGUIはImageの子要素として配置
- Padding設定（例: 左右10px、上下8px）

### 2. TypingIndicator Prefab

#### 構成要素
- **GameObject名**: TypingIndicator
- **コンポーネント**:
  - `RectTransform`: UI要素の基本コンポーネント
  - `TextMeshProUGUI`または`Image`: 3点リーダー表示
  - （オプション）`DOTween Animation`: アニメーション用（後続タスクで実装予定の場合はプレースホルダー）

#### 設定詳細
- **RectTransform**:
  - Width: 適切な幅（例: 50-80px）
  - Height: 適切な高さ（例: 30-40px）
  - Anchor: 左下（左寄せメッセージ用）
- **TextMeshProUGUI**:
  - Text: "..."（3点リーダー）
  - Font Size: 適切なサイズ（例: 16-20px）
  - Alignment: Left
- **アニメーション**:
  - 後続タスクで実装予定の場合は、プレースホルダーとして静的表示のみ
  - または、DOTweenを使用した簡単なフェードイン/アウトアニメーションを実装

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- 9-Slice設定された背景画像が存在しない場合は、一時的に通常のSpriteを使用し、後続タスクで9-Slice画像を作成する旨をレポートに記録する
- アニメーションは後続タスクで実装予定の場合は、静的表示のみで対応し、後続タスクで実装する旨をレポートに記録する
