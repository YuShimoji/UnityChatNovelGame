# AI Context MVP (Vertical Slice Constitution)

## Purpose
このドキュメントは、Unityプロジェクトを最短でMVP到達させるための最優先ルールを定義する。

## MVP Goal (Non-Negotiable)
- 体験フロー: `Title -> Play(会話進行) -> Choice(2択) -> End` を1プレイ `60秒以内` で完走できること。
- Unity Play Mode で `Console Error/Exception = 0`。
- クリック/タップ連打でも進行が破綻しないこと。
- シーン数は1つでも可（分割や演出強化は後回し）。
- 会話/分岐データはハードコード可（TextAsset/JSON/DSL/Yarn連携は後回し）。

## Priorities
1. プレイ可能性（最初から最後まで通る）
2. 進行の堅牢性（連打耐性、重複入力抑止）
3. 最小変更での実装（既存コードを壊さない）
4. 拡張性はMVP達成後に考慮

## Frozen Scope (Do Not Do Now)
- 大規模リファクタ、抽象化、アーキテクチャ刷新
- 最適化、GC削減、ベンチ、テスト拡充
- Save/Load、バックログ、オート/スキップ
- Yarn Spinner連携の強化、外部データ形式整備
- 新規アセット導入（標準UI以外）や外部依存追加

## Execution Rules
- 判断基準はこの `docs/AI_CONTEXT_MVP.md` を最優先とする。
- 既存 `AI_CONTEXT.md` と完了済み `TASK_0xx` は、MVP直結時のみ参照する。
- 差分は最小化し、縦切り完走に不要な変更は行わない。
- 迷ったら「60秒以内の通し完走」に寄与する方を採用する。

## MVP Acceptance Checklist
- [ ] Title画面でPlay開始操作ができる
- [ ] 会話が最低3-5行進行する
- [ ] 2択のChoiceが表示され、どちらを選んでも進行する
- [ ] End到達の表示/状態遷移が確認できる
- [ ] 連打しても二重遷移・二重選択・停止不全が発生しない
- [ ] Console Error/Exceptionが0件
- [ ] テキストが空欄/□にならず表示される

## 確認メモ（2026-02-12）
- MVPSceneでテキストが空欄にならないことを視認
- Choice表示（2択）が画面上で視認できることを確認
