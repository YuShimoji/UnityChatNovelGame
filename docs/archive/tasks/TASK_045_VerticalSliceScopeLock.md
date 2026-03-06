# Task: Vertical Slice Scope Lock

Status: DONE
Tier: 1 (Planning)
Branch: main
Owner: Worker
Created: 2026-02-11
Updated: 2026-02-11
Completed: 2026-02-11
Report: docs/reports/REPORT_TASK_045_VerticalSliceScopeLock.md

## Objective

Vertical Slice の実装対象と除外対象を固定し、以後の実装判断を一意にする。

## Milestone

- SG-1: Phase 0-2 Ticket Bootstrap
- MG-1: Vertical Slice Completion

## Focus Area

- docs/GAME_DESIGN_DOCUMENT.md
- docs/PROJECT_ROADMAP.md
- AI_CONTEXT.md

## Forbidden Area

- Assets/ 以下の実装コード改修
- 既存機能の仕様拡張（このタスクでは行わない）

## Constraints

- SSOT は `docs/GAME_DESIGN_DOCUMENT.md` を優先する
- 物語の自動生成前提を導入しない
- 探索スレッドの候補リソースは TBD のまま維持する（バッテリーは未導入）

## DoD

- [ ] Vertical Slice の in-scope / out-of-scope が文書化されている
- [ ] 実装順序（Phase 1-3）の入口条件が明文化されている
- [ ] Worker が迷わないレベルの判断基準が記載されている
- [ ] 変更点がレポートに要約されている

## Test Plan

- テスト対象:
  - ドキュメント整合性（SSOTと矛盾しないこと）
- テスト種別:
  - ドキュメントレビュー
- 期待結果:
  - `docs/GAME_DESIGN_DOCUMENT.md` / `docs/PROJECT_ROADMAP.md` / `AI_CONTEXT.md` の方針が一致している
- テスト不要項目の理由:
  - コード実装変更を含まないため、EditMode/PlayMode/ビルドテストは対象外

## Stop Conditions

- SSOT 間で矛盾が解消できない
- 追加仕様の判断にユーザー決定が必須
