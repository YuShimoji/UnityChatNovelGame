# Task: Structure Cleanup (Docs)

Status: DONE
Tier: 3
Branch: chore/docs-cleanup
Owner: Worker
Created: 2026-01-30T11:15:00+09:00
Report: docs/reports/REPORT_TASK_044_structure_cleanup.md

## Objective
リポジトリ内に散在する `Docs/` (Capitalized) と `docs/` (Lowercase) を `docs/` に統合する。
Windows 環境での大文字小文字の区別による混乱を防ぐ。

## Context
- 現状: `Docs/` と `docs/` が混在している（Git上は別ディレクトリとして扱われる場合がある）。
- 目標: 全て `docs/` に統一する。

## Focus Area
- `docs/` フォルダ
- `README.md` (リンク修正)

## Forbidden Area
- ドキュメントの中身自体の大幅な書き換え（移動とリンク修正のみ）

## Steps
1. `git mv Docs/* docs/` を実行する。
2. `Docs` フォルダを削除する。
3. `README.md` や他のMarkdownファイル内の `Docs/` へのリンクを `docs/` に置換する。
4. ローカルで `docs/` に全てのデータがあることを確認する。

## DoD (Definition of Done)
- [x] `Docs/` ディレクトリが存在しない
- [x] `docs/` ディレクトリに全てのドキュメントが含まれている
- [x] `Usage: git ls-files Docs` が空である
- [x] Report 作成 (`docs/reports/REPORT_TASK_044_structure_cleanup.md`)
