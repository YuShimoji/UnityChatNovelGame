# TASK_MVP_03_ChoiceToEnd

## Objective
2択Choiceから必ずEndまで到達する分岐終端フローを実装し、MVP縦切りを完了する。

## Scope
- 触る範囲
  - 2択Choiceの表示と選択処理
  - 分岐先の会話または状態遷移
  - End表示（テキスト/パネル/状態）
  - 選択連打時の二重確定防止
- 触らない範囲
  - 分岐数の増加
  - エンディング演出強化
  - リプレイ/周回/保存連携

## Non-Goals
- 複数エンド実装
- 実績や統計の記録
- 章管理システムの導入

## Implementation Plan
1. ✅ ChoicePanel に2ボタン（信じる/無視する）を配置。OnChoiceSelected で分岐。
2. ✅ 各分岐後に1-2行の追加会話→End画面へ遷移。
3. ✅ End画面に「はじめからやり直す」ボタンでTitle復帰。
4. ⬜ Play Modeで通し60秒検証（ユーザー実施待ち）。

## 実装対象ファイル（実績）
- `Assets/Scripts/MVP/MVPGameController.cs`（OnChoiceSelected / RunBranchSequence）

## DoD
- Choiceが2択で表示される。
- どちらを選んでもEndに到達する。
- Choice連打しても二重確定・進行破綻が起きない。
- Title->Play->Choice->Endを60秒以内で完走できる。
- Console Error/Exception 0。

## Manual Test Steps
1. MVPScene Play Mode → 「はじめる」 → 会話5行 → Choice画面到達。
2. 「信じる」を押してEnd画面到達を確認。メッセージ:「あなたは見知らぬ相手を信じた...」
3. 「はじめからやり直す」でTitleへ戻り、再度Play → 「無視する」→ End到達確認。
4. Choice画面で連打し、二重遷移やUI固着がないことを確認。
5. ストップウォッチで Title→End の通し時間を計測（目標: 60秒以内）。
6. ConsoleでError/Exceptionが0件であることを確認。

## Output Format
- `Summary`: 実装内容の要約
- `Changed Files`: 変更ファイル一覧
- `Branch Paths`: A/B分岐とEnd到達結果
- `Timing`: 通しプレイ時間
- `Manual Test Result`: 手順ごとのPass/Fail
- `Risk Notes`: MVP後に解消する技術的負債
