# Worker Prompt: TASK_003_PrefabCreation

## 参照
- チケット: Docs/tasks/TASK_003_PrefabCreation.md
- SSOT: Docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: Docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- 前タスクレポート: Docs/inbox/REPORT_TASK_002_LogicImplementation.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `Assets/Prefabs/UI/` 配下: MessageBubble.prefab, TypingIndicator.prefab
- UnityエディタでのPrefab作成とコンポーネント設定
- TextMeshProを使用したテキスト表示
- 9-Slice設定された背景画像（Sliced Sprite）
- ContentSizeFitterによる高さ自動調整
- 右寄せ/左寄せレイアウト対応

### Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更
- パッケージの追加（TextMeshProは既に前提）
- スクリプトの作成（Prefab作成のみ）
- Sceneの作成（Prefab作成のみ）
- アニメーションの作成（後続タスクへ分離）

## Tier / Branch
- Tier: 2（機能実装）
- Branch: main

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

## 停止条件
- Forbidden Area に触れないと完遂できない
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない
- 長時間待機が必要（定義したタイムアウト超過）
- Unityエディタが起動していない、またはPrefab作成が不可能な環境

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_003_PrefabCreation.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_003_PrefabCreation.md

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

## コーディング規約
- Unityエディタの標準的なPrefab作成手順に従う
- Prefab名: MessageBubble.prefab, TypingIndicator.prefab
- ディレクトリ構造: `Assets/Prefabs/UI/` を作成してからPrefabを配置

## 参考情報
- 前タスクレポート: `Docs/inbox/REPORT_TASK_002_LogicImplementation.md` を参照
- プロジェクト仕様: `最初のプロンプト`（プロジェクトルート）を参照
- Unityバージョン: Unity 6 (or 2022 LTS)
- 必須パッケージ: TextMeshPro
- ChatController.cs: `Assets/Scripts/UI/ChatController.cs` を参照（Prefabの使用方法）

## 注意事項
1. **9-Slice画像**: 9-Slice設定された背景画像が存在しない場合は、一時的に通常のSpriteを使用し、後続タスクで9-Slice画像を作成する旨をレポートに記録してください。
2. **アニメーション**: TypingIndicatorのアニメーションは後続タスクで実装予定の場合は、静的表示のみで対応し、後続タスクで実装する旨をレポートに記録してください。
3. **Prefab配置**: Prefabは`Assets/Prefabs/UI/`配下に配置し、ChatController.csのInspectorから参照可能な状態にしてください。
4. **Unityエディタ**: Unityエディタが起動していない場合は、Prefab作成が不可能なため、BLOCKEDとして報告してください。
