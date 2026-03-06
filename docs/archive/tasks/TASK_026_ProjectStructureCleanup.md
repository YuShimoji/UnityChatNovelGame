# Task: Project Structure Cleanup

Status: DONE
Tier: 3 (Maintenance)
Branch: feature/structure-cleanup
Owner: Worker
Created: 2026-02-02T04:00:00+09:00
Completed: 2026-02-06T02:50:00+09:00
Report: docs/reports/REPORT_TASK_026_ProjectStructureCleanup.md

## Objective

プロジェクト内の `docs/` (大文字) と `docs/` (小文字) ディレクトリの混在を解消し、一貫した構造に統合する。

## Context

- 現状: `docs/tasks/`, `docs/reports/`, `docs/evidence/` と `docs/inbox/`, `docs/HANDOVER.md` が混在。
- 問題: ファイル参照の不整合、Git大文字・小文字問題のリスク。
- Backlog項目として長年の懸案だった。

## Scope

- `docs/` (小文字) に統一する方針を推奨。
- 既存の参照（MISSION_LOG, AI_CONTEXT, タスクファイル内のパス等）を更新。

## Constraints

- Git履歴を可能な限り保持（`git mv` 使用）。
- 既存のレポートやエビデンスファイルの内容は変更しない。

## Steps

1. 現在のディレクトリ構造を調査し、移行計画を作成。
2. `git mv` で `docs/` 配下を `docs/` に移動。
3. 全ファイル内の参照パスを更新。
4. コンパイル・リンク切れがないことを確認。

## DoD (Definition of Done)

- [ ] プロジェクト内に `docs/` ディレクトリが存在しない。
- [ ] 全てのパス参照が `docs/` に統一されている。
- [ ] Git履歴が保持されている（または明示的な履歴断絶を受け入れている）。
