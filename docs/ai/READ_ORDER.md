# エージェント・アシスタント向け読書順（SSOT）

本ファイルは、このリポジトリでアダプタが参照する **読書順の正規リスト**である（他のファイルに別の番号列を増やさない）。`.claude/CLAUDE.md`・`prompt-resume.md`・ルート `CLAUDE.md` はここを指すだけにする。

## 標準順（上から順に）

1. `docs/ai/CORE_RULESET.md` — ベンダー中立の行動原則
2. `docs/ai/DECISION_GATES.md` — 承認・境界のゲート
3. `docs/ai/STATUS_AND_HANDOFF.md` — ステータス語彙・信頼度の扱い
4. `docs/ai/WORKFLOWS_AND_PHASES.md` — フェーズとワークフロー
5. `docs/HANDOFF.md` — 直近の焦点・Safe Next Steps・会話なし引き継ぎ
6. `docs/project-context.md` — レーン・スライス・方針の正典
7. `docs/runtime-state.md` — セッション単位の作業状態
8. `docs/INVARIANTS.md` — 非交渉の不変条件
9. `docs/USER_REQUEST_LEDGER.md` — 継続要求・バックログ差分
10. `docs/OPERATOR_WORKFLOW.md` — 人間オペレーターの制作フロー
11. `docs/INTERACTION_NOTES.md` — 対話スタイル・検証の約束事
12. `docs/FEATURE_REGISTRY.md` — ENH など改善候補レジストリ

## タスク別の追加読み（5. `HANDOFF.md` 内の「実作業に応じて」と同義）

- 制作フロー: `docs/YarnEditingPipeline.md`, `docs/SCENARIO_AUTHORING_GUIDE.md`
- UI/挙動: `docs/DISPLAY_ALGORITHMS.md`, `docs/UI_ISSUES.md`
- 実装/監査: `docs/FEATURE_STATUS_AUDIT.md`, `docs/spec-index.json`
- Ch1 サブクエスト検証を別スレッドに渡す: `docs/ai/TASK_PROMPT_ch1_sidequest_verification.md`
- 並行レーン用プロンプト（プラン直前準備）: `docs/ai/PARALLEL_LANE_PROMPTS.md`
- SP-023/024 表示系デモ計画（修正版）: `docs/plans/display-batch-showcase.md`

## 人間向けの最短経路

会話ログなしで状況だけ掴む場合は、`docs/HANDOFF.md` の「まず読む順番」（HANDOFF → project-context → …）で足りることが多い。アシスタントは上記 **標準順** を優先する。

## メタ

- `docs/archive/` は、明示依頼がない限り読まない（ルート `CLAUDE.md` / `AGENTS.md` のルールと整合）。
