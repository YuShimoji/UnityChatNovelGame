# Worker Prompt: TASK_013_TopicDataVerification

## 参照
- チケット: docs/tasks/TASK_013_TopicDataVerification.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- AI_CONTEXT: AI_CONTEXT.md
- MISSION_LOG: .cursor/MISSION_LOG.md
- プロジェクト仕様: 最初のプロンプト（プロジェクトルート）

## 境界

### Focus Area
- `docs/evidence/` 配下: スクリーンショットの保存
- `Assets/Scripts/Debug/Editor/` 配下: スクリーンショット自動取得エディタスクリプトの作成（新規）
- `Assets/Scenes/DebugChatScene.unity`: UnlockTopicCommandの動作確認
- `Assets/Resources/Yarn/DebugScript.yarn`: トピック解放コマンドの実行確認
- Unity Editor内での動作確認とEvidence取得

### Forbidden Area
- TopicDataアセットの変更（既存のアセットを維持）
- UnlockTopicCommandのロジック変更（動作確認のみ）
- 新機能の追加（検証のみ、ただしスクリーンショット自動取得エディタスクリプトは許可）

## Tier / Branch
- Tier: 3（検証・修正）
- Branch: feat/topic-verification

## DoD
- [ ] スクリーンショット自動取得エディタスクリプトを作成
  - [ ] `Assets/Scripts/Debug/Editor/TopicAssetScreenshotCapture.cs` を作成
  - [ ] Unity Editorメニューから実行可能（`Tools/FoundPhone/Capture Topic Asset Screenshot`）
  - [ ] `Assets/Resources/Topics/debug_topic_01.asset` を選択してInspector表示
  - [ ] スクリーンショットを `docs/evidence/task011_topic_assets.png` として保存
  - [ ] 保存成功時にコンソールログを出力
- [ ] トピックアセットのInspector表示スクリーンショットを取得
  - [ ] エディタスクリプトを実行してスクリーンショットを取得
  - [ ] `docs/evidence/task011_topic_assets.png` が存在することを確認
- [ ] UnlockTopicCommandの動作確認
  - [ ] `Assets/Scenes/DebugChatScene.unity` を開く
  - [ ] `ScenarioManager` が `DebugScript.yarn` を実行するように設定
  - [ ] Playボタンで実行し、`<<UnlockTopic "debug_topic_01">>` コマンドが正常に動作することを確認
  - [ ] Consoleウィンドウに「Topic unlocked: debug_topic_01」のログが表示されることを確認
  - [ ] エラーが発生しないことを確認
- [ ] TASK_011のStatusをDONEに更新（Evidence取得とUnlockTopicCommand確認完了後）
- [ ] `docs/inbox/` にレポート (`REPORT_TASK_013_TopicDataVerification.md`) が作成されている
- [ ] 本チケットの Report 欄にレポートパスが追記されている

## 停止条件
- Unity Editor が起動できない
- 既存のコンパイルエラーが再発し、解消不能
- 仕様の仮定が 3 つ以上必要
- 依存追加/更新、破壊的Git操作、GitHubAutoApprove不明での push が必要
- SSOT不足を `ensure-ssot.js` で解決できない

停止時は以下を実施：
1. チケットのStatusをBLOCKEDに更新
2. 事実/根拠/次手（候補）をチケット本文に追記
3. docs/inbox/REPORT_TASK_013_TopicDataVerification.md を作成し、停止理由を記録
4. チケットのReport欄にレポートパスを追記

## 納品先
- docs/inbox/REPORT_TASK_013_TopicDataVerification.md

## 実装ヒント

### スクリーンショット自動取得エディタスクリプト
- Unity Editorの `Selection.activeObject` を使用して選択中のアセットを取得
- `EditorUtility.FocusProjectWindow()` でProjectウィンドウにフォーカス
- `EditorGUIUtility.PingObject()` でアセットをハイライト
- `ScreenCapture.CaptureScreenshot()` または `MCPForUnity.Runtime.Helpers.ScreenshotUtility` を使用してスクリーンショットを取得
- `System.IO.File` を使用して `docs/evidence/task011_topic_assets.png` に保存
- `AssetDatabase.Refresh()` でアセットデータベースを更新

### UnlockTopicCommand確認
- DebugChatSceneを開き、GameManagerのScenarioManagerコンポーネントを確認
- DialogueRunnerがDebugScript.yarnを参照していることを確認
- Playボタンで実行し、Consoleログを確認

## 注意事項
- スクリーンショット取得は、Unity Editor内で実行する必要があります
- Inspectorウィンドウが表示されている必要があります
- スクリーンショットは、Unity Editorのウィンドウ全体をキャプチャする可能性があります
