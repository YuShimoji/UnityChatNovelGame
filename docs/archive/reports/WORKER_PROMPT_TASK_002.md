# Worker Prompt: TASK_002_LogicImplementation

## 参照
- チケット: docs/tasks/TASK_002_LogicImplementation.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- 前タスクレポート: docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `Assets/Scripts/UI/ChatController.cs` のTODO実装
- `Assets/Scripts/Core/ScenarioManager.cs` のTODO実装
- Unity C# コーディング規約（PascalCase, camelCase, #region使用）
- SOLID原則に基づく設計の維持
- DOTween Pro を使用したアニメーション実装
- Yarn Spinner API の適切な使用

### Forbidden Area
- 既存ファイルの削除・破壊的変更（TODO実装のみ）
- Unityプロジェクト設定の変更
- パッケージの追加（Yarn Spinner, DOTween, TextMeshProは既に前提）
- PrefabやSceneの作成（後続タスクへ分離）
- テストコードの作成（後続タスクへ分離）
- 完全なエラーハンドリング（主要パスのみ実装）

## Tier / Branch
- Tier: 2（機能実装）
- Branch: main

## DoD
- [ ] ChatController.cs の全TODOコメントが実装されている
  - [ ] InitializeComponents() のnullチェックと警告
  - [ ] CheckUserScrollInput() のスクロール位置監視
  - [ ] CreateMessageBubble() のPrefabインスタンス化とレイアウト設定
  - [ ] AddMessage() のメッセージ追加ロジック
  - [ ] ShowTypingIndicator() のAutoScroll連携
  - [ ] AutoScroll() のスクロールアニメーション（DOTween使用）
  - [ ] ClearMessages() の履歴クリア処理
- [ ] ScenarioManager.cs の全TODOコメントが実装されている
  - [ ] RegisterCustomCommands() のコマンド登録（Yarn Spinner API使用）
  - [ ] UnregisterCustomCommands() のコマンド解除
  - [ ] ImageCommand() の画像読み込みと送信
  - [ ] StartWaitCommand() の待機処理と入力ロック
  - [ ] UnlockTopicCommand() のトピック解放とYarn変数更新
  - [ ] GlitchCommand() のグリッチ演出（MetaEffectController連携は後続タスク）
  - [ ] StartScenario() のDialogueRunner起動
  - [ ] StopScenario() のDialogueRunner停止
  - [ ] GetVariable/SetVariable() のVariableStorage操作
- [ ] 全ての実装がSOLID原則に基づいている
- [ ] 主要パスの動作確認が完了している
- [ ] docs/inbox/ にレポート（REPORT_TASK_002_LogicImplementation.md）が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 停止条件
- Forbidden Area に触れないと完遂できない
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない
- 長時間待機が必要（定義したタイムアウト超過）
- Yarn Spinner APIが想定と大きく異なり、実装方針の再検討が必要

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_002_LogicImplementation.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_002_LogicImplementation.md

## 実装詳細

### ChatController.cs の実装項目

#### 1. InitializeComponents()
- m_MessageBubblePrefab、m_TypingIndicatorのnullチェック
- nullの場合の警告ログ出力（Debug.LogWarning）

#### 2. CheckUserScrollInput()
- ScrollRect.verticalNormalizedPositionを監視
- スクロール位置が下から一定以上離れている場合、m_IsUserScrolling = true
- 閾値: m_AutoScrollThreshold（デフォルト0.1）
- スクロール位置が1.0に近い場合、m_IsUserScrolling = false

#### 3. CreateMessageBubble(string charID, string text)
- m_MessageBubblePrefabからインスタンスを生成（Instantiate）
- charIDに応じて右寄せ/左寄せを設定（"player"の場合は右寄せ、それ以外は左寄せ）
- TextMeshProコンポーネントにtextを設定
- ContentSizeFitterで高さを自動調整
- m_ScrollRect.contentの子として追加（SetParent）

#### 4. AddMessage(string charID, string text)
- CreateMessageBubble()でメッセージバブルを生成
- m_ScrollRect.contentの子として追加
- m_IsUserScrollingがfalseの場合のみAutoScroll()を実行

#### 5. ShowTypingIndicator(bool show)
- 表示時はAutoScroll()を実行してインジケーターが見えるようにする

#### 6. AutoScroll()
- ScrollRect.verticalNormalizedPositionを1.0に設定
- DOTweenを使用したスクロールアニメーション（0.3秒程度）
  - DOTween.To(() => m_ScrollRect.verticalNormalizedPosition, x => m_ScrollRect.verticalNormalizedPosition = x, 1.0f, 0.3f)
