# Worker Prompt: TASK_046_ChatDialogueView_VerticalSlice

## 概要
`ChatDialogueView` を縦切り導線へ接続し、タイトル開始から「会話 -> 分岐 -> 待機」までを実行可能にする。

## 現状
- `docs/tasks/TASK_045_VerticalSliceScopeLock.md` は `DONE`。
- `docs/tasks/TASK_046_ChatDialogueView_VerticalSlice.md` は `IN_PROGRESS`。
- 縦切り方針は「まず導線を通す」。探索スレッド候補リソースは `TBD` のまま維持。

## 参照
- チケット: `docs/tasks/TASK_046_ChatDialogueView_VerticalSlice.md`
- SSOT: `docs/GAME_DESIGN_DOCUMENT.md`
- 方針: `docs/CONCEPT.md`, `docs/PROJECT_ROADMAP.md`, `AI_CONTEXT.md`
- 運用: `docs/Windsurf_AI_Collab_Rules_latest.md`, `docs/HANDOVER.md`, `.cursor/MISSION_LOG.md`

## 境界
- Focus Area:
  - `Assets/Scripts/Core`
  - `Assets/Scripts/UI`
  - `Assets/Resources/Yarn`
  - `Assets/Scenes`
- Forbidden Area:
  - 探索スレッドの候補リソース仕様確定（TBDを維持）
  - テーマ依存演出の大規模追加
  - Addressables移行
  - 物語自動生成ロジックの導入

## Test Plan
- テスト対象:
  - `ChatDialogueView` 行表示/選択肢表示
  - Yarn進行（行/選択肢/待機）
  - Save/Load 復帰
- テスト種別:
  - EditMode（必要なロジック単体）
  - PlayMode（タイトル -> 会話 -> 分岐 -> 待機 の通し）
  - ビルド検証（エラーなし）
- 期待結果:
  - 主要導線で例外・進行停止が発生しない
  - Save/Load 後も進行が継続する

## Impact Radar
- コード: `ScenarioManager` と `ChatDialogueView` の連携点に変更集中
- テスト: 最小スモークを優先し、回帰検知を残す
- パフォーマンス: 計測最適化は対象外（破綻検知優先）
- UX: チャット進行の可視性と待機制御の一貫性を担保
- 互換性: 既存セーブ互換を維持（破壊変更禁止）

## Milestone
- `MG-1: Vertical Slice Completion`

## DoD
- [ ] `ChatDialogueView` で行表示と選択肢表示が機能する
- [ ] タイトル -> 会話 -> 分岐 -> 待機 の通し導線が成立する
- [ ] StartWait/SkipWait の進行制御が破綻しない
- [ ] セーブ/ロードを挟んでも進行が継続できる
- [ ] テスト結果（EditMode/PlayMode/Build）が記録される
- [ ] レポートが作成される

## 停止条件
- 既存 `ScenarioManager` との整合が取れず設計変更が大きくなる
- Yarn資産側の修正だけでは進行不具合が解消できない
- Forbidden Area への変更が必要になる

## 納品先
- `docs/inbox/REPORT_TASK_046_ChatDialogueView_VerticalSlice.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_046_ChatDialogueView_VerticalSlice.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_046_ChatDialogueView_VerticalSlice.md`（Status/DoD根拠）
- `AI_CONTEXT.md`（直近の実装状況）
- `.cursor/MISSION_LOG.md`（Phase進行と証跡）
