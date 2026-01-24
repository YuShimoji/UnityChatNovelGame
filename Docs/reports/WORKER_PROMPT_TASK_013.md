# Worker Prompt: TASK_011 + TASK_013 Topic System Verification

## Request
あなたは Unity クライアントエンジニアとして、**Topic System (ScriptableObjects, UnlockTopicCommand)** の動作検証を行ってください。
アセットは作成済み(TASK_011)ですが、Unity Editor 上での正しい設定確認と動作ログ確認(TASK_013)が未完了です。

## Context
- **Current State**: `TopicData` クラスとアセット(Resources/Topics/)は存在。
- **Goal**: アセットが Inspector で正しく表示されるか、`UnlockTopicCommand` でエラーが出ないか確認する。

## Focus Area
1. **Assets**: `Assets/Resources/Topics/*.asset`
2. **Command**: `UnlockTopicCommand.cs` (動作確認のみ)
3. **Action**: 
   - Inspector での目視確認
   - DebugChatScene での `<<UnlockTopic>>` コマンド実行確認

## Tasks (Step-by-Step)
1. **Topic Asset Inspection**:
   - `Assets/Resources/Topics/` 以下の適当なアセット (例: `debug_topic_01.asset`) を選択。
   - Inspector にプロパティ (ID, Title, Description, Icon) が表示されているか確認。
   - **Evidence 取得 (Automated)**:
     - `DebugChatScene` に `VerificationCapture` コンポーネントがあることを確認。
     - PlayMode で実行し、`Docs/evidence/` に画像が生成されることを確認。
     - (Optional) `Topic unlocked` ログが確認できるタイミングで `VerificationCapture.TriggerCapture()` を呼ぶように調整してもよいが、基本は起動時キャプチャでシーンが動作している証明があればよしとする（ログはテキスト保存機能があればそちらを確認）。
     - 生成された画像（およびログファイル）を納品物とする。

2. **Command Execution**:
   - `Assets/Scenes/DebugChatScene.unity` を Play する (TASK_007 と同じシーンで可)。
   - `DebugScript.yarn` 内で `<<UnlockTopic "debug_topic_01">>` が呼ばれた時、Console に `Topic unlocked: debug_topic_01` (または類似の成功ログ) が出るか確認。
   - エラーが出ていないことを確認。
   - **Evidence 取得 (Automated)**:
     - コンソールログは `VerificationCapture` の `CaptureLogs` 機能により `docs/evidence/Log_*.txt` に保存される。
     - このログファイルに `Topic unlocked` の記述があることを確認する。
     - 生成されたログファイルを証拠として採用する。

3. **Report**:
   - `docs/tasks/TASK_011_TopicScriptableObjects.md` の DoD 更新。
   - `docs/tasks/TASK_013_TopicDataVerification.md` の DoD 更新。
   - レポート `docs/inbox/REPORT_TASK_013_TopicDataVerification.md` を作成 (TASK_011 の完了報告も兼ねる)。

## Forbidden Area
- `TopicData.cs` の構造変更
- 新規アセットの大量作成

## Output
- `docs/evidence/task011_topic_inspector.png`
- `docs/evidence/task013_unlock_log.png`
- `docs/inbox/REPORT_TASK_013_TopicDataVerification.md`
