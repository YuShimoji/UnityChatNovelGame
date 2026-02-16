# Worker Prompt: TASK_052_VerticalSliceSmokeResultClosure

## 概要
TASK_047 の未完了DoD（PlayMode/Build成功証跡）を回収して、スモークゲートを完了状態にする。

## 現状
- `docs/tasks/TASK_052_VerticalSliceSmokeResultClosure.md` は `OPEN`。
- **依存関係**: TASK_049 / TASK_050 / TASK_051 が完了していること。

## 参照
- チケット: `docs/tasks/TASK_052_VerticalSliceSmokeResultClosure.md`
- 連動チケット: `docs/tasks/TASK_047_VerticalSliceSmokeGate.md`
- SSOT: `docs/GAME_DESIGN_DOCUMENT.md`
- 運用: `docs/Windsurf_AI_Collab_Rules_latest.md`, `docs/HANDOVER.md`, `.cursor/MISSION_LOG.md`

## 境界
- Focus Area:
  - `docs/evidence/TASK_047/`
  - `docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md`
  - `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`
- Forbidden Area:
  - 本体機能の新規実装
  - 演出仕様の追加

## Test Plan
- テスト対象:
  - VerticalSliceSmokeGatePlayModeTests
  - Windows build command
- テスト種別:
  - PlayMode（CLI）
  - Build（CLI/Windows）
- 期待結果:
  - `PlayModeResults.xml` が生成される
  - `TinyChatNovel.exe` が生成される

## Impact Radar
- コード: テスト資産と実行ログのみ
- テスト: 回帰ゲートの完了に直結
- パフォーマンス: 影響なし
- UX: 導線品質保証に寄与
- 連携: TASK_047 / TASK_053 の土台

## Milestone
- `SG-1: MVP縦切りの最終確認`
- `MG-1: MVP安定化と最低限の品質基盤`

## DoD
- [ ] PlayModeResults.xml 取得
- [ ] Build成果物取得
- [ ] TASK_047 レポートの未達DoD解消
- [ ] チケット/レポート更新

## 停止条件
- Unity実行環境でCLI検証不可
- 依存タスク未完了

## 納品先
- `docs/inbox/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_052_VerticalSliceSmokeResultClosure.md`
- `docs/reports/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`
- `docs/tasks/TASK_047_VerticalSliceSmokeGate.md`
- `docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md`
- `.cursor/MISSION_LOG.md`