- スクロール完了後にm_LastScrollPositionを更新
- m_IsUserScrollingがtrueの場合は実行しない

#### 7. ClearMessages()
- m_ScrollRect.contentの子オブジェクト（メッセージバブル）を全て削除
- Destroy()を使用（foreachでループ）

### ScenarioManager.cs の実装項目

#### 1. RegisterCustomCommands()
- DialogueRunner.AddCommandHandler()を使用してコマンドを登録
- 登録するコマンド:
  - "Message": MessageCommand(string, string)
  - "Image": ImageCommand(string, string)
  - "StartWait": StartWaitCommand(int)
  - "UnlockTopic": UnlockTopicCommand(string)
  - "Glitch": GlitchCommand(int)
- Yarn SpinnerのAPIはバージョン依存の可能性があるため、最新ドキュメントを参照
- コマンドハンドラのシグネチャはYarn Spinnerの要件に従う

#### 2. UnregisterCustomCommands()
- 登録したコマンドハンドラを解除
- DialogueRunner.RemoveCommandHandler()を使用

#### 3. ImageCommand(string charID, string imageID)
- Resources.Load<Sprite>($"Images/{imageID}")で画像を読み込み
- ChatControllerに画像メッセージとして送信（AddMessage()の拡張または新規メソッド）
- 読み込み失敗時は警告ログを出力

#### 4. StartWaitCommand(int seconds)
- m_ChatController.ShowTypingIndicator(true)でタイピングインジケーターを表示
- 入力ロックを有効化（DialogueRunnerの入力無効化または専用フラグ）
- CoroutineまたはDOTween.DelayedCall()で指定秒数待機
- 待機解除後、タイピングインジケーターを非表示（ShowTypingIndicator(false)）

#### 5. UnlockTopicCommand(string topicID)
- Resources.Load<TopicData>($"Topics/{topicID}")でTopicDataを読み込み
- 推論ボード（DeductionBoard）にトピックを追加（DeductionBoardは後続タスクで実装予定）
- Yarn変数を更新: SetVariable($"has_topic_{topicID}", true)
- 読み込み失敗時は警告ログを出力

#### 6. GlitchCommand(int level)
- MetaEffectControllerまたは専用のGlitchEffectコンポーネントにグリッチ効果を要求
- レベルに応じた強度でノイズを表示
- MetaEffectControllerが未実装の場合は、Debug.Logのみで対応（後続タスクで実装）

#### 7. StartScenario(string nodeName)
- DialogueRunner.StartDialogue(nodeName)を呼び出し
- nodeNameがnullの場合はm_StartNodeを使用

#### 8. StopScenario()
- DialogueRunner.Stop()を呼び出し

#### 9. GetVariable<T>(string variableName)
- DialogueRunner.VariableStorageから変数を取得
- VariableStorage.GetValue()を使用

#### 10. SetVariable<T>(string variableName, T value)
- DialogueRunner.VariableStorageに変数を設定
- VariableStorage.SetValue()を使用

## コーディング規約
- 変数名: m_VariableName（private field）
- 定数: c_ConstantName
- 静的: s_StaticName
- クラス/メソッド: PascalCase
- #region を使用してコードを整理
- [SerializeField] でprivate fieldをInspectorに表示
- Unity C# ベストプラクティスに従う

## 参考情報
- 前タスクレポート: `docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md` を参照
- プロジェクト仕様: `最初のプロンプト`（プロジェクトルート）を参照
- Unityバージョン: Unity 6 (or 2022 LTS)
- 必須パッケージ: Yarn Spinner, DOTween Pro, TextMeshPro
- Yarn Spinner API: 最新のドキュメントを参照（バージョン依存の可能性あり）

## 注意事項
1. **Yarn Spinner API**: DialogueRunner.AddCommandHandler()のシグネチャはバージョンによって異なる可能性があります。実装時は最新のドキュメントを参照してください。
2. **Prefab依存**: ChatControllerはm_MessageBubblePrefabとm_TypingIndicatorのPrefabが必要です。これらは後続タスクで作成される予定ですが、nullチェックを適切に行い、警告ログを出力してください。
3. **MetaEffectController**: GlitchCommand()の実装時、MetaEffectControllerが未実装の場合はDebug.Logのみで対応し、後続タスクで実装する旨をレポートに記録してください。
4. **DeductionBoard**: UnlockTopicCommand()の実装時、DeductionBoardが未実装の場合はDebug.Logのみで対応し、後続タスクで実装する旨をレポートに記録してください。
