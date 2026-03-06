# Worker Prompt: TASK_055_EvidenceReuseAutomation

## 概要
`TASK_053` 着手前に、証跡再利用ルールを明文化し、証跡回収の反復コストを下げるための運用整備（必要に応じて軽量スクリプト化）を行う。

## 事前必読（新ルール）
- `docs/02_design/ASSEMBLY_ARCHITECTURE.md`
- `docs/03_guides/COMPILATION_GUARD_PROTOCOL.md`
- `.cursor/MISSION_LOG.md`

## 現状
- `docs/tasks/TASK_055_EvidenceReuseAutomation.md` は `OPEN`。
- `TASK_054` は `IN_PROGRESS`（PlayModeスモーク再判定待ち）。
- `TASK_053` は `OPEN` だが、証跡の再利用可否基準が未整備。

## 参照
- チケット: `docs/tasks/TASK_055_EvidenceReuseAutomation.md`
- 連動チケット: `docs/tasks/TASK_053_MVPFinalVerificationPack.md`
- 関連レポート: `docs/reports/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`
- 運用SSOT: `.shared-workflows/docs/windsurf_workflow/EVERY_SESSION.md`
- AI Context: `.shared-workflows/AI_CONTEXT.md`
- 最新状態: `.cursor/MISSION_LOG.md`

## 境界
- Tier / Branch:
  - Tier 2 (Tooling)
  - `feature/task-055-evidence-reuse-automation`
- Target Assemblies:
  - `N/A`（ドキュメント/運用スクリプト中心。asmdef変更は対象外）
- Focus Area:
  - `docs/tasks/TASK_053_MVPFinalVerificationPack.md`
  - `docs/evidence/TASK_047/`
  - `docs/reports/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`
  - `.shared-workflows/scripts/`
  - `docs/03_guides/`
- Forbidden Area:
  - 新規ゲーム機能実装
  - シーン演出/UI仕様の追加変更
  - 根拠なしのタスクDONE化

## Test Plan
- テスト対象:
  - 証跡再利用ルール文書の網羅性
  - manifest生成/更新手順（スクリプトまたは運用手順）
  - `TASK_053` への反映内容
- テスト種別:
  - 手順テスト（ドキュメント検証）
  - スクリプト実行テスト（該当する場合）
  - EditMode（コード変更がある場合のみコンパイル確認）
- 期待結果:
  - 再利用可否を第三者が同一判定できる
  - `TASK_053` で「再取得必要証跡」と「再利用可能証跡」が分離される
  - 自動化手順後にmanifestが生成される

## Impact Radar
- コード:
  - `.shared-workflows/scripts/` を変更する場合、既存運用スクリプトとの互換性
- テスト:
  - 証跡の妥当性判定フローが `TASK_053` の完了判定に直結
- パフォーマンス:
  - 手動証跡回収の反復工数を削減できるか
- UX:
  - Orchestrator/Worker が同一基準で判断でき、再作業が減るか
- 連携:
  - `TASK_054` の結果受領後に `TASK_053` へ滑らかに接続できるか

## Milestone
- `SG-1: MVP縦切りの最終確認`
- `MG-1: MVP安定化と最低限の品質基盤`

## AI_CONTEXT 背景情報
- 表示ポリシーは `data/presentation.json` をSSOTとする。
- リスクとして「Evidence Missing（手動検証負担）」が既知であり、本タスクはその緩和が目的。
- 破壊的操作は避け、運用整備を優先する。

## DoD
- [ ] `docs/03_guides/` 配下に証跡再利用ルールを文書化
- [ ] `docs/evidence/TASK_047/` を入力に再利用判定manifestを出力可能にする
- [ ] `docs/tasks/TASK_053_MVPFinalVerificationPack.md` に再利用条件を反映
- [ ] `docs/reports/REPORT_TASK_055_EvidenceReuseAutomation.md` を作成
- [ ] コード変更が発生した場合のみ Unity Editor でコンパイルエラー0を確認

## 停止条件
- 証跡基準の合意が取れず判定ルールを確定できない
- 既存証跡の欠損で判定前提データが不足
- asmdef境界を跨ぐ大規模改修が必要

## 納品先
- `docs/inbox/REPORT_TASK_055_EvidenceReuseAutomation.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_055_EvidenceReuseAutomation.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_055_EvidenceReuseAutomation.md`
- `docs/reports/REPORT_TASK_055_EvidenceReuseAutomation.md`
- `docs/tasks/TASK_053_MVPFinalVerificationPack.md`
- `docs/03_guides/` 配下の新規または更新ガイド
- `.shared-workflows/scripts/` 配下の追加/更新スクリプト（必要な場合）
- `.cursor/MISSION_LOG.md`
