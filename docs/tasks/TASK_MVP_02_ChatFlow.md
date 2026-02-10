# TASK_MVP_02_ChatFlow

## Objective
Play開始後、会話が先頭から選択肢表示直前まで途切れず進行する最小チャットフローを成立させる。

## Scope
- 触る範囲
  - 会話行表示の最小実装（ハードコード可）
  - 入力受付と進行制御（次へ進む、入力ロック）
  - 連打時の進行破綻防止
- 触らない範囲
  - 外部シナリオデータ化（Yarn/JSON/TextAsset）
  - タイピング演出、画像演出の拡張
  - メッセージ履歴/バックログ

## Non-Goals
- 会話内容の本番品質化
- ローカライズ対応
- メッセージUIデザイン調整

## Implementation Plan
1. ✅ MVPGameController 内に5行のハードコード会話データを定義。
2. ✅ RunChatSequence コルーチンで順次表示＋タイピング遅延を実装。
3. ✅ 進行中は m_IsTransitioning=true で操作をロック。

## 実装対象ファイル（実績）
- `Assets/Scripts/MVP/MVPGameController.cs`（RunChatSequence / AddBubble）

## DoD
- Play開始後、会話が最低3-5行表示される。
- 進行中の連打で行スキップ暴走や停止不全が発生しない。
- Choice表示前まで必ず到達する。
- Console Error/Exception 0を維持する。

## Manual Test Steps
1. MVPScene を Play Mode で開始し、「はじめる」をクリック。
2. メッセージが5行、各行の間にタイピング遅延をもって順次表示されることを確認。
3. チャット表示中に画面クリックしても二重表示や進行停止が起きないことを確認。
4. 5行目の後、Choice画面（「信じる」/「無視する」）に遷移することを確認。
5. ConsoleでError/Exceptionが0件であることを確認。

## Output Format
- `Summary`: 実装の要点
- `Changed Files`: 変更ファイル
- `Flow Verification`: Title->Play->Chatの確認結果
- `Manual Test Result`: 手順別Pass/Fail
- `Open Issues`: 次タスクへ持ち越す軽微課題
