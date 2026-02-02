# Task: Project Structure Cleanup

Status: DONE
Tier: 3 (Maintenance)
Branch: feature/structure-cleanup
Owner: Worker
Created: 2026-02-02T04:00:00+09:00
Report: N/A

## Objective
プロジェクト内の `Docs/` (大文字) と `docs/` (小文字) ディレクトリの混在を解消し、一貫した構造に統合する。

## Context
- 現状: `Docs/tasks/`, `Docs/reports/`, `Docs/evidence/` と `docs/inbox/`, `docs/HANDOVER.md` が混在。
- 問題: ファイル参照の不整合、Git大文字小文字問題のリスク。
- Backlog項目として長期懸案だった。

## Scope
- `docs/` (小文字) に統一する方針を推奨。
- 既存の参照（MISSION_LOG, AI_CONTEXT, タスクファイル内のパス）を更新。

## Constraints
- Git履歴を可能な限り保持（`git mv` 使用）。
- 既存のレポート・エビデンスファイルの内容は変更しない。

## Steps
1. 現在のディレクトリ構造を調査し、移行計画を作成。
2. `git mv` で `Docs/` 配下を `docs/` に移動。
3. 全ファイル内の参照パスを更新。
4. コンパイル・リンク切れがないことを確認。

## DoD (Definition of Done)
- [ ] プロジェクト内に `Docs/` ディレクトリが存在しない。
- [ ] 全てのパス参照が `docs/` に統一されている。
- [ ] Git履歴が保持されている（または明示的に履歴断絶を受け入れている）。
