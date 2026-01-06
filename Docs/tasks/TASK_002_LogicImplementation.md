# Task: ChatController & ScenarioManager ロジック実装
Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T08:15:00Z
Report: Docs/inbox/REPORT_TASK_002_LogicImplementation.md 

## Objective
ChatController.cs と ScenarioManager.cs のTODOコメントに記載されたロジックを実装する。スケルトンコードで定義されたメソッドの実装を完了させ、基本的な動作を可能にする。

実装対象：
1. **ChatController.cs**: メッセージ表示、スクロール制御、タイピングインジケーター制御のロジック
2. **ScenarioManager.cs**: Yarn Spinner連携、カスタムコマンドハンドラの実装

## Context
- 前タスク（TASK_001）でスケルトンコードが作成済み
- ChatController.cs と ScenarioManager.cs に多数のTODOコメントが残っている
- 必須パッケージ: Yarn Spinner, DOTween Pro, TextMeshPro（既に前提）
- 参照ドキュメント: `最初のプロンプト`（プロジェクトルート）、`Docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md`

## Focus Area
- `Assets/Scripts/UI/ChatController.cs` のTODO実装
- `Assets/Scripts/Core/ScenarioManager.cs` のTODO実装
- Unity C# コーディング規約（PascalCase, camelCase, #region使用）
- SOLID原則に基づく設計の維持
- DOTween Pro を使用したアニメーション実装
- Yarn Spinner API の適切な使用

## Forbidden Area
- 既存ファイルの削除・破壊的変更（TODO実装のみ）
- Unityプロジェクト設定の変更
- パッケージの追加（Yarn Spinner, DOTween, TextMeshProは既に前提）
- PrefabやSceneの作成（後続タスクへ分離）
- テストコードの作成（後続タスクへ分離）
- 完全なエラーハンドリング（主要パスのみ実装）

## Constraints
- テスト: 主要パスのみ（網羅テストは後続タスクへ分離）
- フォールバック: 新規追加禁止（既存ファイルのTODO実装のみ）
- コードスタイル: Unity C# ベストプラクティスに従う
- 命名規則: 変数名は m_VariableName, 定数は c_ConstantName, 静的は s_StaticName
- Yarn Spinner API: 最新のドキュメントを参照（バージョン依存の可能性あり）

## DoD
- [x] ChatController.cs の全TODOコメントが実装されている
  - [x] InitializeComponents() のnullチェックと警告
  - [x] CheckUserScrollInput() のスクロール位置監視
  - [x] CreateMessageBubble() のPrefabインスタンス化とレイアウト設定
  - [x] AddMessage() のメッセージ追加ロジック
  - [x] ShowTypingIndicator() のAutoScroll連携
  - [x] AutoScroll() のスクロールアニメーション（DOTween使用）
  - [x] ClearMessages() の履歴クリア処理
- [x] ScenarioManager.cs の全TODOコメントが実装されている
  - [x] RegisterCustomCommands() のコマンド登録（Yarn Spinner API使用）
  - [x] UnregisterCustomCommands() のコマンド解除
  - [x] ImageCommand() の画像読み込みと送信
  - [x] StartWaitCommand() の待機処理と入力ロック
  - [x] UnlockTopicCommand() のトピック解放とYarn変数更新
  - [x] GlitchCommand() のグリッチ演出（MetaEffectController連携は後続タスク）
  - [x] StartScenario() のDialogueRunner起動
  - [x] StopScenario() のDialogueRunner停止
  - [x] GetVariable/SetVariable() のVariableStorage操作
- [x] 全ての実装がSOLID原則に基づいている
- [x] 主要パスの動作確認が完了している
- [x] docs/inbox/ にレポート（REPORT_TASK_002_LogicImplementation.md）が作成されている
- [x] 本チケットの Report 欄にレポートパスが追記されている

## 実装詳細

### ChatController.cs の実装項目

#### 1. InitializeComponents()
- m_MessageBubblePrefab、m_TypingIndicatorのnullチェック
- nullの場合の警告ログ出力

#### 2. CheckUserScrollInput()
- ScrollRect.verticalNormalizedPositionを監視
- スクロール位置が下から一定以上離れている場合、m_IsUserScrolling = true
- 閾値: m_AutoScrollThreshold（デフォルト0.1）

#### 3. CreateMessageBubble(string charID, string text)
- m_MessageBubblePrefabからインスタンスを生成
- charIDに応じて右寄せ/左寄せを設定（"player"の場合は右寄せ、それ以外は左寄せ）
- TextMeshProコンポーネントにtextを設定
- ContentSizeFitterで高さを自動調整
- m_ScrollRect.contentの子として追加

#### 4. AddMessage(string charID, string text)
- CreateMessageBubble()でメッセージバブルを生成
- m_ScrollRect.contentの子として追加
- m_IsUserScrollingがfalseの場合のみAutoScroll()を実行

#### 5. ShowTypingIndicator(bool show)
- 表示時はAutoScroll()を実行してインジケーターが見えるようにする

#### 6. AutoScroll()
- ScrollRect.verticalNormalizedPositionを1.0に設定
- DOTweenを使用したスクロールアニメーション（0.3秒程度）
- スクロール完了後にm_LastScrollPositionを更新
- m_IsUserScrollingがtrueの場合は実行しない

#### 7. ClearMessages()
- m_ScrollRect.contentの子オブジェクト（メッセージバブル）を全て削除
- Destroy()を使用

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

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- Yarn SpinnerのAPIが想定と異なる場合は、最新のドキュメントを参照して適切に実装する
- MetaEffectControllerやDeductionBoardが未実装の場合は、Debug.Logで代替し、後続タスクで実装する旨をレポートに記録する
- Prefab（MessageBubble, TypingIndicator）が未作成の場合は、nullチェックを適切に行い、警告ログを出力する
