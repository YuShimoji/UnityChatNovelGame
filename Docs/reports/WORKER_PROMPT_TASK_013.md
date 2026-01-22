# Worker Prompt: TASK_011 + TASK_013 Topic System Verification

## Request
あなたは Unity クライアントエンジニアとして、**Topic System (ScriptableObjects, UnlockTopicCommand)** の動作検証を行ってください。
TASK_011 (Asset作成) と TASK_013 (検証) が「Evidence 待ち」で停止しています。

## Context
- **Current State**: アセット作成済み。コード実装済み。Phase 3 戦略により「Verification First」が決定。
- **Goal**: アセットの Inspector 表示と、コマンド実行ログの**スクリーンショットを提出すること**。
- **Blocking**: これが完了しない限り、Deduction Board (TASK_008/009 -> Renamed to 016/017 for conflict resolution) の実装を再開できない。

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
   - **Evidence 取得**: Inspector のスクリーンショットを撮る (`docs/evidence/task011_topic_inspector.png`)。

2. **Command Execution**:
   - `Assets/Scenes/DebugChatScene.unity` を Play する (TASK_007 と同じシーンで可)。
   - `DebugScript.yarn` 内で `<<UnlockTopic "debug_topic_01">>` が呼ばれた時、Console に `Topic unlocked: debug_topic_01` (または類似の成功ログ) が出るか確認。
   - エラーが出ていないことを確認。
   - **Evidence 取得**: Console ログのスクリーンショット (`docs/evidence/task013_unlock_log.png`)。

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
