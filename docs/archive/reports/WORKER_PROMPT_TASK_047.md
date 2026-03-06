# Worker Prompt: TASK_047_VerticalSliceSmokeGate

## 概要
縦切り導線の破綻を早期検知するため、最小構成のスモークゲート（PlayMode + 手動チェック）を整備する。

## 現状
- `docs/tasks/TASK_047_VerticalSliceSmokeGate.md` は `OPEN`。
- 方針は「網羅より先に導線を守る」。過剰な検証拡張は行わない。

## 参照
- チケット: `docs/tasks/TASK_047_VerticalSliceSmokeGate.md`
- SSOT: `docs/GAME_DESIGN_DOCUMENT.md`
- 方針: `docs/CONCEPT.md`, `docs/PROJECT_ROADMAP.md`, `AI_CONTEXT.md`
- 運用: `docs/Windsurf_AI_Collab_Rules_latest.md`, `docs/HANDOVER.md`, `.cursor/MISSION_LOG.md`

## 境界
- Focus Area:
  - `Assets/Scripts/Tests`
  - `docs/evidence`
  - `docs/reports`
- Forbidden Area:
  - 本体機能の仕様拡張
  - パフォーマンス最適化の深掘り
  - 演出仕様の追加決定

## Test Plan
- テスト対象:
  - タイトル -> チャット進行 -> 分岐 -> 待機 -> セーブ/ロード
- テスト種別:
  - PlayMode スモーク（最低1本）
  - 手動チェックリスト
  - ビルド検証（最低1ターゲット）
- 期待結果:
  - 導線破綻を再現可能な形で検出できる
  - 失敗時の証跡（ログ/スクリーンショット）が保存される

## Impact Radar
- コード: テストコード中心。ランタイム仕様への介入を最小化
- テスト: 回帰検知を最短で回せる粒度に固定
- パフォーマンス: 測定自体は対象外、実行コストのみ管理
- UX: 体験品質の崩壊点を早期に見つけるための守りを強化
- 互換性: 既存テスト資産と衝突しない命名/配置を徹底

## Milestone
- `MG-1: Vertical Slice Completion`

## DoD
- [ ] 縦切り導線のスモークチェック項目が定義されている
- [ ] 最低1本のPlayModeスモークが実行可能
- [ ] 手動チェックリストがレポートに整備されている
- [ ] 失敗時の記録方法（ログ/スクリーンショット）が明記されている
- [ ] テスト実行結果（PlayMode/Build）が記録される

## 停止条件
- スモーク設計が機能仕様を越えて過剰に広がる
- 既存テストとの衝突でメンテナンス不能になる
- Forbidden Area への変更が必要になる

## 納品先
- `docs/inbox/REPORT_TASK_047_VerticalSliceSmokeGate.md`
- （アーカイブ先）`docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md`

## 完了時に更新するファイル
- `docs/tasks/TASK_047_VerticalSliceSmokeGate.md`（Status/DoD根拠）
- `AI_CONTEXT.md`（検証運用の更新）
- `.cursor/MISSION_LOG.md`（Phase進行と証跡）
