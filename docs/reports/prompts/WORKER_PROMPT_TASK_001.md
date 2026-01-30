# Worker Prompt: TASK_001_UnityCoreSystemSkeleton

## 参�E
- チケチE��: docs/tasks/TASK_001_UnityCoreSystemSkeleton.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- プロジェクト仕槁E 最初�Eプロンプト�E��Eロジェクトルート！E
## 墁E��

### Focus Area
- `Assets/Scripts/Data/` 配丁E TopicData.cs, SynthesisRecipe.cs
- `Assets/Scripts/UI/` 配丁E ChatController.cs
- `Assets/Scripts/Core/` 配丁E ScenarioManager.cs
- Unity C# コーチE��ング規紁E��EascalCase, camelCase, #region使用�E�E- SOLID原則に基づく設訁E- スケルトンコード�Eみ�E�ロジチE��はTODOコメント！E
### Forbidden Area
- 既存ファイルの削除・破壊的変更
- Unityプロジェクト設定�E変更
- パッケージの追加�E�Earn Spinner, DOTween, TextMeshProは既に前提�E�E- ロジチE��の完�E実裁E��スケルトンコード�Eみ�E�E- PrefabやSceneの作�E
- チE��トコード�E作�E�E�後続タスクへ刁E���E�E
## Tier / Branch
- Tier: 2�E�機�E実裁E��E- Branch: main

## DoD
- [ ] TopicData.cs が作�EされてぁE���E�EcriptableObject、ID, Icon, Title, Description�E�E- [ ] SynthesisRecipe.cs が作�EされてぁE���E�EcriptableObject、Topic A + Topic B = Topic C�E�E- [ ] ChatController.cs が作�EされてぁE���E�EcrollRect, VerticalLayoutGroup, ContentSizeFitter使用、Typing Indicator, Auto Scroll�E�E- [ ] ScenarioManager.cs が作�EされてぁE���E�Earn Spinner DialogueRunnerラチE�E、カスタムコマンドハンドラ�E�E- [ ] 全てのクラスがSOLID原則に基づぁE��設計されてぁE��
- [ ] 主要メソチE��と変数が定義されてぁE���E�ロジチE��はTODOコメント！E- [ ] docs/inbox/ にレポ�Eト！EEPORT_TASK_001_UnityCoreSystemSkeleton.md�E�が作�EされてぁE��
- [ ] 本チケチE��の Report 欁E��レポ�Eトパスが追記されてぁE��

## 停止条件
- Forbidden Area に触れなぁE��完遂できなぁE- 仕様�E仮定が 3 つ以上忁E��E- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不�Eでの push が忁E��E- SSOT不足めE`ensure-ssot.js` で解決できなぁE- 長時間征E��が忁E��E��定義したタイムアウト趁E���E�E
停止時�E以下を実施�E�E1. チケチE��のStatusをBLOCKEDに更新
2. 事宁E根拠/次手（候補）をチケチE��本斁E��追訁E3. docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md を作�Eし、停止琁E��を記録
4. チケチE��のReport欁E��レポ�Eトパスを追訁E
## 納品允E- docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md

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

## コーチE��ング規紁E- 変数吁E m_VariableName�E�Erivate field�E�E- 定数: c_ConstantName
- 静的: s_StaticName
- クラス/メソチE��: PascalCase
- #region を使用してコードを整琁E- [SerializeField] でprivate fieldをInspectorに表示
- Unity C# ベスト�EラクチE��スに従う

## 参老E��報
- プロジェクト仕槁E `最初�Eプロンプト`�E��Eロジェクトルート）を参�E
- Unityバ�Eジョン: Unity 6 (or 2022 LTS)
- 忁E��パチE��ージ: Yarn Spinner, DOTween Pro, TextMeshPro
- アーキチE��チャ: MVCパターン
