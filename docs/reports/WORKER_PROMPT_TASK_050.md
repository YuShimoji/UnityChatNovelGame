# Worker Prompt: TASK_050_YarnConflictCleanup

## 概要
Yarn スクリプトのノード重複/変数再宣言を解消し、縦切りシナリオを安定化する。

## 現状
- `docs/tasks/TASK_050_YarnConflictCleanup.md` は `OPEN`。
- TASK_047 証跡で `Start` ノード重複と変数再宣言が記録されている。

## 参照
- チケット: `docs/tasks/TASK_050_YarnConflictCleanup.md`
- SSOT: `docs/GAME_DESIGN_DOCUMENT.md`
- 方針: `docs/PROJECT_ROADMAP.md`, `docs/MILESTONE_PLAN.md`, `AI_CONTEXT.md`
- 運用: `docs/Windsurf_AI_Collab_Rules_latest.md`, `docs/HANDOVER.md`, `.cursor/MISSION_LOG.md`

## 境界
- Focus Area:
  - `Assets/Resources/Yarn/DebugScript.yarn`
  - `Assets/Resources/Yarn/VerticalSlice.yarn`
  - `Assets/Resources/Yarn/Project.yarnproject`
- Forbidden Area:
  - ChatController/ScenarioManager の仕様追加
  - 物語テーマの大幅改稿

## Test Plan
- テスト対象:
  - Yarn importer コンパイル
  - DebugChatScene のシナリオ開始
- テスト種別:
  - EditMode（Yarn import/compile）
  - PlayMode（開始ノード確認）
- 期待結果:
  - Yarn コンパイルエラー 0
  - シナリオ進行開始を確認

## Impact Radar
- コード: Yarnアセット本体
- テスト: Smoke/MVP検証の開始条件に影響
- パフォーマンス: 影響軽微
- UX: シナリオ開始の安定化
- 連携: ScenarioManager/DialogueRunner と直結

## Milestone
- `SG-1: MVP縦切りの最終確認`
- `MG-1: MVP安定化と最低限の品質基盤`

## DoD
- [ ] ノード重複エラー解消
- [ ] 変数再宣言エラー解消
- [ ] 開始ノード一意化
- [ ] レポート作成

## 停止条件
- シナリオ仕様再設計が必要
- 影響範囲が広域化してチケット境界を超える

## 納品先
- `docs/inbox/REPORT_TASK_050_YarnConflictCleanup.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_050_YarnConflictCleanup.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_050_YarnConflictCleanup.md`
- `docs/reports/REPORT_TASK_050_YarnConflictCleanup.md`
- `AI_CONTEXT.md`
- `.cursor/MISSION_LOG.md`
