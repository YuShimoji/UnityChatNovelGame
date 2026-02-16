# Worker Prompt: TASK_051_DebugChatUIWiring

## 概要
DebugChatScene の ChatController 参照不足（Choice/Image）を解消し、選択肢/画像導線の実行安定性を確保する。

## 現状
- `docs/tasks/TASK_051_DebugChatUIWiring.md` は `OPEN`。
- ChatController に Choice/Image 参照フィールドはあるが、シーン設定の不足が疑われる。

## 参照
- チケット: `docs/tasks/TASK_051_DebugChatUIWiring.md`
- SSOT: `docs/GAME_DESIGN_DOCUMENT.md`
- 方針: `docs/PROJECT_ROADMAP.md`, `docs/MILESTONE_PLAN.md`, `AI_CONTEXT.md`
- 運用: `docs/Windsurf_AI_Collab_Rules_latest.md`, `docs/HANDOVER.md`, `.cursor/MISSION_LOG.md`

## 境界
- Focus Area:
  - `Assets/Scenes/DebugChatScene.unity`
  - `Assets/Prefabs/UI/`
  - `Assets/Scripts/UI/ChatController.cs`
- Forbidden Area:
  - コアシナリオ仕様の拡張
  - SaveSystem仕様の変更

## Test Plan
- テスト対象:
  - ShowChoices / AddImageMessage
  - DebugChatScene の参照解決
- テスト種別:
  - PlayMode（シーン実行）
  - 手動確認（Unity Editor）
- 期待結果:
  - Choice表示・選択が成功
  - 画像メッセージ表示成功
  - Console Error/Exception 0

## Impact Radar
- コード: Chat UI統合点
- テスト: Smoke/MVPテスト成功率に直結
- パフォーマンス: 影響軽微
- UX: 分岐導線の安定化
- 連携: ScenarioManager, Yarnコマンドとの実行連携

## Milestone
- `SG-1: MVP縦切りの最終確認`
- `MG-1: MVP安定化と最低限の品質基盤`

## DoD
- [ ] Choice関連エラー解消
- [ ] 画像メッセージ導線確認
- [ ] DebugChatScene 参照設定完了
- [ ] レポート作成

## 停止条件
- 大量の新規UI資産作成が必須
- シーン分割を伴う変更が必須

## 納品先
- `docs/inbox/REPORT_TASK_051_DebugChatUIWiring.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_051_DebugChatUIWiring.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_051_DebugChatUIWiring.md`
- `docs/reports/REPORT_TASK_051_DebugChatUIWiring.md`
- `AI_CONTEXT.md`
- `.cursor/MISSION_LOG.md`
