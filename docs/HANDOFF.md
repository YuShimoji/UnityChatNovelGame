# Handoff

会話ログなしで再開するための最短入口。

## まず読む

1. `docs/HANDOFF.md`
2. `docs/project-context.md`
3. `docs/runtime-state.md`
4. `docs/INVARIANTS.md`
5. `docs/USER_REQUEST_LEDGER.md`

## Current Focus

- 主目的: Ch1 コンテンツ前進 + 制作パイプライン実走
- 次: SP-022 でサブクエスト設計を確定し、Ch1 に 1〜3 本のパイロット追加
- その次: 通し手動検証 → ギャップ P0/P1/P2 付け → Ch2 執筆

## Recent Doc Delta

- docs 整理を継続実施し、`docs/wiki` の重複ページを段階的に統廃合
- `docs/wiki` はポータル最小構成（`README.md` / `_sidebar.md` / `save-system.md`）へ縮約
- `docs/EVIDENCE_REUSE.md` を新設し、archive 側の Evidence Reuse 文書を統合削除
- `docs/ENGINE_FEATURE_INVENTORY.md` / `docs/SCENARIO_AUTHORING_GUIDE.md` / `docs/PROJECT_OVERVIEW.md` / `docs/HANDOFF.md` を索引・要点中心の薄型へ更新
- `docs/verification/2026-03-30-playmode-batchmode-attempt.md` に旧ローカルパス注記を追加（環境差分）

## Safe Next Steps

1. Ch1 Yarn を前進
2. Content Pipeline で `Sync Authoring Assets`
3. ContentAuthoring で StartNode を確認して再生
4. 好機に PlayMode 8件の実ラン記録を `docs/verification/` に追加
5. UI 気づきは `docs/UI_ISSUES.md`、仕様ギャップは SP-022 に記録
6. docs 拡張時は `docs/ai/READ_ORDER.md` の正典導線を崩さず、重複ページを増やさない

## Source Of Truth

- 方針・スライス: `docs/project-context.md`
- 作業状態: `docs/runtime-state.md`
- 決定履歴: `docs/DECISION_LOG.md`
- 制作フロー: `docs/OPERATOR_WORKFLOW.md`
