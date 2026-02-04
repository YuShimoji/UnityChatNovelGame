# Task: TopicData Verification and Evidence Collection

Status: IN_PROGRESS
Tier: 3
Branch: feat/topic-verification
Owner: Worker
Created: 2026-01-17T06:00:00+09:00
Report: docs/reports/REPORT_TASK_013_TopicDataVerification.md
## Objective
TASK_011で作成したTopicDataアセットの動作確認とEvidence取得を行う。
`UnlockTopicCommand` が正常に動作することを確認し、トピックアセットのInspector表示スクリーンショットを取得する。

実装対象：
1. **Evidence取得**: トピックアセットのInspector表示スクリーンショット
2. **UnlockTopicCommand確認**: DebugScript.yarnでトピック解放を確認

## Context
- TASK_011でTopicDataアセットの作成とResources.Loadテストは完了
- 4つのトピックアセットが正常に読み込まれることを確認済み（4 succeeded, 0 failed）
- 残りの作業: Evidence取得とUnlockTopicCommandの動作確認
- プロジェクトには `MCPForUnity.Runtime.Helpers.ScreenshotUtility` が存在し、スクリーンショット取得が可能

## Focus Area
- `docs/evidence/` 配下: スクリーンショットの保存
- `Assets/Scenes/DebugChatScene.unity`: UnlockTopicCommandの動作確認
- `Assets/Resources/Yarn/DebugScript.yarn`: トピック解放コマンドの実行確認
- Unity Editor内での動作確認とEvidence取得

## Forbidden Area
- TopicDataアセットの変更（既存のアセットを維持）
- UnlockTopicCommandのロジック変更（動作確認のみ）
- 新機能の追加（検証のみ）

## Constraints
- スクリーンショット取得: Unity Editor内で手動取得、または `ScreenshotUtility` を使用
- Evidence保存先: `docs/evidence/task011_topic_assets.png`
- UnlockTopicCommand確認: DebugScript.yarnで `<<UnlockTopic "debug_topic_01">>` を実行
- 動作確認: Consoleログで「Topic unlocked: debug_topic_01」が表示されることを確認

## DoD (Definition of Done)
- [/] トピックアセットのInspector表示スクリーンショットを取得 (Pending Manual Action)
  - [ ] `Assets/Resources/Topics/debug_topic_01.asset` を選択
  - [ ] Inspectorウィンドウでトピック情報を表示
  - [ ] スクリーンショットを `docs/evidence/task011_topic_assets.png` として保存
- [/] UnlockTopicCommandの動作確認 (Code Verified / Pending Runtime Check)
  - [ ] `Assets/Scenes/DebugChatScene.unity` を開く
  - [ ] `ScenarioManager` が `DebugScript.yarn` を実行するように設定
  - [ ] Playボタンで実行し、`<<UnlockTopic "debug_topic_01">>` コマンドが正常に動作することを確認
  - [ ] Consoleウィンドウに「Topic unlocked: debug_topic_01」のログが表示されることを確認
  - [ ] エラーが発生しないことを確認 (Static Check Passed)
- [/] TASK_011のStatusをDONEに更新（Evidence取得とUnlockTopicCommand確認完了後）
- [x] `docs/inbox/` にレポート (`REPORT_TASK_013_TopicDataVerification.md`) が作成されている
- [x] 本チケットの Report 欄にレポートパスが追記されている

## スクリーンショット取得方法

### 方法1: Unity Editor内で手動取得（推奨）
1. Unity Editorで `Assets/Resources/Topics/debug_topic_01.asset` を選択
2. Inspectorウィンドウでトピック情報を表示
3. Windows: `Win + Shift + S` でスクリーンショットを取得
4. `docs/evidence/task011_topic_assets.png` として保存

### 方法2: ScreenshotUtilityを使用（Unity Editor内で実行）
プロジェクトには `MCPForUnity.Runtime.Helpers.ScreenshotUtility` が存在しますが、これはUnity Editor内で実行する必要があります。
エディタスクリプトを作成して、Inspector表示のスクリーンショットを自動取得することも可能です。

## UnlockTopicCommand確認手順

1. **シーンの準備**
   - Unity Editorで `Assets/Scenes/DebugChatScene.unity` を開く
   - `ScenarioManager` コンポーネントが `DebugScript.yarn` を参照していることを確認

2. **実行と確認**
   - Playボタンを押してシーンを実行
   - シナリオが進行し、`<<UnlockTopic "debug_topic_01">>` コマンドが実行されるタイミングで以下を確認:
     - Consoleウィンドウに `Topic unlocked: debug_topic_01` のログが表示される
     - エラーが発生しない
     - トピックが正常に読み込まれる（Resources.Loadが成功する）

3. **ログ確認**
   - Consoleウィンドウで以下のログを確認:
     ```
     Topic unlocked: debug_topic_01
     ```
   - エラーログが表示されないことを確認

## Notes
- Status は OPEN / IN_PROGRESS / BLOCKED / DONE を想定
- BLOCKED の場合は、事実/根拠/次手（候補）を本文に追記し、Report に docs/inbox/REPORT_...md を必ず設定
- Evidence取得は手動で行う必要があります（Unity Editor内での操作が必要）
- UnlockTopicCommand確認後、TASK_011のStatusをDONEに更新すること
