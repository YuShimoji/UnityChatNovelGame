# Task: Structure Cleanup (Docs)

Status: OPEN
Tier: 3
Branch: chore/docs-cleanup
Owner: Worker
Created: 2026-01-30T11:15:00+09:00
Report: docs/reports/REPORT_TASK_044_structure_cleanup.md

## Objective
リポジトリ冁E��散在する `docs/` (Capitalized) と `docs/` (Lowercase) めE`docs/` に統合する、E
Windows 環墁E��の大斁E��小文字�E区別による混乱を防ぐ、E

## Context
- 現状: `docs/` と `docs/` が混在してぁE���E�Eit上�E別チE��レクトリとして扱われる場合がある�E�、E
- 目樁E 全て `docs/` に統一する、E

## Focus Area
- `docs/` フォルダ
- `docs/` フォルダ
- `README.md` (リンク修正)

## Forbidden Area
- ドキュメント�E中身自体�E大幁E��書き換え（移動とリンク修正のみ�E�E

## Steps
1. `git mv docs/* docs/` を実行する、E
2. `Docs` フォルダを削除する、E
3. `README.md` めE���EMarkdownファイル冁E�E `docs/` へのリンクめE`docs/` に置換する、E
4. ローカルで `docs/` に全てのチE�Eタがあることを確認する、E

## DoD (Definition of Done)
- [ ] `docs/` チE��レクトリが存在しなぁE
- [ ] `docs/` チE��レクトリに全てのドキュメントが含まれてぁE��
- [ ] `Usage: git ls-files Docs` が空である
- [ ] Report 作�E (`docs/reports/REPORT_TASK_044_structure_cleanup.md`)
