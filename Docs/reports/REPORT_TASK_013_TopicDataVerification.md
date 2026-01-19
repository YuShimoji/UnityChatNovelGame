# Report: TASK_013_TopicDataVerification

**作成日時**: 2026-01-17T06:30:00+09:00  
**タスク**: TASK_013_TopicDataVerification  
**ステータス**: IN_PROGRESS（コード実装完了、Unity Editor実行待ち）  
**実行者**: AI Agent (Worker)

## 概要

TASK_011で作成したTopicDataアセットの動作確認とEvidence取得を行うためのツールを実装しました。Inspector表示のスクリーンショット取得用エディタスクリプトを作成し、UnlockTopicCommandの動作確認準備を完了しました。

## 実装ファイル一覧

### 1. TopicAssetScreenshotTool.cs
- **パス**: `Assets/Scripts/Debug/Editor/TopicAssetScreenshotTool.cs`
- **役割**: TopicDataアセットのInspector表示スクリーンショットを取得するエディタツール
- **実装内容**:
  - `CaptureTopicAssetScreenshot()`: アセットを選択してスクリーンショットを取得
  - `SelectTopicAsset()`: アセットを選択してInspectorに表示（準備用）
  - `docs/evidence/` ディレクトリの自動作成
  - Unity Editor メニューから実行可能（`Tools/FoundPhone/Capture Topic Asset Screenshot`）

### 2. docs/evidence/ ディレクトリ
- **パス**: `docs/evidence/`
- **役割**: Evidence（スクリーンショット等）の保存先
- **状態**: 作成済み

## 実装詳細

### エディタスクリプトの機能

#### 1. CaptureTopicAssetScreenshot()
- **メニューパス**: `Tools/FoundPhone/Capture Topic Asset Screenshot`
- **機能**:
  - `Assets/Resources/Topics/debug_topic_01.asset` を読み込み
  - アセットを選択してInspectorに表示
  - `docs/evidence/` ディレクトリが存在しない場合は自動作成
  - スクリーンショットを `docs/evidence/task011_topic_assets.png` として保存
  - Unity 2022.1以降では自動スクリーンショット取得、それ以前では手動取得を案内

#### 2. SelectTopicAsset()
- **メニューパス**: `Tools/FoundPhone/Select Topic Asset for Screenshot`
- **機能**:
  - `Assets/Resources/Topics/debug_topic_01.asset` を選択
  - Inspectorウィンドウに表示されるようにする
  - スクリーンショット取得前の準備として使用

### UnlockTopicCommandの動作確認準備

#### コードレベルでの確認結果

1. **DebugScript.yarn の確認**
   - **パス**: `Assets/Resources/Yarn/DebugScript.yarn`
   - **18行目**: `<<UnlockTopic "debug_topic_01">>` コマンドが含まれている
   - **状態**: ✅ 正常

2. **ScenarioManager.cs の確認**
   - **パス**: `Assets/Scripts/Core/ScenarioManager.cs`
   - **214-234行目**: `UnlockTopicCommand` メソッドが実装されている
   - **実装内容**:
     - `Resources.Load<TopicData>($"Topics/{topicID}")` でトピックを読み込み
     - 読み込み成功時: `Debug.Log($"ScenarioManager: Topic unlocked - {topicData.Title} (ID: {topicID})");` を出力
     - Yarn変数 `$has_topic_{topicID}` を `true` に設定
   - **状態**: ✅ 正常

3. **コマンド登録の確認**
   - **92行目**: `m_DialogueRunner.AddCommandHandler<string>("UnlockTopic", UnlockTopicCommand);` で登録されている
   - **状態**: ✅ 正常

## 動作確認方法（詳細手順）

### 1. Inspector表示スクリーンショットの取得

#### 方法1: エディタスクリプトを使用（推奨）

