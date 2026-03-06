# Worker Prompt: TASK_049_BuildGateFix_VerticalSlice

## 概要
縦切り導線のビルド阻害要因を解消し、PlayMode/Build 検証を再開可能な状態にする。

## 現状
- `docs/tasks/TASK_049_BuildGateFix_VerticalSlice.md` は `OPEN`。
- TASK_047 の Build証跡で Editor コンパイルエラーが確認されている。

## 参照
- チケット: `docs/tasks/TASK_049_BuildGateFix_VerticalSlice.md`
- SSOT: `docs/GAME_DESIGN_DOCUMENT.md`
- 方針: `docs/PROJECT_ROADMAP.md`, `docs/MILESTONE_PLAN.md`, `AI_CONTEXT.md`
- 運用: `docs/Windsurf_AI_Collab_Rules_latest.md`, `docs/HANDOVER.md`, `.cursor/MISSION_LOG.md`

## 境界
- Focus Area:
  - `Assets/Scripts/Editor/`
  - `Assets/Scripts/MVP/Editor/`
  - `Assets/Scripts/Debug/Editor/`
  - `Assets/Scripts/*.asmdef`
- Forbidden Area:
  - 仕様拡張
  - シナリオ本文の改稿
  - パフォーマンス最適化の深掘り

## Test Plan
- テスト対象:
  - Editor asmdef の参照関係
  - EditorOnly スクリプトのコンパイル境界
- テスト種別:
  - EditMode（コンパイル確認）
  - ビルド検証（Windows）
- 期待結果:
  - `error CS0234 / CS0246` が再発しない
  - ビルド成果物が生成される

## Impact Radar
- コード: asmdef と Editorスクリプトの参照境界
- テスト: ビルドゲート再実行に直接影響
- パフォーマンス: 影響なし（構成修正中心）
- UX: ユーザー体験変更なし
- 連携: TASK_052/TASK_053 の前提条件

## Milestone
- `SG-1: MVP縦切りの最終確認`
- `MG-1: MVP安定化と最低限の品質基盤`

## DoD
- [ ] `Build.log` の Editor namespace 系エラーを解消
- [ ] Unity Editor コンパイルエラー 0
- [ ] Windowsビルド成果物生成を確認
- [ ] レポート作成

## 停止条件
- `ProjectSettings/` / `Packages/` の変更が必須
- ランタイムasmdef再設計が必要

## 納品先
- `docs/inbox/REPORT_TASK_049_BuildGateFix_VerticalSlice.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_049_BuildGateFix_VerticalSlice.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_049_BuildGateFix_VerticalSlice.md`
- `docs/reports/REPORT_TASK_049_BuildGateFix_VerticalSlice.md`
- `AI_CONTEXT.md`
- `.cursor/MISSION_LOG.md`
