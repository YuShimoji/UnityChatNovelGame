# TASK_027 Minimum Manual Evidence Guide

このディレクトリは、`TASK_027 Full Playthrough Test` の最小証跡を保存する場所です。

## Goal

- `DebugChatScene` の通し導線を 1 回で確認する
- 失敗しても、どこで止まったかを日付付きで残す

## Minimum Runbook

1. `Assets/Scenes/DebugChatScene.unity` を開く
2. Unity Console をクリアする
3. PlayMode 開始
4. `Chat -> Topic unlock -> Deduction -> Synthesis -> Effect/End` を 1 回通す
5. PlayMode 終了後、このフォルダに結果を保存する

## Required Outputs

- `FULL_PLAYTHROUGH_RESULTS_YYYYMMDD.md`
  - 開始時刻
  - 終了時刻
  - 到達した最終ステップ
  - 成功/失敗
  - Console Error/Exception の有無
- `Log_YYYYMMDD.txt`
  - Unity Console のコピー、または関連ログの貼り付け
- `Capture_01_start.png`
  - PlayMode 開始直後
- `Capture_02_topic.png`
  - Topic unlock または Deduction 表示時
- `Capture_03_synthesis_or_end.png`
  - Synthesis 成功時、または End 到達時

## Failure Handling

- 途中で止まった場合も中断しない
- 最後に成功したステップを `FULL_PLAYTHROUGH_RESULTS_YYYYMMDD.md` に書く
- 失敗画面を追加で `Capture_blocker.png` として保存する

## Notes

- 既知の技術 blocker は解消済みです。今回の目的は「再現確認」ではなく「最終通し証跡の確定」です。
- `TASK_053` を閉じるため、この証跡は日付付きファイル名を維持してください。
