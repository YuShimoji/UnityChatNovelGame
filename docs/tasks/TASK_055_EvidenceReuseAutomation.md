# Task: Evidence Reuse Rule and Capture Automation for MVP Gates

Status: IN_PROGRESS
Tier: 2 (Tooling)
Branch: feature/task-055-evidence-reuse-automation
Owner: Worker
Created: 2026-02-20
Updated: 2026-02-20
Report: docs/reports/REPORT_TASK_055_EvidenceReuseAutomation.md

## Objective

`TASK_053` 着手前提として、証跡の再利用条件を明文化し、連続ゲート作業での証跡回収コストを下げる運用を整備する。

## Milestone

- SG-1: MVP縦切りの最終確認
- MG-1: MVP安定化と最低限の品質基盤

## Target Assemblies

- `N/A`（ドキュメント/運用スクリプト中心。asmdef変更は対象外）

## Focus Area

- `docs/tasks/TASK_053_MVPFinalVerificationPack.md`
- `docs/evidence/TASK_047/`
- `docs/reports/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`
- `.shared-workflows/scripts/`
- `docs/03_guides/`

## Forbidden Area

- 新規ゲーム機能の実装
- シーン演出やUI仕様の追加変更
- 既存タスクを根拠なしに DONE へ変更

## Constraints

- 証跡再利用の判定は「ファイル存在」だけでなく、テスト名・実行日時・結果一致を必須条件にする
- 再利用可能/不可の判断基準を文書化し、`TASK_053` から参照できる状態にする
- 自動化は既存ワークフロー（`docs/evidence` 配下運用）と衝突しないこと

## DoD

- [ ] 証跡再利用ルールを `docs/03_guides/` 配下に文書化し、再利用可否判定基準を明記する
- [ ] 既存証跡（`docs/evidence/TASK_047/`）を入力に、再利用判定のための manifest（例: hash/updated/test result）を出力できる
- [ ] `TASK_053_MVPFinalVerificationPack.md` に再利用条件と追加取得が必要な証跡の線引きを反映する
- [ ] 適用結果を `docs/reports/REPORT_TASK_055_EvidenceReuseAutomation.md` に記録する
- [ ] 変更がコードを含む場合は Unity Editor でコンパイルエラー 0 を確認する

## Test Plan

- テスト対象:
  - 証跡再利用ルール文書の網羅性
  - manifest 生成/更新手順（スクリプトまたは運用手順）
  - `TASK_053` への反映内容
- テスト種別:
  - 手順テスト（ドキュメント検証）
  - スクリプト実行テスト（該当する場合）
  - EditMode（コード変更がある場合のみコンパイル確認）
- 期待結果:
  - 同一成果物を横展開できる条件が第三者にも判定可能
  - `TASK_053` で「再取得が必要な証跡」と「再利用可能な証跡」が分離される
  - 自動化手順実行後に manifest が生成される
- テスト不要項目:
  - PlayModeシナリオの再実行（本タスクは運用整備が対象）

## Stop Conditions

- 証跡基準の合意が得られず、判定ルールが確定できない
- 既存証跡の欠損で再利用判定の前提データが不足する
- asmdef境界を跨ぐ大規模改修が必要になる
