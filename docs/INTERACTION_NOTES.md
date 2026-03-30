# Interaction Notes

報告UI・手動確認・質問形式に関する project-local メモ。

## 手動確認の出し方

- 手動確認項目は本文で提示する
- AskUserQuestion では OK / NG番号 だけを聞く
- 手動確認依頼と次アクション選択を同じ質問に混ぜない

## 禁止パターン

- AskUserQuestion の question に Markdown テーブルを入れる
- 選択肢を commit / しない の yes/no で埋める
- 既知文脈を「詳細を教えてください」で再質問する
- 「フォントサイズを変えたので確認してください」のような微修正確認ループ
- 1件の UI 問題ごとに手動確認を求める (一括処理すること)

## ユーザーが嫌う形式

- 進路選択を狭める二択
- 作業量に見合わない微修正の繰り返し提案
- 全ての作業が先送りになる保守偏重の選択肢
- 「前回の反動」で振り子的に方向を決める提案 (UI修正が続いた→次はコンテンツ、ではなく本当に必要なものを特定する)
- AI がユーザーの仕事 (執筆) を代行しようとする提案。AI はシステム/ツールを整備する側

## 報告メモ

- セッション成果を報告する際、「コンテンツまたは機能として何が前進したか」を先に示す
- UI 値調整は Inspector 作業として扱い、セッション成果に含めない
- handoff summary は Shared Focus / Non-Negotiables / Reused Canonical Context / New Fossils / Active Artifact / 現在地 / Expansion Risk / Next を明示する
- handoff 前に `git status` と docs 反映確認を出す

## 運用ルール

- ユーザーの報告形式の好みや嫌いなパターンが判明したらここに追加する