1. Unity Editor を起動
2. Unity Editor のメニューバーから `Tools > FoundPhone > Select Topic Asset for Screenshot` を選択
   - `debug_topic_01.asset` が選択され、Inspectorウィンドウに表示される
3. Inspectorウィンドウが表示されていることを確認
4. Unity Editor のメニューバーから `Tools > FoundPhone > Capture Topic Asset Screenshot` を選択
   - Unity 2022.1以降: 自動的にスクリーンショットが保存される
   - Unity 2022.1未満: 手動取得の案内が表示される

#### 方法2: 手動取得

1. Unity Editor で `Assets/Resources/Topics/debug_topic_01.asset` を選択
2. Inspectorウィンドウでトピック情報を表示
3. Windows: `Win + Shift + S` でスクリーンショットを取得
4. `docs/evidence/task011_topic_assets.png` として保存

### 2. UnlockTopicCommandの動作確認

#### シーンの準備

1. Unity Editor で `Assets/Scenes/DebugChatScene.unity` を開く
2. `ScenarioManager` コンポーネントが `DebugScript.yarn` を参照していることを確認
   - `ScenarioManager` の `DialogueRunner` が `DebugScript.yarn` を読み込むように設定されている必要がある

#### 実行と確認

1. Playボタンを押してシーンを実行
2. シナリオが進行し、`<<UnlockTopic "debug_topic_01">>` コマンドが実行されるタイミングで以下を確認:
   - Consoleウィンドウに以下のログが表示される:
     ```
     ScenarioManager: Topic unlocked - Strange Signal (ID: debug_topic_01)
     ```
   - エラーが発生しない
   - トピックが正常に読み込まれる（Resources.Loadが成功する）

#### ログ確認

Consoleウィンドウで以下のログを確認:
- ✅ `ScenarioManager: Topic unlocked - Strange Signal (ID: debug_topic_01)` が表示される
- ❌ エラーログが表示されない
- ❌ `ScenarioManager: Failed to load TopicData from Resources/Topics/debug_topic_01` が表示されない

## 期待される動作

### スクリーンショット取得
- `docs/evidence/task011_topic_assets.png` が作成される
- スクリーンショットには `debug_topic_01.asset` のInspector表示が含まれる
- 以下の情報が表示される:
  - **Topic ID**: `debug_topic_01`
  - **Title**: `Strange Signal`
  - **Description**: `拾ったスマホから受信した不審な信号。ノイズが多く、内容は不明瞭だが、何か重要な情報が隠されている気がする。`

### UnlockTopicCommand
- `ScenarioManager.UnlockTopicCommand` が正常に動作する
- トピックが `Resources.Load<TopicData>($"Topics/debug_topic_01")` で正常に読み込まれる
- Consoleウィンドウに成功ログが表示される
- Yarn変数 `$has_topic_debug_topic_01` が `true` に設定される
- エラーが発生しない

## トラブルシューティング

### スクリーンショットが取得できない場合

1. **メニューが表示されない場合**
   - Unity Editor を再起動
   - コンパイルエラーがないことを確認

2. **アセットが選択されない場合**
   - `Assets/Resources/Topics/debug_topic_01.asset` が存在することを確認
   - アセットが正しく読み込まれていることを確認

3. **スクリーンショットが保存されない場合**
   - `docs/evidence/` ディレクトリが存在することを確認
   - ファイルの書き込み権限があることを確認
   - Unity 2022.1以降を使用していることを確認（自動スクリーンショット取得の場合）

### UnlockTopicCommandが動作しない場合

1. **コマンドが実行されない場合**
   - `DebugScript.yarn` が正しく読み込まれていることを確認
   - `ScenarioManager` の `DialogueRunner` が `DebugScript.yarn` を参照していることを確認
   - Yarnスクリプトの構文エラーがないことを確認

2. **トピックが読み込まれない場合**
   - `Assets/Resources/Topics/debug_topic_01.asset` が存在することを確認
   - アセットのパスが正しいことを確認
   - Consoleウィンドウでエラーログを確認

