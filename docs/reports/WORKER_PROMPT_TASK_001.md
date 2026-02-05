# Worker Prompt: TASK_001_UnityCoreSystemSkeleton

## 参照
- チケット: docs/tasks/TASK_001_UnityCoreSystemSkeleton.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `Assets/Scripts/Data/` 配下: TopicData.cs, SynthesisRecipe.cs
- `Assets/Scripts/UI/` 配下: ChatController.cs
- `Assets/Scripts/Core/` 配下: ScenarioManager.cs
- Unity C# コーディング規約（PascalCase, camelCase, #region使用）
- SOLID原則に基づく設計
- スケルトンコードのみ（ロジックはTODOコメント）

### Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定の変更
- パッケージの追加（Yarn Spinner, DOTween, TextMeshProは既に前提）
- ロジックの完全実装（スケルトンコードのみ）
- PrefabやSceneの作成
- テストコードの作成（後続タスクへ分離）

## Tier / Branch
- Tier: 2（機能実装）
- Branch: main

## DoD
- [ ] TopicData.cs が作成されている（ScriptableObject、ID, Icon, Title, Description）
- [ ] SynthesisRecipe.cs が作成されている（ScriptableObject、Topic A + Topic B = Topic C）
- [ ] ChatController.cs が作成されている（ScrollRect, VerticalLayoutGroup, ContentSizeFitter使用、Typing Indicator, Auto Scroll）
- [ ] ScenarioManager.cs が作成されている（Yarn Spinner DialogueRunnerラップ、カスタムコマンドハンドラ）
- [ ] 全てのクラスがSOLID原則に基づいて設計されている
- [ ] 主要メソッドと変数が定義されている（ロジックはTODOコメント）
- [ ] docs/inbox/ にレポート（REPORT_TASK_001_UnityCoreSystemSkeleton.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 停止条件
- Forbidden Area に触れないと完遂できない
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない
- 長時間待機が必要（定義したタイムアウト超過）

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md

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

## コーディング規約
- 変数名: m_VariableName（private field）
- 定数: c_ConstantName
- 静的: s_StaticName
- クラス/メソッド: PascalCase
- #region を使用してコードを整理
- [SerializeField] でprivate fieldをInspectorに表示
- Unity C# ベストプラクティスに従う

## 参考情報
- プロジェクト仕様: `最初のプロンプト`（プロジェクトルート）を参照
- Unityバージョン: Unity 6 (or 2022 LTS)
- 必須パッケージ: Yarn Spinner, DOTween Pro, TextMeshPro
- アーキテクチャ: MVCパターン
