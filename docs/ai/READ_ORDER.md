# エージェント・アシスタント向け参照索引

本ファイルは正本の所在を探す索引であり、毎回すべてを読む必読リストではない。通常再開の読書予算は `docs/REPO_LOCAL_RULES.md` が所有する。

## 通常再開

1. `docs/REPO_LOCAL_RULES.md` — 日常の実行・停止条件
2. `docs/HANDOFF.md` — 唯一のライブ現在地
3. 今回の active artifact に直接関係する正本

通常は最大 4 文書まで。根拠不足がある場合だけ追加する。

## 判断や運用を変えるとき

| 必要な情報 | 正本 |
|---|---|
| ベンダー中立の原則 | `docs/ai/CORE_RULESET.md` |
| 承認、方向確認、停止条件 | `docs/ai/DECISION_GATES.md` |
| 状態語彙と handoff 責務 | `docs/ai/STATUS_AND_HANDOFF.md` |
| Work Packet と実行フェーズ | `docs/ai/WORKFLOWS_AND_PHASES.md` |
| 監修 AI から渡す Prompt | `docs/ai/PARALLEL_LANE_PROMPTS.md` |
| 長期軸とロードマップ | `docs/project-context.md` |
| 環境・検証条件 | `docs/runtime-state.md` |
| 非交渉条件 | `docs/INVARIANTS.md` |
| 継続要求 | `docs/USER_REQUEST_LEDGER.md` |
| 人間の制作工程 | `docs/OPERATOR_WORKFLOW.md` |
| 対話・レビュー方法 | `docs/INTERACTION_NOTES.md` |
| 機能候補 | `docs/FEATURE_REGISTRY.md` |

## 実作業別

- 制作フロー: `docs/YarnEditingPipeline.md`, `docs/SCENARIO_AUTHORING_GUIDE.md`
- UI / 挙動: `docs/DISPLAY_ALGORITHMS.md`, `docs/UI_ISSUES.md`, 該当 StorySpec
- 実装監査: `docs/FEATURE_STATUS_AUDIT.md`, `docs/spec-index.json`
- Ch1 サブクエスト検証: `docs/ai/TASK_PROMPT_ch1_sidequest_verification.md`
- SP-023 / SP-024 表示系: `docs/plans/display-batch-showcase.md`

## 境界

- `AGENTS.md`, `CLAUDE.md`, `.claude/CLAUDE.md`, `prompt-resume.md` は薄い入口に留め、別の読書順を持たせない。
- `docs/archive/` は明示依頼がない限り読まない。
