# Task: Worktree Normalization

Status: COMPLETED
Tier: 1 (Hygiene)
Branch: main
Owner: Orchestrator
Created: 2026-03-01T05:35:00+09:00
Updated: 2026-03-01T05:37:17+09:00
Report: docs/reports/REPORT_TASK_055_WorktreeNormalization.md

## Objective

generated logs / evidence と durable evidence を整理し、unrelated local modifications を除外した orchestrator-owned commit boundary を作る。

## Milestone

- MG-1: MVP 安定化と最低限の品質基盤

## Focus Area

- `docs/evidence/`
- `docs/logs/`
- `docs/inbox/`
- `docs/reports/`
- git staging boundary

## Forbidden Area

- ユーザー起因の local modifications の破棄
- 実装内容の巻き戻し
- 証跡リンクを壊すだけの削除

## Constraints

- 残す evidence は report / task / handover から辿れるものを優先する
- 削除対象は transient log / superseded report を中心にする
- user-side local modifications は触らずに除外する

## Layer Split

### Layer A (AI-completable)

- referenced artifacts の棚卸し
- transient artifacts の削除
- task / SSOT / handover の同期
- commit boundary の準備

### Layer B (human decision if needed)

- user-side local modifications の採否判断
- push / merge の最終判断

## DoD (Definition of Done)

- [x] referenced artifacts と transient artifacts の区分が task/report に残る
- [x] transient generated artifacts が整理される
- [x] unrelated local modifications を除外した orchestrator-owned commit boundary を作る
- [x] SSOT / handover / mission log が最新状態に同期される

## Test Plan

- テスト対象:
  - `git status --short`
  - `report-validator`
  - `session-end-check`
- テスト種別:
  - repository hygiene verification
- 期待結果:
  - 整理後の差分理由が説明可能になる
  - checker の失敗理由が user-side local modifications または boundary 外の residual file に限定される

## Stop Conditions

- referenced artifacts の削除が必要になる
- user-side local modifications と同一ファイルで安全に切り分けできない
- commit boundary の説明責任が維持できない