3. **ログが表示されない場合**
   - Consoleウィンドウが開いていることを確認
   - ログレベルが適切に設定されていることを確認
   - `ScenarioManager` が正しく初期化されていることを確認

## 技術的詳細

### エディタスクリプトの設計

#### 名前空間
- `ProjectFoundPhone.Debug.Editor` 名前空間を使用
- Unity Editor 専用の機能のため、`#if UNITY_EDITOR` ディレクティブは不要（`Editor/` フォルダ内のため自動的にEditor専用）

#### スクリーンショット取得の実装
- Unity 2022.1以降: `ScreenCapture.CaptureScreenshot()` を使用
- Unity 2022.1未満: 手動取得を案内
- プロジェクトルートからの相対パスで保存

#### アセット選択の実装
- `Selection.activeObject` でアセットを選択
- `EditorUtility.FocusProjectWindow()` でプロジェクトウィンドウにフォーカス
- `EditorGUIUtility.PingObject()` でアセットをハイライト

### 制限事項

1. **Inspectorウィンドウの直接取得**
   - Unity Editor APIではInspectorウィンドウの内容を直接取得できない
   - スクリーン全体のスクリーンショットを取得する方法を使用
   - ユーザーはInspectorウィンドウを表示してから実行する必要がある

2. **Unity Editor実行が必要**
   - スクリーンショット取得はUnity Editor内での実行が必要
   - 自動化（CI/CD等）には対応していない

## 次のステップ

1. **Unity Editor での実行**
   - Unity Editor を起動
   - `Tools/FoundPhone/Select Topic Asset for Screenshot` を実行
   - Inspectorウィンドウでトピック情報を確認
   - `Tools/FoundPhone/Capture Topic Asset Screenshot` を実行してスクリーンショットを取得

2. **UnlockTopicCommandの動作確認**
   - `Assets/Scenes/DebugChatScene.unity` を開く
   - `ScenarioManager` が `DebugScript.yarn` を参照していることを確認
   - Playボタンで実行し、`<<UnlockTopic "debug_topic_01">>` コマンドが正常に動作することを確認
   - Consoleウィンドウに成功ログが表示されることを確認

3. **TASK_011のStatus更新**
   - Evidence取得とUnlockTopicCommand確認完了後、TASK_011のStatusをDONEに更新

4. **タスクファイルの更新**
   - TASK_013のStatusをDONEに更新
   - Report欄にレポートパスを追記

## 実装完了チェックリスト

- [x] `docs/evidence/` ディレクトリの確認・作成
- [x] Inspector表示スクリーンショット取得用エディタスクリプトの作成
- [x] `TopicAssetScreenshotTool.cs` の実装完了
- [x] DebugScript.yarn の確認（`<<UnlockTopic "debug_topic_01">>` が含まれている）
- [x] ScenarioManager.cs の UnlockTopicCommand 実装確認
- [ ] トピックアセットのInspector表示スクリーンショットを取得（Unity Editor実行待ち）
- [ ] UnlockTopicCommandの動作確認（Unity Editor実行待ち）
- [ ] TASK_011のStatusをDONEに更新（Evidence取得とUnlockTopicCommand確認完了後）
- [x] `docs/inbox/` にレポート (`REPORT_TASK_013_TopicDataVerification.md`) が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている（完了後に更新）

## まとめ

TASK_013の実装を完了しました。Inspector表示のスクリーンショット取得用エディタスクリプトを作成し、UnlockTopicCommandの動作確認準備を完了しました。

エディタスクリプトは `Tools/FoundPhone/Capture Topic Asset Screenshot` メニューから実行でき、`docs/evidence/task011_topic_assets.png` としてスクリーンショットを保存します。

UnlockTopicCommandのコードレベルでの確認は完了しており、DebugScript.yarnとScenarioManager.csの実装が正常であることを確認しました。Unity Editor内での実際の動作確認をお願いします。
