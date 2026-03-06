# Worker Prompt: TASK_053_MVPFinalVerificationPack

## 概要
MVP最終検証タスク群（TASK_MVP_04 / TASK_027 / TASK_025）を実測で埋め、短期ゲートを閉じる。

## 現状
- `docs/tasks/TASK_053_MVPFinalVerificationPack.md` は `OPEN`。
- **依存関係**: TASK_049 / TASK_050 / TASK_051 が完了していること。

## 参照
- チケット: `docs/tasks/TASK_053_MVPFinalVerificationPack.md`
- 連動チケット:
  - `docs/tasks/TASK_MVP_04_VerifyVerticalSlice.md`
  - `docs/tasks/TASK_027_FullPlaythroughTest.md`
  - `docs/tasks/TASK_025_GCAllocReduction.md`
- SSOT: `docs/GAME_DESIGN_DOCUMENT.md`
- 運用: `docs/Windsurf_AI_Collab_Rules_latest.md`, `docs/HANDOVER.md`, `.cursor/MISSION_LOG.md`

## 境界
- Focus Area:
  - `docs/tasks/TASK_MVP_04_VerifyVerticalSlice.md`
  - `docs/tasks/TASK_027_FullPlaythroughTest.md`
  - `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md`
  - `docs/tasks/TASK_025_GCAllocReduction.md`
  - `docs/reports/REPORT_TASK_025_GCAllocReduction.md`
  - `docs/AI_CONTEXT_MVP.md`
- Forbidden Area:
  - 長期機能（Addressables/CloudSave等）
  - 大規模リファクタ

## Test Plan
- テスト対象:
  - MVPScene 通し導線
  - Full playthrough
  - PerformanceMonitor After計測
- テスト種別:
  - PlayMode（手動/自動）
  - 計測検証
- 期待結果:
  - 60秒以内完走
  - Console Error/Exception 0
  - GC Alloc Before/After 比較成立

## Impact Radar
- コード: 原則ドキュメント/証跡更新中心
- テスト: SG-1/MG-1ゲート達成に直結
- パフォーマンス: TASK_025の実測確定
- UX: MVP完走品質の最終確認
- 連携: TASK_047完了後の最終検証ハブ

## Milestone
- `SG-1: MVP縦切りの最終確認`
- `MG-1: MVP安定化と最低限の品質基盤`

## DoD
- [ ] TASK_MVP_04 実測更新
- [ ] TASK_027 Pending解消
- [ ] TASK_025 After計測完了
- [ ] AI_CONTEXT_MVP チェック更新
- [ ] 統合レポート作成

## 停止条件
- 依存タスク未完了
- 計測環境の再現不能
- 仕様不一致が3件以上

## 納品先
- `docs/inbox/REPORT_TASK_053_MVPFinalVerificationPack.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_053_MVPFinalVerificationPack.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_053_MVPFinalVerificationPack.md`
- `docs/reports/REPORT_TASK_053_MVPFinalVerificationPack.md`
- `docs/tasks/TASK_MVP_04_VerifyVerticalSlice.md`
- `docs/tasks/TASK_027_FullPlaythroughTest.md`
- `docs/tasks/TASK_025_GCAllocReduction.md`
- `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md`
- `docs/reports/REPORT_TASK_025_GCAllocReduction.md`
- `docs/AI_CONTEXT_MVP.md`
- `.cursor/MISSION_LOG.md`
